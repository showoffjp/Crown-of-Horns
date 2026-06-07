using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Dialogue;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Content for the late eras — the TIME OF TROUBLES (1358 DR) and the SPELLPLAGUE (1385 DR).
    /// Provides their arrival framings and a couple of era-flavoured enemy abilities. These eras have
    /// no new companion; they're lore + combat (the Crown-of-Horns reveal; the world coming apart),
    /// and they ride on the reusable SimpleEra builder.
    /// </summary>
    public class LateEraContent
    {
        public DialogueGraph TimeOfTroublesArrival { get; private set; }
        public DialogueGraph SpellplagueArrival { get; private set; }

        public AbilityDefinition AvatarTouch { get; private set; }   // divine static
        public AbilityDefinition BlueFire { get; private set; }      // Spellplague

        public LateEraContent()
        {
            AvatarTouch = Weapon("Avatar's Touch", "2d8", DamageType.Radiant, 1);
            BlueFire = Spell("Blue Fire", "3d6", DamageType.Force, range: 8, slot: 0, attack: true);

            TimeOfTroublesArrival = Graph("toot.arrival",
                ("—", "You fall to 1358 DR — the Time of Troubles. The gods have been cast down to walk Faerûn as " +
                      "mortals, and they can *die.* Above Waterdeep the sky is the wrong colour, and something vast " +
                      "and grey is dying in it.", "1", "act4.crown_is_myrkul"),
                ("The Returned", "There. The skull. They're forging it into a crown — the Crown of Horns, from the " +
                      "death of the god Myrkul. The relic Aldric carries. The relic that has been *whispering* to me. " +
                      "It was never a tool. It was a god, the whole time, waiting.", null, null));

            SpellplagueArrival = Graph("spellplague.arrival",
                ("—", "1385 DR. The goddess of magic is murdered a second time, and the Weave does not fail — it " +
                      "*shatters.* Blue fire runs across the world like spilled ink; two worlds bleed together; and " +
                      "here, in the wound, cause no longer reliably precedes effect.", "1", "spellplague.arrived"),
                ("The Returned", "Reality is running like wet ink. Things happen before I decide them. This is where " +
                      "the Unmade comes closest to winning — where one pulled thread could unravel everything. Hold on " +
                      "to *now.* Whatever 'now' still means here.", null, null));
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

        // (speaker, text, autoNextId-or-null, onEnterFlag-or-null) per node, ids "0","1",...
        private DialogueGraph Graph(string id, params (string speaker, string text, string next, string flag)[] nodes)
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = id; g.startNodeId = "0";
            for (int i = 0; i < nodes.Length; i++)
            {
                var n = nodes[i];
                g.nodes.Add(new DialogueNode
                {
                    id = i.ToString(),
                    speaker = n.speaker,
                    text = n.text,
                    autoNextNodeId = n.next,
                    onEnter = string.IsNullOrEmpty(n.flag) ? null
                        : new[] { new FlagClause { key = n.flag, op = FlagOp.SetTrue } }
                });
            }
            return g;
        }
    }
}
