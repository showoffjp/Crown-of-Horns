using UnityEngine;

namespace SunderedCrown.Characters
{
    /// <summary>
    /// A 5e-style background (Candlekeep Acolyte, Flaming Fist Deserter, etc.).
    /// Grants skill proficiencies, a feature, a starting item, and — important for a
    /// reactive CRPG — a GameFlags key that unlocks background-specific dialogue.
    ///
    /// Create via: Assets > Create > Sundered Crown > Background
    /// </summary>
    [CreateAssetMenu(menuName = "Sundered Crown/Background", fileName = "NewBackground")]
    public class BackgroundDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string backgroundName = "Folk Hero";
        [TextArea(2, 4)] public string description;

        [Header("Proficiencies")]
        public string[] skillProficiencies;
        public string[] toolProficiencies;

        [Header("Feature")]
        public string featureName;
        [TextArea(2, 4)] public string featureText;

        [Header("Reactivity")]
        [Tooltip("GameFlags bool set true at creation, e.g. 'bg.candlekeep_acolyte'. Drives unique dialogue.")]
        public string grantsFlag;

        [Header("Starting kit")]
        public Items.ItemDefinition startingItem;
        public int startingGold = 15;
    }
}
