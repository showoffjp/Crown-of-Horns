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
    }
}
