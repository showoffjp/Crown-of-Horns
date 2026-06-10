using System;
using System.Text.RegularExpressions;
using SunderedCrown.Characters;
using SunderedCrown.Stats;

namespace SunderedCrown.Combat
{
    /// <summary>A read-only preview of what an ability is likely to do, before you commit to it.</summary>
    public struct Forecast
    {
        public bool isHeal;
        public bool isSave;            // saving-throw ability (else an attack roll)
        public bool advantage;
        public bool disadvantage;
        public float hitChance;        // P(attack lands) — or, for saves, P(target fails). 0..1
        public float critChance;       // P(natural crit). 0..1 (0 for saves/heals)
        public float expectedDamage;   // mean damage dealt, accounting for miss / crit / save-for-half
        public float expectedHealing;  // mean healing (heals only)
        public float lethalChance;     // P(this single use drops the target). 0..1

        /// <summary>XCOM-style one-liner for the HUD, e.g. "72% · ~6 dmg · 18% kill".</summary>
        public string Summary()
        {
            if (isHeal) return $"heal ~{expectedHealing:0.#}";
            string verb = isSave ? "fail" : "hit";
            string s = $"{Pct(hitChance)} {verb} · ~{expectedDamage:0.#} dmg";
            if (critChance > 0.0001f) s += $" · {Pct(critChance)} crit";
            if (lethalChance > 0.005f) s += $" · {Pct(lethalChance)} kill";
            return s;
        }

        private static string Pct(float p) => $"{(int)Math.Round(p * 100)}%";
    }

    /// <summary>
    /// Analytic previews of an ability's outcome — the "72% hit, 18% to kill" the combat HUD shows
    /// before you spend an action. It mirrors <see cref="AttackResolver"/> exactly: the same driving
    /// ability modifier, proficiency, advantage/disadvantage sources, crit-doubles-dice, and
    /// save-for-half rules — so the forecast equals the dice in the long run. A Monte-Carlo cross-check
    /// in AttackForecastTests pins forecast ≈ empirical. Pure logic; no scene, no Unity types.
    /// </summary>
    public static class AttackForecast
    {
        public static Forecast Of(CharacterSheet attacker, CharacterSheet target, AbilityDefinition ability,
            bool extraAdvantage = false, int targetAcBonus = 0)
        {
            var fc = new Forecast { isHeal = ability.isHeal, isSave = !ability.isAttackRoll && !ability.isHeal };

            // Same driving modifier the resolver uses.
            Ability mod = ability.spellSlotLevel > 0 || ability.isHeal
                ? attacker.SpellcastingAbility
                : (attacker.classDef != null ? attacker.classDef.primaryAbility : Ability.Strength);
            int abilityMod = attacker.Modifier(mod);

            if (ability.isHeal)
            {
                string dice = !string.IsNullOrWhiteSpace(ability.healDice) ? ability.healDice : ability.damageDice;
                fc.expectedHealing = Math.Max(0f, Average(dice) + abilityMod);
                fc.hitChance = 1f;
                return fc;
            }

            var (count, sides, flat) = Parse(ability.damageDice);
            float diceAvg = (count == 0 || sides == 0) ? 0f : count * (sides + 1) / 2f;
            int modDmg = ability.addAbilityModToDamage ? abilityMod : 0;
            int hp = Math.Max(1, target.currentHitPoints);

            if (ability.isAttackRoll)
            {
                bool advSource = target.GrantsAdvantageToAttackers || attacker.HasHelpAdvantage || extraAdvantage;
                bool disadvSource = attacker.AttacksAtDisadvantage || target.IsDodging;
                fc.advantage = advSource && !disadvSource;
                fc.disadvantage = disadvSource && !advSource;

                int toHit = abilityMod + attacker.ProficiencyBonus + attacker.EffectAttackModifier;
                int ac = target.ArmorClass + targetAcBonus;
                float pSingle = SingleRollHitChance(toHit, ac); // one d20 (nat-20 always, nat-1 never)
                const float pCritSingle = 0.05f;

                if (fc.advantage)        { fc.hitChance = 1f - (1f - pSingle) * (1f - pSingle); fc.critChance = 1f - (1f - pCritSingle) * (1f - pCritSingle); }
                else if (fc.disadvantage){ fc.hitChance = pSingle * pSingle;                    fc.critChance = pCritSingle * pCritSingle; }
                else                     { fc.hitChance = pSingle;                              fc.critChance = pCritSingle; }

                float pHitNotCrit = Math.Max(0f, fc.hitChance - fc.critChance);
                float normalAvg = diceAvg + flat + modDmg;          // 1 roll of the notation, + mod
                float critAvg = 2f * (diceAvg + flat) + modDmg;     // crit doubles the whole roll (dice + its flat)
                fc.expectedDamage = Math.Max(0f, pHitNotCrit * normalAvg + fc.critChance * critAvg);

                // Lethality from the exact damage distribution.
                double pNormalKill = PAtLeast(SumDist(count, sides), flat + modDmg, hp);
                double pCritKill = PAtLeast(SumDist(count * 2, sides), 2 * flat + modDmg, hp);
                fc.lethalChance = (float)(pHitNotCrit * pNormalKill + fc.critChance * pCritKill);
            }
            else
            {
                int saveDC = 8 + attacker.ProficiencyBonus + abilityMod;
                int saveMod = target.Modifier(ability.saveAbility);
                float pFail = FailSaveChance(saveDC, saveMod);
                fc.hitChance = pFail; // "hit" == failed save

                float full = diceAvg + flat + modDmg;
                fc.expectedDamage = Math.Max(0f, ability.saveForHalf
                    ? pFail * full + (1f - pFail) * (float)Math.Floor(full / 2.0)
                    : pFail * full);

                var dist = SumDist(count, sides);
                int shift = flat + modDmg;
                double pFullKill = PAtLeast(dist, shift, hp);
                double lethal = pFail * pFullKill;
                if (ability.saveForHalf)
                    lethal += (1f - pFail) * PAtLeast(dist, shift, 2 * hp); // floor(x/2) >= hp  <=>  x >= 2*hp
                fc.lethalChance = (float)lethal;
            }
            return fc;
        }

