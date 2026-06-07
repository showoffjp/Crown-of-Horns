using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Era content for NETHERIL (-339 DR, Karsus's Folly): arcane abilities, the Netherese
    /// war-constructs, the time-displaced companion Naeve, and the era's framing + recruit
    /// dialogue. Built on top of the core SwordCoastContent (reuses its classes/races) so the
    /// party and rules stay consistent across eras.
    /// </summary>
    public class NetherilContent
    {
        private readonly SwordCoastContent _core;

        public readonly Dictionary<string, AbilityDefinition> Abilities = new Dictionary<string, AbilityDefinition>();
        public DialogueGraph ArrivalDialogue { get; private set; }
        public DialogueGraph NaeveRecruit { get; private set; }

        public NetherilContent(SwordCoastContent core)
        {
            _core = core;
            BuildAbilities();
            BuildDialogues();
        }

        // ---- arcane abilities ----
        private void BuildAbilities()
        {
            Abilities["arcane_bolt"] = Spell("Arcane Bolt", "1d10", DamageType.Force, range: 10, slot: 0, attack: true);

            var lance = Spell("Lightning Lance", "3d6", DamageType.Lightning, range: 8, slot: 2, attack: true);
            Abilities["lightning_lance"] = lance;

            var shatter = Spell("Shatterglass", "4d6", DamageType.Force, range: 9, slot: 2, attack: false);
            shatter.saveAbility = Ability.Dexterity; shatter.saveForHalf = true;
            shatter.targeting = TargetingMode.AreaBurst; shatter.areaRadiusTiles = 2;
            shatter.addAbilityModToDamage = false;
            Abilities["shatterglass"] = shatter;

            // Construct melee.
            Abilities["arcane_slam"] = Weapon("Arcane Slam", "2d6", DamageType.Bludgeoning, 1);
            Abilities["voidlash"] = Weapon("Void Lash", "1d8", DamageType.Necrotic, 2);
        }

        // ---- the time-displaced wizard ----
        public CharacterSheet BuildNaeve()
        {
            var wizard = _core.Classes.Find(c => c.className == "Wizard");
            var elf = _core.Races.Find(r => r.raceName == "High Elf") ?? _core.Races[0];
            var s = new CharacterSheet
            {
                displayName = "Naeve", classDef = wizard, raceDef = elf, level = 3, baseArmorClass = 12
            };
            s.abilities.Set(Ability.Strength, 8);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 12);
            s.abilities.Set(Ability.Intelligence, 17);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(Abilities["arcane_bolt"]);
            s.knownAbilities.Add(Abilities["shatterglass"]);
            s.knownAbilities.Add(Abilities["lightning_lance"]);
            s.equippedWeaponAbility = Abilities["arcane_bolt"];
            s.spellSlots.max[2] = 3;
            return s;
        }

        // ---- dialogue ----
        private void BuildDialogues()
        {
            // The arrival — auto-played when the era loads.
            var a = ScriptableObject.CreateInstance<DialogueGraph>();
            a.conversationId = "netheril.arrival";
            a.startNodeId = "0";
            a.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "—",
                text = "You fall out of your own century. Gold light. Sunlight on glass. A city — an entire " +
                       "city — floating in an impossible blue sky, held aloft by magic so total it has " +
                       "forgotten it is magic at all.",
                autoNextNodeId = "1",
                onEnter = new[] { new FlagClause { key = "netheril.arrived", op = FlagOp.SetTrue } }
            });
            a.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "—",
                text = "And then — all at once, everywhere, like a bell rung underwater — every spell in the " +
                       "world goes silent. The light dies. And the city, which flies by magic, begins, very " +
                       "slowly and then all at once, to FALL.",
                autoNextNodeId = "2"
            });
            a.nodes.Add(new DialogueNode
            {
                id = "2", speaker = "The Returned",
                text = "Karsus's Folly. The day the goddess of magic died. I'm standing inside the first " +
                       "apocalypse — and the floor is going out from under the world. Move. MOVE.",
            });
            ArrivalDialogue = a;

            // Naeve — recruit.
            var n = ScriptableObject.CreateInstance<DialogueGraph>();
            n.conversationId = "netheril.naeve";
            n.startNodeId = "0";
            n.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Naeve",
                text = "You pulled me from the rubble. Impossible — the ward-stones should have rendered me to " +
                       "ash. Unless..." + " you are not of this Now at all. You have the look of someone " +
                       "*falling through time.* I have read of people like you. In the margins of a forbidden " +
                       "proof. In a hand identical to my own — written centuries before I was born.",
                choices = new[]
                {
                    new DialogueChoice { text = "Whose hand?", nextNodeId = "hand" },
                    new DialogueChoice
                    {
                        text = "No time. The city's falling — come with me, NOW.", nextNodeId = "join",
                        effects = new[] { new FlagClause { key = "companion.naeve.recruited", op = FlagOp.SetTrue } }
                    },
                }
            });
            n.nodes.Add(new DialogueNode
            {
                id = "hand", speaker = "Naeve",
                text = "Yours, I rather think. Which should be impossible, and is the single most interesting " +
                       "thing that has ever happened to me, and we are both about to die discussing it. " +
                       "Get me off this dying enclave and I will tell you everything I have read about you. " +
                       "About what you are.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Deal. Stay close.", nextNodeId = "join",
                        effects = new[] { new FlagClause { key = "companion.naeve.recruited", op = FlagOp.SetTrue } }
                    },
                }
            });
            n.nodes.Add(new DialogueNode
            {
                id = "join", speaker = "Naeve",
                text = "Then I am yours, impossible one. Mind the gaps in the floor — gravity has only just " +
                       "remembered it has opinions. Let us not be where the marble is not."
            });
            NaeveRecruit = n;
        }

        // ---- small factories ----
        private AbilityDefinition Weapon(string name, string dice, DamageType type, int range)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = name; a.damageDice = dice; a.damageType = type; a.rangeTiles = range; a.isAttackRoll = true;
            return a;
        }

        private AbilityDefinition Spell(string name, string dice, DamageType type, int range, int slot, bool attack)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = name; a.damageDice = dice; a.damageType = type; a.rangeTiles = range;
            a.spellSlotLevel = slot; a.isAttackRoll = attack;
            return a;
        }
    }
}
