using System;
using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// Data-only definition of a status effect (Poisoned, Prone, Blessed, Burning...).
    /// The mechanical consequences are expressed as flags/values here so the whole
    /// condition system is authored in assets, never hard-coded.
    ///
    /// Create via: Assets > Create > Sundered Crown > Status Effect
    /// </summary>
    [CreateAssetMenu(menuName = "Sundered Crown/Status Effect", fileName = "NewStatusEffect")]
    public class StatusEffectDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string effectName = "Effect";
        public Condition condition = Condition.None;
        [TextArea(2, 4)] public string description;
        public Sprite icon;
        public Color tint = Color.white;
        public bool isBeneficial = false;

        [Header("Duration")]
        [Tooltip("Rounds the effect lasts on its bearer. Ticks down at end of the bearer's turn.")]
        public int durationRounds = 2;

        [Header("Damage over time (applied at start of bearer's turn)")]
        [Tooltip("Dice rolled each turn, e.g. '1d6'. Blank = no DoT.")]
        public string damageOverTimeDice = "";
        public DamageType damageOverTimeType = DamageType.Fire;

        [Header("Mechanical effects")]
        [Tooltip("Bearer cannot take actions or move (Stunned/Incapacitated/Paralyzed).")]
        public bool incapacitates = false;
        [Tooltip("Attacks AGAINST the bearer are made with advantage (Prone/Restrained/Stunned).")]
        public bool attackersHaveAdvantage = false;
        [Tooltip("The bearer's own attacks are made with disadvantage (Poisoned/Frightened).")]
        public bool bearerAttacksDisadvantage = false;
        [Tooltip("Flat modifier to the bearer's attack rolls (Blessed = +2-ish).")]
        public int attackRollModifier = 0;
        [Tooltip("Flat modifier to the bearer's Armor Class while active.")]
        public int armorClassModifier = 0;
        [Tooltip("Flat modifier to the bearer's movement tiles (Hasted +, Slowed -).")]
        public int speedModifier = 0;
    }

    /// <summary>A runtime instance of a status effect on a particular creature.</summary>
    [Serializable]
    public class EffectInstance
    {
        public StatusEffectDefinition def;
        public int remainingRounds;
        public string sourceId; // who applied it (for logs / concentration later)

        public EffectInstance(StatusEffectDefinition def, int rounds, string sourceId)
        {
            this.def = def;
            this.remainingRounds = rounds;
            this.sourceId = sourceId;
        }
    }
}