        // ---- 5e probability helpers ---------------------------------------

        private static float SingleRollHitChance(int toHit, int ac)
        {
            int hits = 0;
            for (int f = 1; f <= 20; f++)
                if (f == 20 || (f != 1 && f + toHit >= ac)) hits++; // nat-20 always, nat-1 never
            return hits / 20f;
        }

        private static float FailSaveChance(int dc, int saveMod)
        {
            int fails = 0;
            for (int f = 1; f <= 20; f++) if (f + saveMod < dc) fails++; // no nat-1/20 special-casing on saves
            return fails / 20f;
        }

        // ---- dice notation + distribution ---------------------------------

        private static readonly Regex Notation =
            new Regex(@"^\s*(\d+)d(\d+)\s*([+-]\s*\d+)?\s*$", RegexOptions.Compiled);

        private static (int count, int sides, int flat) Parse(string notation)
        {
            var m = Notation.Match(notation ?? "");
            if (!m.Success) return (0, 0, 0);
            int count = int.Parse(m.Groups[1].Value);
            int sides = int.Parse(m.Groups[2].Value);
            int flat = m.Groups[3].Success ? int.Parse(m.Groups[3].Value.Replace(" ", "")) : 0;
            return (count, sides, flat);
        }

        /// <summary>Mean of dice notation: X*(Y+1)/2 + Z. Blank/invalid → 0.</summary>
        public static float Average(string notation)
        {
            var (c, s, f) = Parse(notation);
            return (c == 0 || s == 0) ? f : c * (s + 1) / 2f + f;
        }

        /// <summary>Probability mass of the sum of <paramref name="count"/> d<paramref name="sides"/>,
        /// indexed by sum (index 0..count*sides). count==0 → a point mass at 0.</summary>
        private static double[] SumDist(int count, int sides)
        {
            var cur = new double[] { 1.0 }; // sum 0, prob 1
            if (sides <= 0) return cur;
            for (int d = 0; d < count; d++)
            {
                var next = new double[cur.Length + sides];
                for (int s = 0; s < cur.Length; s++)
                    if (cur[s] > 0)
                        for (int face = 1; face <= sides; face++)
                            next[s + face] += cur[s] / sides;
                cur = next;
            }
            return cur;
        }

        private static double PAtLeast(double[] dist, int shift, int threshold)
        {
            double p = 0;
            for (int s = 0; s < dist.Length; s++)
                if (dist[s] > 0 && s + shift >= threshold) p += dist[s];
            return p;
        }
    }
}
