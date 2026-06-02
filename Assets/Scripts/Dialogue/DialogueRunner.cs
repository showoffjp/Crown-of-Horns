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
                int roll = Dice.D20() + mod;
                bool success = roll >= choice.checkDC;
                OnSkillCheck?.Invoke(choice.checkAbility, choice.checkDC, roll, success);
                if (!success && !string.IsNullOrEmpty(choice.failNodeId))
                    next = choice.failNodeId;
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
