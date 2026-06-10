using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Dialogue;
using SunderedCrown.Grid;

namespace SunderedCrown.World
{
    /// <summary>
    /// Drives real-time hub/exploration: click a tile to walk the party leader there
    /// (A* pathing, no turn budget), click an interactable (NPC / door / point of
    /// interest) to walk over and trigger it, or press E to use the nearest one in
    /// range. Movement is blocked while a conversation is open.
    /// </summary>
    public class ExplorationController : MonoBehaviour
    {
        public static ExplorationController Active { get; private set; }

        public GridUnit Leader;
        public Camera worldCamera;
        public float interactRange = 1.6f;

        public Interactable Nearby { get; private set; } // for the HUD prompt

        private GridSystem _grid;
        private readonly List<Interactable> _interactables = new List<Interactable>();
        private Coroutine _moving;

        void Awake() => Active = this;
        void OnDestroy() { if (Active == this) Active = null; }

        void Start()
        {
            _grid = GridSystem.Instance;
            if (worldCamera == null) worldCamera = Camera.main;
            _interactables.AddRange(FindObjectsByType<Interactable>());

            // Skill checks in hub dialogue use the leader's modifiers.
            if (DialogueRunner.Instance != null && Leader != null)
                DialogueRunner.Instance.ResolvePlayerSpeaker = () => Leader.Sheet;
        }

        void Update()
        {
            if (_grid == null || Leader == null) return;
            if (DialogueRunner.Instance != null && DialogueRunner.Instance.IsActive) return; // talking
            if (SunderedCrown.UI.RiddleVaultUI.AnyOpen) return; // solving a riddle (modal)

            Nearby = FindNearest();

            if (Input.GetKeyDown(KeyCode.E) && Nearby != null && InRange(Nearby))
                Trigger(Nearby);

            if (Input.GetMouseButtonDown(0))
                HandleClick();
        }

        private void HandleClick()
        {
            Vector3 world = worldCamera.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;
            var cell = _grid.GetCell(_grid.WorldToGrid(world));
            if (cell == null) return;

            // Did we click an interactable's tile?
            var target = _interactables.Find(i => i.Cell == cell);
            if (target != null) { StartMove(MoveToThenTrigger(target)); return; }

            if (cell.IsFree) StartMove(MoveTo(cell));
        }

        private void StartMove(IEnumerator routine)
        {
            if (_moving != null) StopCoroutine(_moving);
            _moving = StartCoroutine(routine);
        }

        private IEnumerator MoveTo(GridCell cell)
        {
            var path = Pathfinding.FindPath(_grid, Leader.Cell, cell);
            if (path != null) yield return Leader.FollowPath(path);
        }

        private IEnumerator MoveToThenTrigger(Interactable it)
        {
            // Walk to the nearest walkable tile beside the interactable, then trigger.
            GridCell spot = null;
            foreach (var n in _grid.Neighbours(it.Cell))
                if (n.IsFree) { spot = n; break; }

            if (spot != null && spot != Leader.Cell)
            {
                var path = Pathfinding.FindPath(_grid, Leader.Cell, spot);
                if (path != null) yield return Leader.FollowPath(path);
            }
            if (InRange(it)) Trigger(it);
        }

        private Interactable FindNearest()
        {
            Interactable best = null;
            float bestSqr = float.MaxValue;
            foreach (var it in _interactables)
            {
                if (it == null || it.Cell == null) continue;
                float d = (it.transform.position - Leader.transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = it; }
            }
            return best;
        }

        private bool InRange(Interactable it) =>
            it != null && it.Cell != null &&
            GridSystem.ManhattanDistance(Leader.Cell, it.Cell) <= 1;

        private void Trigger(Interactable it)
        {
            switch (it.kind)
            {
                case InteractionKind.Talk:
                    if (it.dialogue != null && DialogueRunner.Instance != null)
                    {
                        DialogueRunner.Instance.ResolvePlayerSpeaker = () => Leader.Sheet;
                        DialogueRunner.Instance.Begin(it.dialogue);
                    }
                    break;
                case InteractionKind.Exit:
                    it.onExit?.Invoke();
                    break;
                case InteractionKind.Examine:
                    ExamineText = it.examineText;
                    ExamineUntil = Time.time + 4f;
                    it.onExamined?.Invoke();
                    break;
                case InteractionKind.Container:
                    LootContainer(it);
                    break;
            }
        }

        private void LootContainer(Interactable it)
        {
            if (it.looted || (!string.IsNullOrEmpty(it.lootFlag) && Core.GameFlags.Current.GetBool(it.lootFlag)))
            { Flash($"{it.label} is empty."); return; }
            it.looted = true;
            if (!string.IsNullOrEmpty(it.lootFlag)) Core.GameFlags.Current.SetBool(it.lootFlag, true);
            var party = Characters.Party.Instance;
            string msg = $"Looted {it.label}: ";
            if (party != null)
            {
                if (it.gold > 0) { party.inventory.AddGold(it.gold); msg += $"{it.gold} gold "; }
                foreach (var item in it.contents)
                    if (item != null) { party.inventory.Add(item); msg += $"· {item.displayName} "; }
            }
            // Dim the looted container so it reads as opened.
            var rend = it.GetComponent<Renderer>();
            if (rend != null) rend.material.color = new Color(0.25f, 0.25f, 0.25f);
            Flash(msg);
        }

        private void Flash(string text) { ExamineText = text; ExamineUntil = Time.time + 4f; }

        // Surfaced to the HUD for examine flavor text.
        public string ExamineText { get; private set; }
        public float ExamineUntil { get; private set; }
    }
}
