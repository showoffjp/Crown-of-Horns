using System;

namespace SunderedCrown.Stats
{
    public enum Ability
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }

    public enum DamageType
    {
        Slashing, Piercing, Bludgeoning,
        Fire, Cold, Lightning, Acid, Poison,
        Necrotic, Radiant, Psychic, Force, Thunder
    }

    /// <summary>
    /// The six 5e-style ability scores. Plain serializable class so it survives
    /// JSON save/load and shows up in the Unity inspector.
    /// </summary>
    [Serializable]
    public class AbilityScores
    {
        public int strength = 10;
        public int dexterity = 10;
        public int constitution = 10;
        public int intelligence = 10;
        public int wisdom = 10;
        public int charisma = 10;

        public int Get(Ability ability) => ability switch
        {
            Ability.Strength     => strength,
            Ability.Dexterity    => dexterity,
            Ability.Constitution => constitution,
            Ability.Intelligence => intelligence,
            Ability.Wisdom       => wisdom,
            Ability.Charisma     => charisma,
            _ => 10
        };

        public void Set(Ability ability, int value)
        {
            switch (ability)
            {
                case Ability.Strength:     strength = value; break;
                case Ability.Dexterity:    dexterity = value; break;
                case Ability.Constitution: constitution = value; break;
                case Ability.Intelligence: intelligence = value; break;
                case Ability.Wisdom:       wisdom = value; break;
                case Ability.Charisma:     charisma = value; break;
            }
        }

        /// <summary>5e ability modifier: floor((score - 10) / 2).</summary>
        public int Modifier(Ability ability)
        {
            int score = Get(ability);
            return (int)Math.Floor((score - 10) / 2.0);
        }
    }
}
