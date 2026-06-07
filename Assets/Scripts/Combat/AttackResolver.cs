using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Stats;

namespace SunderedCrown.Combat
{
    public struct AttackResult
    {
        public bool hit;            // attack landed, OR (for saves) the target failed its save
        public bool critical;
        public bool isHeal;
        public bool advantage;
        public bool disadvantage;

        public int attackRoll;      // natural d20 face (attack-roll abilities)
        public int totalToHit;      // d20 + mods, or the target's save roll
        public int targetAC;        // target AC, or the save DC

        public int damage;
        public int healing;
        public DamageType damageType;

        public StatusEffectDefinition effectApplied; // non-null if a condition should land
        public int effectRounds;
        public string sourceId;

        public string log;
    }

    /// <summary>
    /// Pure 5e-style resolution functions. No scene dependency → deterministically
    /// testable (seed Dice, assert outcomes). Handles attack rolls vs AC, saving
    /// throws (with save-for-half), healing, advantage/disadvantage from conditions,
    /// crits, and queuing a status effect to apply.
    /// </summary>
    public static class AttackResolver
    {
        public static AttackResult Resolve(CharacterSheet attacker, CharacterSheet target, AbilityDefinition ability,
            bool extraAdvantage = false, int targetAcBonus = 0)
        {
            var r = new AttackResult { damageType = ability.damageType, sourceId = attacker.id };

            // Driving ability modifier: spells use the casting stat; weapons use class primary.
            Ability mod = ability.spellSlotLevel > 0 || ability.isHeal
                ? attacker.SpellcastingAbility
                : (attacker.classDef != null ? attacker.classDef.primaryAbility : Ability.Strength);
            int abilityMod = attacker.Modifier(mod);

            // ---- Healing short-circuit ----
            if (ability.isHeal)
            {
                r.isHeal = true;
                r.hit = true;
                string dice = !string.IsNullOrWhiteSpace(ability.healDice) ? ability.healDice : ability.damageDice;
                r.healing = Mathf.Max(0, Dice.Roll(dice) + abilityMod);
                r.log = $"{attacker.displayName}'s {ability.abilityName} heals {target.displayName} for {r.healing} HP.";
                return r;
            }

            bool effectShouldLand;

            if (ability.isAttackRoll)
            {
                // Advantage/disadvantage from conditions (they cancel out). The Dodge action makes attacks
                // against the dodger a source of disadvantage, just like the attacker's own conditions.
                bool advSource = target.GrantsAdvantageToAttackers || attacker.HasHelpAdvantage || extraAdvantage;
                bool disadvSource = attacker.AttacksAtDisadvantage || target.IsDodging;
                r.advantage = advSource && !disadvSource;
                r.disadvantage = disadvSource && !advSource;
                r.attackRoll = r.advantage ? Dice.D20Advantage()
                             : r.disadvantage ? Dice.D20Disadvantage()
                             : Dice.D20();

                r.critical = r.attackRoll == 20;
                bool autoMiss = r.attackRoll == 1;
                r.totalToHit = r.attackRoll + abilityMod + attacker.ProficiencyBonus + attacker.EffectAttackModifier;
                r.targetAC = target.ArmorClass + targetAcBonus; // cover adds to effective AC
                r.hit = !autoMiss && (r.critical || r.totalToHit >= r.targetAC);
                effectShouldLand = r.hit;
            }
            else
            {
                // Saving throw: target rolls d20 + save mod vs the attacker's spell save DC.
                int saveDC = 8 + attacker.ProficiencyBonus + abilityMod;
                int save = Dice.D20() + target.Modifier(ability.saveAbility);
                r.targetAC = saveDC;
                r.totalToHit = save;
                bool failedSave = save < saveDC;
                r.hit = failedSave;                 // "hit" == failed save
                effectShouldLand = failedSave;       // conditions only land on a failed save
            }

            // ---- Damage ----
            if (!string.IsNullOrWhiteSpace(ability.damageDice))
            {
                bool dealDamage = r.hit || (!ability.isAttackRoll && ability.saveForHalf);
                if (dealDamage)
                {
                    int dmg = Dice.Roll(ability.damageDice);
                    if (r.critical) dmg += Dice.Roll(ability.damageDice); // crit: double the dice
                    if (ability.addAbilityModToDamage) dmg += abilityMod;
                    if (!r.hit && ability.saveForHalf) dmg /= 2;          // made the save → half
                    r.damage = Mathf.Max(0, dmg);
                }
            }

            // ---- Queue applied condition ----
            if (ability.appliedEffect != null && effectShouldLand)
            {
                r.effectApplied = ability.appliedEffect;
                r.effectRounds = ability.appliedEffectRounds > 0
                    ? ability.appliedEffectRounds
                    : ability.appliedEffect.durationRounds;
            }

            r.log = BuildLog(attacker, target, ability, r);
            return r;
        }

        /// <summary>Apply a resolved result to the target's sheet.</summary>
        public static void Apply(CharacterSheet target, in AttackResult result)
        {
            if (result.isHeal) { target.Heal(result.healing); return; }
            if (result.damage > 0) target.TakeDamage(result.damage);
            if (result.effectApplied != null)
                target.AddEffect(result.effectApplied, result.effectRounds, result.sourceId);
        }

        private static string BuildLog(CharacterSheet a, CharacterSheet t, AbilityDefinition ab, in AttackResult r)
        {
            string advTag = r.advantage ? " (adv)" : r.disadvantage ? " (disadv)" : "";

            if (!r.hit)
            {
                if (ab.saveForHalf && r.damage > 0)
                    return $"{t.displayName} saves vs {a.displayName}'s {ab.abilityName}, takes {r.damage} {r.damageType} (half).";
                return ab.isAttackRoll
                    ? $"{a.displayName}'s {ab.abilityName} misses {t.displayName} ({r.totalToHit} vs AC {r.targetAC}){advTag}."
                    : $"{t.displayName} resists {a.displayName}'s {ab.abilityName} (save {r.totalToHit} vs DC {r.targetAC}).";
            }

            string crit = r.critical ? " CRITICAL!" : "";
            string cond = r.effectApplied != null ? $" [{r.effectApplied.effectName}]" : "";
            return $"{a.displayName}'s {ab.abilityName} hits {t.displayName} for {r.damage} {r.damageType}{advTag}.{crit}{cond}";
        }
    }
}
