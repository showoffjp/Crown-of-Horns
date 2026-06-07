using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Stats;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// A tiny, deterministic combat-balance smoke test. It builds two throwaway sheets entirely by hand (no
    /// scene, no content dependencies) and simulates many seeded melee duels through the *real*
    /// <see cref="AttackResolver"/> math, reporting a win-rate. The point isn't a perfect model — there's no
    /// movement or positioning here — it's a fast canary: if a change to the resolver, modifiers, or crits
    /// wildly skews these numbers, you'll see it. Pure logic; `CombatBalanceDemo` runs it and logs the report.
    /// </summary>
    public static class CombatBalance
    {
        /// <summary>Simulate a fixed-position slugfest: A swings, then B, until someone drops. Returns true if
        /// A wins (ties on the turn cap go to whoever has more HP remaining). Resets both sheets' HP first.</summary>
        public static bool DuelAWins(CharacterSheet a, CharacterSheet b, AbilityDefinition atkA, AbilityDefinition atkB, int maxTurns = 200)
        {
            a.currentHitPoints = a.maxHitPoints;
            b.currentHitPoints = b.maxHitPoints;
            for (int t = 0; t < maxTurns; t++)
            {
                AttackResolver.Apply(b, AttackResolver.Resolve(a, b, atkA));
                if (!b.IsAlive) return true;
                AttackResolver.Apply(a, AttackResolver.Resolve(b, a, atkB));
                if (!a.IsAlive) return false;
            }
            return a.currentHitPoints >= b.currentHitPoints;
        }

        /// <summary>Run a couple of representative matchups over many seeds and return a win-rate report.</summary>
        public static string Report(int trials = 400)
        {
            int p1 = WinRate(Sheet("Hero", 16, 16, 28), Sheet("Brute", 14, 13, 22),
                             Weapon("Longsword", "1d8"), Weapon("Club", "1d6"), trials);
            // A glass cannon (hits hard, frail, lightly armored) vs the same brute — should be a coin-flip-ish.
            int p2 = WinRate(Sheet("Duelist", 18, 14, 20), Sheet("Brute", 14, 13, 22),
                             Weapon("Rapier", "1d10"), Weapon("Club", "1d6"), trials);

            string v1 = (p1 >= 55 && p1 <= 85) ? "OK" : p1 > 85 ? "HIGH" : "LOW";
            string v2 = (p2 >= 35 && p2 <= 70) ? "OK" : p2 > 70 ? "HIGH" : "LOW";
            return $"⚖ Combat balance ({trials} duels ea.) — Hero vs Brute {p1}% [{v1}] · Duelist vs Brute {p2}% [{v2}]";
        }

        private static int WinRate(CharacterSheet a, CharacterSheet b, AbilityDefinition atkA, AbilityDefinition atkB, int trials)
        {
            int wins = 0;
            for (int i = 0; i < trials; i++) { Dice.Seed(i); if (DuelAWins(a, b, atkA, atkB)) wins++; }
            return Mathf.RoundToInt(100f * wins / Mathf.Max(1, trials));
        }

        private static CharacterSheet Sheet(string name, int str, int dexAcBase, int hp)
        {
            var s = new CharacterSheet { displayName = name, classDef = null, level = 1, baseArmorClass = dexAcBase };
            s.abilities.Set(Ability.Strength, str);
            s.abilities.Set(Ability.Dexterity, 10);
            s.abilities.Set(Ability.Constitution, 12);
            s.maxHitPoints = hp;
            s.currentHitPoints = hp;
            return s;
        }

        private static AbilityDefinition Weapon(string name, string dice)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = name; a.damageDice = dice; a.damageType = DamageType.Slashing;
            a.rangeTiles = 1; a.isAttackRoll = true; a.addAbilityModToDamage = true;
            return a;
        }
    }
}
