using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Content for the FUGUE PLANE / the Wall of the Faithless (Act II): the companion Maerin (pulled
    /// from the Wall), the descent framing, and the pull-her-free conversation that triggers the
    /// Breach — the game's first permanent loss. The mechanical loss is resolved by the director's
    /// authored-fate system (see CampaignBootstrap.DoBreach); the dialogue here sets the flags.
    /// </summary>
    public class FugueContent
    {
        private readonly SwordCoastContent _core;

        public DialogueGraph ArrivalDialogue { get; private set; }
        public DialogueGraph MaerinDialogue { get; private set; }

        public FugueContent(SwordCoastContent core)
        {
            _core = core;
            BuildDialogues();
        }

        public CharacterSheet BuildMaerin()
        {
            var ranger = _core.Classes.Find(c => c.className == "Ranger");
            var s = new CharacterSheet
            {
                displayName = "Maerin", classDef = ranger, raceDef = _core.Races[0], level = 3, baseArmorClass = 13
            };
            s.abilities.Set(Ability.Dexterity, 16);
            s.abilities.Set(Ability.Constitution, 12);
            s.abilities.Set(Ability.Wisdom, 14);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(_core.Abilities["shortbow"]);
            s.knownAbilities.Add(_core.Abilities["dagger"]);
            s.equippedWeaponAbility = _core.Abilities["shortbow"];
            return s;
        }

        private void BuildDialogues()
        {
            var a = ScriptableObject.CreateInstance<DialogueGraph>();
            a.conversationId = "fugue.arrival";
            a.startNodeId = "0";
            a.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "—",
                text = "The grey is not a colour. It is the absence of the argument for colour. You are walking — " +
                       "though you don't remember choosing to — in a long grey river of the recently dead, toward " +
                       "a city that never gets closer. And then you see the Wall.",
                autoNextNodeId = "1",
                onEnter = new[] { new FlagClause { key = "fugue.arrived", op = FlagOp.SetTrue } }
            });
            a.nodes.Add(new DialogueNode
            {
                id = "1", speaker = "The Returned",
                text = "It's made of *people.* Faces in the mortar. Mouths still moving — small fragments of names, " +
                       "of weather, of *someone please.* The godless dead, dissolving one unremembered syllable at a " +
                       "time. ...And near the end of the curve: a niche. Empty. The only empty one. With my name " +
                       "half-carved above it.",
            });
            ArrivalDialogue = a;

            var m = ScriptableObject.CreateInstance<DialogueGraph>();
            m.conversationId = "fugue.maerin";
            m.startNodeId = "0";
            m.nodes.Add(new DialogueNode
            {
                id = "0", speaker = "A girl in the Wall",
                text = "You. You're the Returned — you can *reach* in here, I can feel it. My father's burning the " +
                       "whole world to pull me out of this. Don't you do it too. Whatever it costs — and it costs " +
                       "something, the Wall never gives without taking — I am not worth a—",
                choices = new[]
                {
                    new DialogueChoice
                    {
                        text = "I'm pulling you free. Hold on to me.", nextNodeId = "pull",
                        effects = new[]
                        {
                            new FlagClause { key = "companion.maerin.recruited", op = FlagOp.SetTrue },
                            new FlagClause { key = "fugue.pull_maerin", op = FlagOp.SetTrue },
                        }
                    },
                    new DialogueChoice { text = "...You're right. I won't pay that price.", nextNodeId = "leave",
                        effects = new[] { new FlagClause { key = "fugue.left_maerin", op = FlagOp.SetTrue } } },
                }
            });
            m.nodes.Add(new DialogueNode
            {
                id = "pull", speaker = "Maerin",
                text = "—No. NO. The Wall — it's screaming, it won't give without taking, it's *taking someone* — I " +
                       "can feel the grey close on one of them — I'm sorry, I'm sorry, I didn't ask for this—\n\n" +
                       "(She comes free in your arms, whole and luminous and fading and *furious.* And there are " +
                       "fewer of you than there were a moment ago. There will be fewer of you forever.)"
            });
            m.nodes.Add(new DialogueNode
            {
                id = "leave", speaker = "Maerin",
                text = "...Good. You're learning. Most don't. They look at a girl in a wall and stop doing arithmetic. " +
                       "Go. Before the grey decides it likes the look of you, too."
            });
            MaerinDialogue = m;
        }
    }
}
