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

        [Tooltip("Natural 20 on the check routes here (the crit branch). Blank = no special crit.")]
        public string critNodeId;
        [Tooltip("Natural 1 on the check routes here (the fumble branch). Blank = no special fumble.")]
        public string fumbleNodeId;
    }

    /// <summary>
    /// One text variant of a node. The first variant whose <see cref="when"/> conditions all pass is
    /// shown; if none match, the node's plain <c>text</c> is used as the default. Lets a single line
    /// react to flags/disposition without authoring a separate node per state.
    /// </summary>
    [Serializable]
    public class DialogueVariant
    {
        [Tooltip("All clauses must pass for this variant to be chosen. Empty = always matches.")]
        public FlagClause[] when;
        [TextArea(2, 6)] public string text;
    }

    /// <summary>One candidate branch of a random-draw node (e.g. the Wayward Mile's caprice router).</summary>
    [Serializable]
    public class DrawOption
    {
        public string to;                 // node to route to if this option is drawn
        public string onceFlag;           // bool set true when drawn, so it won't repeat (blank = repeatable)
        public string needFlag;           // required bool prereq (blank = always eligible)
    }

    /// <summary>A single line of dialogue spoken by one party.</summary>
    [Serializable]
    public class DialogueNode
    {
        public string id;
        public string speaker = "NPC";
        [TextArea(2, 6)] public string text;

        [Tooltip("Optional reactive text. First variant whose conditions pass wins; else `text`.")]
        public DialogueVariant[] variants;

        [Tooltip("Effects applied the moment this node is shown (e.g. start a quest).")]
        public FlagClause[] onEnter;

        [Tooltip("Player choices. If empty, the conversation ends after this line.")]
        public DialogueChoice[] choices;

        [Tooltip("If no choices, auto-advance to this node. Blank = end conversation.")]
        public string autoNextNodeId;

        [Header("Random draw (optional — the Wayward Mile router)")]
        [Tooltip("If set, this node routes to a random eligible option instead of showing itself.")]
        public DrawOption[] draw;
        [Tooltip("GameFlags int key counting how many draws have fired (for the cap).")]
        public string drawCountKey;
        [Tooltip("Once this many draws have fired, route to drawElse instead.")]
        public int drawMax;
        [Tooltip("Where to route when the draw is exhausted or capped (falls back to autoNextNodeId).")]
        public string drawElse;

        [Tooltip("Choices are generated at runtime by game code (ResolveDynamicChoices); static choices/auto are the fallback.")]
        public bool isDynamic;
        [Tooltip("This node hands off to the party-banter system (CampfireBanter) instead of showing a line.")]
        public bool isBanter;
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
