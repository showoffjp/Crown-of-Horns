using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.Characters
{
    public enum TargetingMode { Self, SingleEnemy, SingleAlly, AnyTile, AreaBurst }
    public enum ActionCost { Action, BonusAction, Reaction, Free }

    /// <summary>
    /// A usable ability: a weapon attack, spell, or maneuver. Fully data-driven so
    /// the whole spell/skill list lives in ScriptableObject assets, not code.
    /// Supports attack rolls vs AC, saving throws (with save-for-half), healing,
    /// area effects, and applying status conditions.
    ///
    /// Create via: Assets > Create > Sundered Crown > Ability
    /// </summary>
    [CreateAssetMenu(menuName = "Sundered Crown/Ability", fileName = "NewAbility")]
    public class AbilityDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string abilityName = "Strike";
        [TextArea(2, 4)] public string description;
        public Sprite icon;

        [Header("Economy")]
        public ActionCost cost = ActionCost.Action;
        [Tooltip("Spell slot level required, or 0 for at-will / cantrips / weapon attacks.")]
        public int spellSlotLevel = 0;

        [Header("Targeting")]
        public TargetingMode targeting = TargetingMode.SingleEnemy;
        [Tooltip("Max range in tiles. 1 = melee/adjacent.")]
        public int rangeTiles = 1;
        [Tooltip("Radius in tiles for AreaBurst targeting (Fireball ~ 3).")]
        public int areaRadiusTiles = 0;

        [Header("Resolution")]
        [Tooltip("If true, roll a d20 attack vs AC. If false, the target rolls a saving throw.")]
        public bool isAttackRoll = true;
        public Ability saveAbility = Ability.Dexterity;
        [Tooltip("On a successful save, deal half damage instead of none (Fireball-style).")]
        public bool saveForHalf = false;

        [Header("Damage / Healing")]
        [Tooltip("Damage dice notation, e.g. '1d8', '8d6'. Blank for non-damaging.")]
        public string damageDice = "1d8";
        public DamageType damageType = DamageType.Slashing;
        [Tooltip("Adds the caster's relevant ability modifier to damage.")]
        public bool addAbilityModToDamage = true;
        [Tooltip("If true, this is a healing ability: 'damageDice' is rolled as healing on an ally/self.")]
        public bool isHeal = false;
        public string healDice = "";

        [Header("Applied condition (on hit / failed save)")]
        public Combat.StatusEffectDefinition appliedEffect;
        [Tooltip("Duration in rounds for the applied effect; 0 uses the effect's own default.")]
        public int appliedEffectRounds = 0;
    }
}
