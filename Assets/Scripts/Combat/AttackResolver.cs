using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Stats;

namespace SunderedCrown.Combat
{
    public struct AttackResult
    {
        public bool hit;
        public bool critical;
        public int attackRoll;     // natural d20 face
        public int totalToHit;     // d20 + mods
        public int targetAC;
        public int damage;
        public DamageType damageType;
        public string log;
    }

    /// <summary>
    /// Pure functions that resolve an ability use between two characters using
    /// 5e-style math. No Unity scene dependencies → easy to unit test.
    /// </summary>
    public static class AttackResolver
    {
        /// <summary>
        /// Resolve a single ability use from attacker against target.
        /// Handles both attack-roll abilities (vs AC) and saving-throw abilities.
        /// </summary>
        public static AttackResult Resolve(CharacterSheet attacker, CharacterSheet target, AbilityDefinition ability)
        {
            var r = new AttackResult { damageType = ability.damageType };

            // Which ability modifier drives this? Spellcasters use their casting stat;
            // weapon attacks default to the class primary (Str/Dex).
            Ability mod = ability.spellSlotLevel > 0
                ? attacker.SpellcastingAbility
                : (attacker.classDef != null ? attacker.classDef.primaryAbility : Ability.Strength);
            int abilityMod = attacker.Modifier(mod);

            if (ability.isAttackRoll)
            {
                r.attackRoll = Dice.D20();
                r.critical = r.attackRoll == 20;
                bool autoMiss = r.attackRoll == 1;
                r.totalToHit = r.attackRoll + abilityMod + attacker.ProficiencyBonus;
                r.targetAC = target.ArmorClass;
                r.hit = !autoMiss && (r.critical || r.totalToHit >= r.targetAC);
            }
            else
            {
                // Saving throw: target rolls d20 + save mod vs the attacker's save DC.
                int saveDC = 8 + attacker.ProficiencyBonus + abilityMod;
                int save = Dice.D20() + target.Modifier(ability.saveAbility);
                r.targetAC = saveDC;          // reuse field to surface the DC in logs
                r.totalToHit = save;
                r.hit = save < saveDC;        // "hit" == failed save
                r.critical = false;
            }

            if (r.hit && !string.IsNullOrWhiteSpace(ability.damageDice))
            {
                r.damage = Dice.Roll(ability.damageDice);
                if (r.critical) r.damage += Dice.Roll(ability.damageDice); // crit: double the dice
                if (ability.addAbilityModToDamage) r.damage += abilityMod;
                r.damage = Mathf.Max(0, r.damage);
            }

            r.log = BuildLog(attacker, target, ability, r);
            return r;
        }

        /// <summary>Apply the resolved result to the target's sheet.</summary>
        public static void Apply(CharacterSheet target, in AttackResult result)
        {
            if (result.hit && result.damage > 0)
                target.TakeDamage(result.damage);
        }

        private static string BuildLog(CharacterSheet a, CharacterSheet t, AbilityDefinition ab, in AttackResult r)
        {
            if (!r.hit)
                return ab.isAttackRoll
                    ? $"{a.displayName}'s {ab.abilityName} misses {t.displayName} ({r.totalToHit} vs AC {r.targetAC})."
                    : $"{t.displayName} resists {a.displayName}'s {ab.abilityName} (save {r.totalToHit} vs DC {r.targetAC}).";

            string crit = r.critical ? " CRITICAL!" : "";
            return $"{a.displayName}'s {ab.abilityName} hits {t.displayName} for {r.damage} {r.damageType} damage.{crit}";
        }
    }
}
