using System;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Stats;
using SunderedCrown.Combat;
using SunderedCrown.Items;

namespace SunderedCrown.Characters
{
    /// <summary>Spell slots by level (index 1..9; index 0 unused / at-will).</summary>
    [Serializable]
    public class SpellSlots
    {
        public int[] max = new int[10];
        public int[] current = new int[10];

        public bool Has(int level) => level <= 0 || (level < 10 && current[level] > 0);

        public bool Spend(int level)
        {
            if (level <= 0) return true;          // cantrips / weapon attacks: free
            if (level < 10 && current[level] > 0) { current[level]--; return true; }
            return false;
        }

        public void RestoreAll()
        {
            for (int i = 0; i < 10; i++) current[i] = max[i];
        }
    }

    /// <summary>
    /// The runtime state of one creature. Plain serializable POCO (not a MonoBehaviour)
    /// so it saves/loads and unit-tests cleanly. A GridUnit wraps it on the map.
    /// Now includes spell slots, equipment, and active status effects.
    /// </summary>
    [Serializable]
    public class CharacterSheet
    {
        public string id = Guid.NewGuid().ToString();
        public string displayName = "Unnamed";

        [Header("Build")]
        public ClassDefinition classDef;
        public RaceDefinition raceDef;
        public AbilityScores abilities = new AbilityScores();
        public int level = 1;
        public int experience = 0;

        [Tooltip("XP this creature is worth when defeated (enemies).")]
        public int experienceValue = 0;

        [Header("Vitals")]
        public int maxHitPoints = 1;
        public int currentHitPoints = 1;
        public int baseArmorClass = 10;

        [Header("Resources")]
        public SpellSlots spellSlots = new SpellSlots();

        [Header("Loadout")]
        public List<AbilityDefinition> knownAbilities = new List<AbilityDefinition>();
        [Tooltip("Equipped weapon supplies the default attack ability.")]
        public AbilityDefinition equippedWeaponAbility;
        [Tooltip("Flat AC from worn armor + shield (computed by the equipment system).")]
        public int armorClassFromGear = 0;
        public Items.Equipment equipment = new Items.Equipment();

        [Header("Loot (what this creature drops when defeated)")]
        public List<ItemDefinition> loot = new List<ItemDefinition>();
        public int lootGold = 0;

        [NonSerialized] public List<EffectInstance> activeEffects = new List<EffectInstance>();

        /// <summary>The 5e Dodge action: set when the unit Defends, cleared at the start of its next turn.
        /// While true, attack rolls against this unit are made at disadvantage.</summary>
        [NonSerialized] public bool IsDodging = false;

        /// <summary>The 5e Help action: an ally granted this makes its next attack roll with advantage.
        /// Consumed the next time this unit makes an attack-roll ability.</summary>
        [NonSerialized] public bool HasHelpAdvantage = false;

        /// <summary>The 5e Disengage action: while set, moving away from melee does not provoke opportunity
        /// attacks. Set when the unit Disengages, cleared at the start of its next turn.</summary>
        [NonSerialized] public bool IsDisengaging = false;

        /// <summary>The round number in which this unit last spent its reaction (opportunity attack). One
        /// reaction per round; 0 means none used yet. Compared against TurnManager.RoundNumber.</summary>
        [NonSerialized] public int lastReactionRound = 0;

        public bool IsAlive => currentHitPoints > 0;

        // ---- Derived stats (5e math) -------------------------------------

        public int ProficiencyBonus => 2 + (level - 1) / 4;
        public int Modifier(Ability ability) => abilities.Modifier(ability);

        public int SpeedTiles
        {
            get
            {
                int speed = raceDef != null ? raceDef.baseSpeedTiles : 6;
                foreach (var e in activeEffects) speed += e.def != null ? e.def.speedModifier : 0;
                return Mathf.Max(0, speed);
            }
        }

        public int ArmorClass
        {
            get
            {
                int ac = baseArmorClass + armorClassFromGear + Modifier(Ability.Dexterity);
                foreach (var e in activeEffects) ac += e.def != null ? e.def.armorClassModifier : 0;
                return ac;
            }
        }

        public int InitiativeModifier => Modifier(Ability.Dexterity);

        public Ability SpellcastingAbility =>
            classDef != null ? classDef.spellcastingAbility : Ability.Intelligence;

        // ---- Status-effect queries ---------------------------------------

        public bool HasCondition(Condition c)
        {
            foreach (var e in activeEffects) if (e.def != null && e.def.condition == c) return true;
            return false;
        }

