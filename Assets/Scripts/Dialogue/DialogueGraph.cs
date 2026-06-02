using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunderedCrown.Dialogue
{
    public enum FlagOp { SetTrue, SetFalse, AddInt, RequireBoolTrue, RequireBoolFalse, RequireIntAtLeast }

    /// <summary>A read (condition) or write (effect) against GameFlags.</summary>
    [Serializable]
    public class FlagClause
    {
        public string key;
        public FlagOp op;
        public int amount; // used by AddInt / RequireIntAtLeast
    }

    /// <summary>One branch the player can pick at a node.</summary>
    [Serializable]
    public class DialogueChoice
    {
        [TextArea(1, 3)] public string text = "...";
        public string nextNodeId;

        [Tooltip("All clauses must pass for this choice to be shown.")]
        public FlagClause[] conditions;

        [Tooltip("Applied when the player picks this choice.")]
        public FlagClause[] effects;

        [Tooltip("Optional skill check: ability + DC. Leave checkDC at 0 to skip.")]
        public Stats.Ability checkAbility = Stats.Ability.Charisma;
        public int checkDC = 0;
        [Tooltip("Node to jump to if the skill check FAILS (falls back to nextNodeId if blank).")]
        public string failNodeId;
    }

    /// <summary>A single line of dialogue spoken by one party.</summary>
    [Serializable]
    public class DialogueNode
    {
        public string id;
        public string speaker = "NPC";
        [TextArea(2, 6)] public string text;

        [Tooltip("Effects applied the moment this node is shown (e.g. start a quest).")]
        public FlagClause[] onEnter;

        [Tooltip("Player choices. If empty, the conversation ends after this line.")]
        public DialogueChoice[] choices;

        [Tooltip("If no choices, auto-advance to this node. Blank = end conversation.")]
        public string autoNextNodeId;
    }

    /// <summary>
    /// A whole conversation, authored as a ScriptableObject asset. Each NPC or
    /// scripted scene points at one of these. Designers build branching dialogue
    /// entirely in the inspector — no code per conversation.
    ///
    /// Create via: Assets > Create > Sundered Crown > Dialogue Graph
    /// </summary>
    [CreateAssetMenu(menuName = "Sundered Crown/Dialogue Graph", fileName = "NewDialogue")]
    public class DialogueGraph : ScriptableObject
    {
        public string conversationId;
        public string startNodeId;
        public List<DialogueNode> nodes = new List<DialogueNode>();

        private Dictionary<string, DialogueNode> _index;

        public DialogueNode GetNode(string id)
        {
            if (_index == null)
            {
                _index = new Dictionary<string, DialogueNode>();
                foreach (var n in nodes)
                    if (!string.IsNullOrEmpty(n.id)) _index[n.id] = n;
            }
            return (id != null && _index.TryGetValue(id, out var node)) ? node : null;
        }
    }
}
