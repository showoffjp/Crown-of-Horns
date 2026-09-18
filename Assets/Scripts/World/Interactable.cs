using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Dialogue;
using SunderedCrown.Grid;
using SunderedCrown.Items;

namespace SunderedCrown.World
{
    public enum InteractionKind { Talk, Exit, Examine, Container }

    /// <summary>
    /// A thing in the exploration world the party can interact with: an NPC to talk to,
    /// an exit/door that transitions the game, or a point of interest to examine.
    /// Sits on a tile; the ExplorationController routes the leader to it and triggers it.
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        public InteractionKind kind = InteractionKind.Talk;
        public string label = "Interact";
        public Vector2Int coord;

        [Header("Talk")]
        public DialogueGraph dialogue;

        [Header("Examine")]
        [TextArea(2, 4)] public string examineText;
        [Tooltip("Optional callback fired when this is examined (assigned in code).")]
        public System.Action onExamined;

        [Header("Exit (assigned in code)")]
        public System.Action onExit;

        [Header("Container")]
        public List<ItemDefinition> contents = new List<ItemDefinition>();
        public int gold = 0;
        public bool looted = false;
        [Tooltip("Optional GameFlags key set true when looted, so the chest stays empty across rebuilds.")]
        public string lootFlag;

        public GridCell Cell { get; private set; }

        /// <summary>Snap to the grid and block the tile so the party stops beside it.</summary>
        public void Place(GridSystem grid)
        {
            Cell = grid.GetCell(coord);
            if (Cell != null) Cell.walkable = false; // route the party to an adjacent tile
            transform.position = grid.GridToWorld(coord.x, coord.y);
        }

        void Start()
        {
            // A container whose flag is already set was emptied on an earlier visit.
            // Restoring that here is what makes looting stick: every scene rebuilds
            // its markers from scratch when the party walks back in, so a chest that
            // only ever recorded `looted` in memory came back full each time.
            if (kind == InteractionKind.Container && !string.IsNullOrEmpty(lootFlag) &&
                Core.GameFlags.Current != null && Core.GameFlags.Current.GetBool(lootFlag))
                looted = true;

            // Art is chosen here, not in Place: the marker factories call Place before
            // they have set kind/lootFlag, so art picked there could not tell a chest
            // from a door, nor an emptied chest from a full one.
            SunderedCrown.Rendering.MarkerArt.Apply(gameObject, label, kind, looted);
        }

        /// <summary>Re-pick this marker's art — called when its state changes, e.g. a
        /// chest that has just been emptied swapping to its open lid.</summary>
        public void RefreshArt() =>
            SunderedCrown.Rendering.MarkerArt.Apply(gameObject, label, kind, looted);
    }
}