        public bool IsIncapacitated
        {
            get { foreach (var e in activeEffects) if (e.def != null && e.def.incapacitates) return true; return false; }
        }

        public bool GrantsAdvantageToAttackers
        {
            get { foreach (var e in activeEffects) if (e.def != null && e.def.attackersHaveAdvantage) return true; return false; }
        }

        public bool AttacksAtDisadvantage
        {
            get
            {
                foreach (var e in activeEffects)
                    if (e.def != null && (e.def.bearerAttacksDisadvantage || e.def.incapacitates)) return true;
                return false;
            }
        }

        /// <summary>Sum of flat to-hit modifiers from effects (Blessed, etc.).</summary>
        public int EffectAttackModifier
        {
            get { int m = 0; foreach (var e in activeEffects) if (e.def != null) m += e.def.attackRollModifier; return m; }
        }

        // ---- Status-effect mutations -------------------------------------

        public void AddEffect(StatusEffectDefinition def, int rounds, string sourceId)
        {
            if (def == null) return;
            // Refresh duration if the same effect is already present.
            foreach (var e in activeEffects)
                if (e.def == def) { e.remainingRounds = Mathf.Max(e.remainingRounds, rounds); return; }
            activeEffects.Add(new EffectInstance(def, rounds, sourceId));
        }

        public void RemoveCondition(Condition c) =>
            activeEffects.RemoveAll(e => e.def != null && e.def.condition == c);

        /// <summary>Start of turn: apply DoT. Returns total damage dealt this tick.</summary>
        public int TickStartOfTurn()
        {
            int total = 0;
            foreach (var e in activeEffects)
            {
                if (e.def != null && !string.IsNullOrWhiteSpace(e.def.damageOverTimeDice))
                {
                    int dmg = Dice.Roll(e.def.damageOverTimeDice);
                    TakeDamage(dmg);
                    total += dmg;
                }
            }
            return total;
        }

        /// <summary>End of turn: count down durations and drop expired effects.</summary>
        public void TickEndOfTurn()
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                activeEffects[i].remainingRounds--;
                if (activeEffects[i].remainingRounds <= 0) activeEffects.RemoveAt(i);
            }
        }

        // ---- Vitals ------------------------------------------------------

        public void TakeDamage(int amount) =>
            currentHitPoints = Mathf.Max(0, currentHitPoints - Mathf.Max(0, amount));

        public void Heal(int amount) =>
            currentHitPoints = Mathf.Min(maxHitPoints, currentHitPoints + Mathf.Max(0, amount));

        public void RecalculateMaxHitPoints()
        {
            if (classDef == null) { maxHitPoints = Math.Max(1, currentHitPoints); return; }
            int conMod = Modifier(Ability.Constitution);
            int hp = classDef.hitDie + conMod;
            for (int lvl = 2; lvl <= level; lvl++)
                hp += classDef.AverageHitDieGain + conMod;
            maxHitPoints = Math.Max(1, hp);
            currentHitPoints = maxHitPoints;
        }

        public int RollInitiative() => Dice.D20() + InitiativeModifier;

        /// <summary>The ability used for a basic attack (equipped weapon, else first known).</summary>
        public AbilityDefinition DefaultAttack =>
            equippedWeaponAbility != null ? equippedWeaponAbility
            : (knownAbilities.Count > 0 ? knownAbilities[0] : null);

        /// <summary>
        /// A combat-ready copy of this sheet (for the Echoes — twisted mirrors of the party).
        /// Shares the immutable ScriptableObject refs (class/race/abilities) but gets a fresh id,
        /// fresh resources, and full HP. Does NOT copy active effects or loot.
        /// </summary>
        public CharacterSheet Clone()
        {
            var c = new CharacterSheet
            {
                id = Guid.NewGuid().ToString(),
                displayName = displayName,
                classDef = classDef,
                raceDef = raceDef,
                level = level,
                baseArmorClass = baseArmorClass,
                armorClassFromGear = armorClassFromGear,
                experienceValue = experienceValue,
                lootGold = lootGold,
                equippedWeaponAbility = equippedWeaponAbility,
                knownAbilities = new List<AbilityDefinition>(knownAbilities),
            };
            foreach (Ability a in Enum.GetValues(typeof(Ability))) c.abilities.Set(a, abilities.Get(a));
            Array.Copy(spellSlots.max, c.spellSlots.max, 10);
            Array.Copy(spellSlots.current, c.spellSlots.current, 10);
            c.maxHitPoints = maxHitPoints;
            c.currentHitPoints = maxHitPoints;
            return c;
        }
    }
}
