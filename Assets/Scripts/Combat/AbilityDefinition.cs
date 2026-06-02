using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.Characters
{
    public enum TargetingMode { Self, SingleEnemy, SingleAlly, AnyTile, AreaBurst }
    public enum ActionCost { Action, BonusAction, Reaction, Free }

    /// <summary>
    /// A usable ability: a weapon attack, spell, or special maneuver.
    /// Kept deliberately generic and data-driven so the entire spell/skill list
    /// lives in ScriptableObject assets, not code.
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
        [Tooltip("Spell slot level required, or 0 for at-will / weapon attacks.")]
        public int spellSlotLevel = 0;

        [Header("Targeting")]
        public TargetingMode targeting = TargetingMode.SingleEnemy;
        [Tooltip("Max range in tiles. 1 = melee/adjacent.")]
        public int rangeTiles = 1;
        [Tooltip("Radius in tiles for AreaBurst targeting.")]
        public int areaRadiusTiles = 0;

        [Header("Resolution")]
        [Tooltip("If true, roll a d20 attack vs the target's AC. If false, the target rolls a saving throw.")]
        public bool isAttackRoll = true;
        public Stats.Ability saveAbility = Stats.Ability.Dexterity;
        [Tooltip("Damage dice notation, e.g. '1d8', '2d6'. Leave blank for non-damaging.")]
        public string damageDice = "1d8";
        public DamageType damageType = DamageType.Slashing;
        [Tooltip("If true, the ability adds the caster's relevant ability modifier to damage.")]
        public bool addAbilityModToDamage = true;
    }
}
