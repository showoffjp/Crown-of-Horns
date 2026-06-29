using System;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Stats;

namespace SunderedCrown.Dialogue
{
    /// <summary>
    /// Walks a DialogueGraph: evaluates conditions against GameFlags, applies
    /// effects, runs skill checks, and raises events the UI binds to. The UI
    /// never needs to know the graph format — it just renders what it's handed.
    /// </summary>
    public class DialogueRunner : MonoBehaviour
    {
        public static DialogueRunner Instance { get; private set; }

        public bool IsActive { get; private set; }
        private DialogueGraph _graph;
        private DialogueNode _current;

        /// <summary>The text the current node should display, after variant resolution. The UI should
        /// prefer this over node.text so reactive variants are shown. Falls back to node.text.</summary>
        public string CurrentText { get; private set; }

        /// <summary>Fired with the current line + the list of selectable choices.</summary>
        public event Action<DialogueNode, List<DialogueChoice>> OnNodeShown;
        public event Action OnConversationEnded;
        /// <summary>Fired when a skill check is rolled: (ability, dc, roll, success).</summary>
        public event Action<Ability, int, int, bool> OnSkillCheck;

        // Provide the speaking character's sheet so skill checks use real modifiers.
        public Func<Characters.CharacterSheet> ResolvePlayerSpeaker;

        void Awake() => Instance = this;

        public void Begin(DialogueGraph graph)
        {
            if (graph == null) return;
            _graph = graph;
            IsActive = true;
            GoTo(graph.startNodeId);
        }

        private void GoTo(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) { End(); return; }
            _current = _graph.GetNode(nodeId);
            if (_current == null) { End(); return; }

            CurrentText = ResolveText(_current);
            ApplyClauses(_current.onEnter);

            // Build the list of choices whose conditions currently pass.
            var available = new List<DialogueChoice>();
            if (_current.choices != null)
                foreach (var c in _current.choices)
                    if (ConditionsPass(c.conditions)) available.Add(c);

            OnNodeShown?.Invoke(_current, available);

            // No choices and no explicit auto-next → end. Auto-advance if specified.
            if (available.Count == 0 && (_current.choices == null || _current.choices.Length == 0))
            {
                if (!string.IsNullOrEmpty(_current.autoNextNodeId))
                    GoTo(_current.autoNextNodeId);
                // else: wait for the UI to call Continue() so the player can read it.
            }
        }

        /// <summary>Called by the UI when the player selects a choice index from OnNodeShown.</summary>
        public void Choose(DialogueChoice choice)
        {
            if (choice == null) { End(); return; }

            string next = choice.nextNodeId;

            if (choice.checkDC > 0)
            {
                var speaker = ResolvePlayerSpeaker?.Invoke();
                int mod = speaker != null ? speaker.Modifier(choice.checkAbility) : 0;
                int natural = Dice.D20();           // the raw face — drives crit/fumble branches
                int roll = natural + mod;

                if (natural == 20 && !string.IsNullOrEmpty(choice.critNodeId))
                {
                    OnSkillCheck?.Invoke(choice.checkAbility, choice.checkDC, roll, true);
                    next = choice.critNodeId;       // nat 20 → the crit branch, regardless of DC
                }
                else if (natural == 1 && !string.IsNullOrEmpty(choice.fumbleNodeId))
                {
                    OnSkillCheck?.Invoke(choice.checkAbility, choice.checkDC, roll, false);
                    next = choice.fumbleNodeId;     // nat 1 → the fumble branch, regardless of DC
                }
                else
                {
                    bool success = roll >= choice.checkDC;
                    OnSkillCheck?.Invoke(choice.checkAbility, choice.checkDC, roll, success);
                    if (!success && !string.IsNullOrEmpty(choice.failNodeId))
                        next = choice.failNodeId;
                }
            }

            ApplyClauses(choice.effects);
            GoTo(next);
        }

        /// <summary>For lines that just need a "Continue" with no real choices.</summary>
        public void Continue()
        {
            if (_current != null && !string.IsNullOrEmpty(_current.autoNextNodeId))
                GoTo(_current.autoNextNodeId);
            else
                End();
        }

        private void End()
        {
            IsActive = false;
            _graph = null;
            _current = null;
            OnConversationEnded?.Invoke();
        }

        // ---- Variant resolution ------------------------------------------

        /// <summary>The text to show for a node: the first variant whose conditions all pass, else the
        /// node's plain text. Variants let one line react to flags/disposition without extra nodes.</summary>
        private string ResolveText(DialogueNode node)
        {
            if (node.variants != null)
                foreach (var v in node.variants)
                    if (ConditionsPass(v.when)) return v.text;
            return node.text;
        }

        // ---- Flag evaluation ---------------------------------------------

        private bool ConditionsPass(FlagClause[] conditions)
        {
            if (conditions == null) return true;
            var f = GameFlags.Current;
            foreach (var c in conditions)
            {
                switch (c.op)
                {
                    case FlagOp.RequireBoolTrue:    if (!f.GetBool(c.key)) return false; break;
                    case FlagOp.RequireBoolFalse:   if (f.GetBool(c.key)) return false; break;
                    case FlagOp.RequireIntAtLeast:  if (f.GetInt(c.key) < c.amount) return false; break;
                }
            }
            return true;
        }

        private void ApplyClauses(FlagClause[] effects)
        {
            if (effects == null) return;
            var f = GameFlags.Current;
            foreach (var e in effects)
            {
                switch (e.op)
                {
                    case FlagOp.SetTrue:  f.SetBool(e.key, true); break;
                    case FlagOp.SetFalse: f.SetBool(e.key, false); break;
                    case FlagOp.AddInt:   f.AddInt(e.key, e.amount); break;
                }
            }
        }
    }
}
