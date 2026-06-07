using UnityEngine;
using SunderedCrown.Combat;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK COMBAT-BALANCE CANARY. Drop on an empty GameObject and press Play: runs many seeded melee
    /// duels through the real <see cref="AttackResolver"/> math and prints a win-rate verdict to the Console
    /// (and on-screen). Use it after touching the resolver, modifiers, crits, or difficulty scaling to spot
    /// a wild skew the compiler can't catch. Deterministic — same numbers every run.
    /// </summary>
    public class CombatBalanceDemo : MonoBehaviour
    {
        private string _summary = "Running…";

        void Start()
        {
            _summary = CombatBalance.Report();
            Debug.Log("[CombatBalance] " + _summary);
        }

        void OnGUI()
        {
            GUI.Label(new Rect(12, 12, Screen.width - 24, 30), "<b>" + _summary + "</b>");
        }
    }
}
