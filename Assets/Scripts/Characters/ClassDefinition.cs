using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.Characters
{
    /// <summary>
    /// Data-only definition of a character class (Fighter, Wizard, Cleric...).
    /// Authored as a ScriptableObject asset in Assets/Data/Classes so designers
    /// can create new classes without touching code.
    ///
    /// Create via: Assets > Create > Sundered Crown > Class Definition
    /// </summary>
    [CreateAssetMenu(menuName = "Sundered Crown/Class Definition", fileName = "NewClass")]
    public class ClassDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string className = "Fighter";
        [TextArea(2, 5)] public string description;

        [Header("Core")]
        [Tooltip("Hit die size, e.g. 10 for a d10 class.")]
        public int hitDie = 10;
        public Ability primaryAbility = Ability.Strength;
        public Ability savingThrowProficiencyA = Ability.Strength;
        public Ability savingThrowProficiencyB = Ability.Constitution;

        [Header("Spellcasting")]
        public bool isSpellcaster = false;
        public Ability spellcastingAbility = Ability.Intelligence;

        [Header("Progression")]
        [Tooltip("Abilities/spells granted automatically at each level (index 0 = level 1).")]
        public AbilityDefinition[] startingAbilities;

        /// <summary>Average HP gained per level after the first (5e 'fixed' rule).</summary>
        public int AverageHitDieGain => (hitDie / 2) + 1;
    }
}
