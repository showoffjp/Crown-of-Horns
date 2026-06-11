using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SunderedCrown.Characters;
using SunderedCrown.Stats;

namespace SunderedCrown.Combat
{
    /// <summary>How dangerous a position is: what a set of attackers would do to one target next round.</summary>
    public struct Threat
    {
        public float expectedIncoming; // mean total damage the listed attackers would deal
        public float downChance;       // P(combined damage >= the target's current HP)
        public int attackerCount;

        /// <summary>HUD one-liner, e.g. "incoming ~14 · 38% down".</summary>
        public string Summary()
        {
            if (attackerCount == 0) return "safe";
            string s = $"incoming ~{expectedIncoming:0.#}";
            if (downChance > 0.005f) s += $" · {(int)Math.Round(downChance * 100)}% down";
            return s;
        }
    }

    /// <summary>
    /// The defensive companion to <see cref="AttackForecast"/>: given a target and the attackers that can
    /// reach it, how much damage is coming and what's the chance it drops this round. Combines each
    /// attacker's exact damage distribution by convolution (independent rolls), so the down-chance is the
    /// real probability, not a guess. Mirrors <see cref="AttackResolver"/>'s modifiers exactly; a
    /// Monte-Carlo cross-check in ThreatForecastTests pins forecast ≈ observed. Pure logic, no Unity types.
    /// </summary>
    public static class ThreatForecast
    {
        public static Threat Against(CharacterSheet target, IReadOnlyList<(CharacterSheet attacker, AbilityDefinition ability)> threats)
        {
            var t = new Threat();
            double[] combined = { 1.0 }; // P(0 damage) = 1, before adding any attacker
            float expected = 0f;
            if (threats != null)
            {
                foreach (var (att, ab) in threats)
                {
                    if (att == null || ab == null || ab.isHeal) continue;
                    var dist = DamageDistribution(att, target, ab, out float exp);
                    expected += exp;
                    combined = Convolve(combined, dist);
                    t.attackerCount++;
                }
            }
            t.expectedIncoming = expected;
            int hp = Math.Max(1, target.currentHitPoints);
            double down = 0;
            for (int d = hp; d < combined.Length; d++) down += combined[d];
            t.downChance = (float)Math.Min(1.0, Math.Max(0.0, down));
            return t;
        }

        /// <summary>Full damage distribution (index = damage dealt, value = probability) for one
        /// attacker/ability vs the target, including miss (0) / crit / save-for-half outcomes.</summary>
        public static double[] DamageDistribution(CharacterSheet attacker, CharacterSheet target, AbilityDefinition ability, out float expected)
        {
            Ability mod = ability.spellSlotLevel > 0 || ability.isHeal
                ? attacker.SpellcastingAbility
                : (attacker.classDef != null ? attacker.classDef.primaryAbility : Ability.Strength);
            int abilityMod = attacker.Modifier(mod);
            int modDmg = ability.addAbilityModToDamage ? abilityMod : 0;
            var (count, sides, flat) = Parse(ability.damageDice);

            var acc = new Dictionary<int, double>();
            void Add(int dmg, double p) { if (p <= 0) return; if (dmg < 0) dmg = 0; acc.TryGetValue(dmg, out var c); acc[dmg] = c + p; }

            if (ability.isAttackRoll)
            {
                bool advS = target.GrantsAdvantageToAttackers || attacker.HasHelpAdvantage;
                bool disS = attacker.AttacksAtDisadvantage || target.IsDodging;
                bool adv = advS && !disS, dis = disS && !advS;
                int toHit = abilityMod + attacker.ProficiencyBonus + attacker.EffectAttackModifier;
                int ac = target.ArmorClass;
                float pSingle = SingleHit(toHit, ac);
                const float pCritS = 0.05f;
                float pHit = adv ? 1f - (1f - pSingle) * (1f - pSingle) : dis ? pSingle * pSingle : pSingle;
                float pCrit = adv ? 1f - 0.95f * 0.95f : dis ? pCritS * pCritS : pCritS;
                float pHitNotCrit = Math.Max(0f, pHit - pCrit);

                Add(0, 1f - pHit);                                            // miss
                foreach (var (sum, p) in SumDist(count, sides)) Add(sum + flat + modDmg, p * pHitNotCrit);
                foreach (var (sum, p) in SumDist(count * 2, sides)) Add(sum + 2 * flat + modDmg, p * pCrit); // crit doubles dice
            }
            else
            {
                int dc = 8 + attacker.ProficiencyBonus + abilityMod;
                int saveMod = target.Modifier(ability.saveAbility);
                float pFail = FailSave(dc, saveMod);
                foreach (var (sum, p) in SumDist(count, sides))
                {
                    int full = sum + flat + modDmg;
                    Add(full, p * pFail);
                    if (ability.saveForHalf) Add(full / 2, p * (1f - pFail)); else Add(0, p * (1f - pFail));
                }
            }

            int max = 0; foreach (var k in acc.Keys) if (k > max) max = k;
            var arr = new double[max + 1];
            double e = 0;
            foreach (var kv in acc) { arr[kv.Key] += kv.Value; e += (double)kv.Key * kv.Value; }
            expected = (float)e;
            return arr;
        }

        // ---- helpers -------------------------------------------------------
        private static float SingleHit(int toHit, int ac)
        {
            int hits = 0;
            for (int f = 1; f <= 20; f++) if (f == 20 || (f != 1 && f + toHit >= ac)) hits++;
            return hits / 20f;
        }
        private static float FailSave(int dc, int saveMod)
        {
            int fails = 0;
            for (int f = 1; f <= 20; f++) if (f + saveMod < dc) fails++;
            return fails / 20f;
        }

        private static readonly Regex Notation = new Regex(@"^\s*(\d+)d(\d+)\s*([+-]\s*\d+)?\s*$", RegexOptions.Compiled);
        private static (int, int, int) Parse(string notation)
        {
            var m = Notation.Match(notation ?? "");
            if (!m.Success) return (0, 0, 0);
            int flat = m.Groups[3].Success ? int.Parse(m.Groups[3].Value.Replace(" ", "")) : 0;
            return (int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), flat);
        }

        // distribution of the sum of `count` d`sides`, yielded as (sum, probability)
        private static IEnumerable<(int sum, double p)> SumDist(int count, int sides)
        {
            double[] cur = { 1.0 };
            if (sides > 0)
                for (int d = 0; d < count; d++)
                {
                    var next = new double[cur.Length + sides];
                    for (int s = 0; s < cur.Length; s++)
                        if (cur[s] > 0)
                            for (int face = 1; face <= sides; face++)
                                next[s + face] += cur[s] / sides;
                    cur = next;
                }
            for (int s = 0; s < cur.Length; s++) if (cur[s] > 0) yield return (s, cur[s]);
        }

        private static double[] Convolve(double[] a, double[] b)
        {
            var r = new double[a.Length + b.Length - 1];
            for (int i = 0; i < a.Length; i++)
                if (a[i] > 0)
                    for (int j = 0; j < b.Length; j++)
                        if (b[j] > 0) r[i + j] += a[i] * b[j];
            return r;
        }
    }
}
