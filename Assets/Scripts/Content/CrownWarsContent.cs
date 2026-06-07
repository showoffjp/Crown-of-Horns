using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Era content for the CROWN WARS (~-10,000 DR): the elven civil wars where the Law of the
    /// Faithless was first *voted* into being. Provides elven martial/holy abilities, the elven
    /// damned (revenants/shades), the time-displaced companion Ilfaeril, the era framing &amp;
    /// recruit dialogue, and the signature moral scene — *The Verdict*, where you can argue a
    /// soul-damnation **down**, proving the Wall was always a decision, not a law.
    /// </summary>
    public class CrownWarsContent
    {
        private readonly SwordCoastContent _core;

        public readonly Dictionary<string, AbilityDefinition> Abilities = new Dictionary<string, AbilityDefinition>();
        public DialogueGraph ArrivalDialogue { get; private set; }
        public DialogueGraph IlfaerilRecruit { get; private set; }
        public DialogueGraph VerdictDialogue { get; private set; }

        public CrownWarsContent(SwordCoastContent core)
        {
            _core = core;
            BuildAbilities();
            BuildDialogues();
        }

        private void BuildAbilities()
        {
            Abilities["elven_blade"] = Weapon("Elven Blade", "1d8", DamageType.Slashing, 1);
            Abilities["moonbow"] = Weapon("Moonbow", "1d8", DamageType.Piercing, 10);

            var smite = Spell("Radiant Smite", "2d8", DamageType.Radiant, range: 1, slot: 1, attack: true);
            Abilities["radiant_smite"] = smite;

            Abilities["grave_claw"] = Weapon("Grave Claw", "1d8", DamageType.Necrotic, 1);

            var dread = Spell("Dread Gaze", "", DamageType.Psychic, range: 5, slot: 0, attack: false);
            dread.saveAbility = Ability.Wisdom; dread.damageDice = "";
            dread.appliedEffect = _core.Effects.TryGetValue("frightened", out var fr) ? fr : null;
            Abilities["dread_gaze"] = dread;
        }

        public CharacterSheet BuildIlfaeril()
        {
            var fighter = _core.Classes.Find(c => c.className == "Fighter");
            var elf = _core.Races.Find(r => r.raceName == "High Elf") ?? _core.Races[0];
            var s = new CharacterSheet
            {
                displayName = "Ilfaeril", classDef = fighter, raceDef = elf, level = 4, baseArmorClass = 16
            };
            s.abilities.Set(Ability.Strength, 15);
            s.abilities.Set(Ability.Dexterity, 13);
            s.abilities.Set(Ability.Constitution, 14);
            s.abilities.Set(Ability.Charisma, 15);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(Abilities["elven_blade"]);
            s.knownAbilities.Add(Abilities["radiant_smite"]);
            s.knownAbilities.Add(Abilities["moonbow"]);
            s.equippedWeaponAbility = Abilities["elven_blade"];
            s.spellSlots.max[1] = 3;
            return s;
        }

        private void BuildDialogues()
        {
            // Arrival.
            var a = ScriptableObject.CreateInstance<DialogueGraph>();
            a.conversationId = "crownwars.arrival";
            a.startNodeId = "0";
            a.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "—",
                text = "You fall further back than you have ever fallen — ten thousand years. Silver spires. " +
                       "Elven banners. A high court mid-session, beautiful and cold. And the matter before the " +
                       "court is not land, nor crowns. It is the *souls* of a defeated people.",
                autoNextNodeId = "1",
                onEnter = new[] { new FlagClause { key = "crownwars.arrived", op = FlagOp.SetTrue } }
            });
            a.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "The Returned",
                text = "This is it. The first verdict. The afternoon they *decided* that the godless dead deserve " +
                       "to be unmade — before it hardened into the law of the Wall. I'm standing at the source.",
            });
            ArrivalDialogue = a;

            // Ilfaeril — recruit.
            var i = ScriptableObject.CreateInstance<DialogueGraph>();
            i.conversationId = "crownwars.ilfaeril";
            i.startNodeId = "0";
            i.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "Ilfaeril",
                text = "You carry the chill of the Wall on you, traveller — and the look of someone unstuck from " +
                       "time. I am Ilfaeril. I sit in that court. And I am about to do a thing I will spend ten " +
                       "thousand years trying to undo. If you have come to stop it... I would not stop you.",
                choices = new[]
                {
                    new DialogueChoice { text = "What thing?", nextNodeId = "thing" },
                    new DialogueChoice
                    {
                        text = "Then help me. Walk with me.", nextNodeId = "join",
                        effects = new[] { new FlagClause { key = "companion.ilfaeril.recruited", op = FlagOp.SetTrue } }
                    },
                }
            });
            i.nodes.Add(new DialogueNode
            {
                id = "thing", speaker = "Ilfaeril",
                text = "I am about to raise my hand to damn an enemy people's souls to nothing — so that not even " +
                       "their memory has anywhere to rest. The court calls it justice. I will call it, in time, " +
                       "the only sin I could never bury. Come. Perhaps a soul from the future can do what my " +
                       "conscience cannot.",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "Then let's change the vote. With me.", nextNodeId = "join",
                        effects = new[] { new FlagClause { key = "companion.ilfaeril.recruited", op = FlagOp.SetTrue } }
                    },
                }
            });
            i.nodes.Add(new DialogueNode
            {
                id = "join", speaker = "Ilfaeril",
                text = "Then my sword and my long, long regret are yours. Let us see if the past can be argued with."
            });
            IlfaerilRecruit = i;

            // The Verdict — the moral hazard.
            var v = ScriptableObject.CreateInstance<DialogueGraph>();
            v.conversationId = "crownwars.verdict";
            v.startNodeId = "0";
            v.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "High Lord Aelryth",
                text = "You are not of this court, outsider, yet Ilfaeril vouches for you, so I will hear you. The " +
                       "matter stands: shall the godless dead of our fallen enemies be *unmade* — denied even a " +
                       "resting-place — that our victory be total? Speak. Why should mercy stay my hand?",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "[PERSUADE DC 15] A law of vengeance outlives the vengeful. You damn your own descendants.",
                        nextNodeId = "spared", checkAbility = Ability.Charisma, checkDC = 15, failNodeId = "passed"
                    },
                    new DialogueChoice
                    {
                        text = "[INSIGHT DC 15] You do not want this. You want to be *told* you mustn't.",
                        nextNodeId = "spared", checkAbility = Ability.Wisdom, checkDC = 15, failNodeId = "passed"
                    },
                    new DialogueChoice { text = "Their souls are yours to take. Take them.", nextNodeId = "passed" },
                    new DialogueChoice { text = "(Say nothing. Let the court decide.)", nextNodeId = "passed" },
                }
            });
            v.nodes.Add(new DialogueNode
            {
                id = "spared", speaker = "High Lord Aelryth",
                text = "...You are right. Gods help me, you are right, and I have wanted someone to say it since the " +
                       "first banner fell. The motion is *withdrawn.* Let their dead rest. Let this one court, at " +
                       "least, be remembered for the cruelty it did NOT do.",
                onEnter = new[]
                {
                    new FlagClause { key = "crownwars.verdict_spared", op = FlagOp.SetTrue },
                    new FlagClause { key = "companion.ilfaeril.approval", op = FlagOp.AddInt, amount = 20 },
                }
            });
            v.nodes.Add(new DialogueNode
            {
                id = "passed", speaker = "High Lord Aelryth",
                text = "Then it is done. Their dead are unmade, root and memory. ...The chamber feels colder than " +
                       "I expected. History will not record this hour. History will only inherit it.",
                onEnter = new[] { new FlagClause { key = "crownwars.verdict_passed", op = FlagOp.SetTrue } }
            });
            VerdictDialogue = v;
        }

        // ---- factories ----
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
