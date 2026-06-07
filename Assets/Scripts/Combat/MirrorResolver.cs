using UnityEngine;
using SunderedCrown.Grid;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// The Mirror fight's special rule. The enemy named "The Last Returned" cannot be defeated by
    /// damage — it's *you*, and you can't out-fight yourself. The real fight is against the **Echoes**
    /// (twisted copies of your party). Once every Echo has fallen, the Last Returned *kneels* — it
    /// yields — and the encounter resolves. (In the full game this is where the player lowers their
    /// weapon and recovers their true name; here we resolve it to victory with a remembered beat.)
    ///
    /// Added by EncounterBuilder when mirrorMode = true.
    /// </summary>
    public class MirrorResolver : MonoBehaviour
    {
        private const string MirrorName = "The Last Returned";
        private TurnManager _turns;
        private bool _kneeled;

        void Start()
        {
            _turns = TurnManager.Instance;
            if (_turns != null) _turns.OnTurnEnded += OnTurnEnded;
        }

        void OnDestroy()
        {
            if (_turns != null) _turns.OnTurnEnded -= OnTurnEnded;
        }

        private void OnTurnEnded(GridUnit _)
        {
            if (_kneeled || _turns == null) return;

            GridUnit mirror = null;
            bool otherEnemyAlive = false;
            foreach (var u in _turns.TurnOrder)
            {
                if (u.faction != Faction.Enemy || !u.Sheet.IsAlive) continue;
                if (u.Sheet.displayName == MirrorName) mirror = u;
                else otherEnemyAlive = true;
            }

            // Once the Echoes are gone, the Last Returned lowers its guard.
            if (mirror != null && !otherEnemyAlive)
            {
                _kneeled = true;
                _turns.Log("The Echoes fall silent. The Last Returned lowers its guard — and its weapon.");
                _turns.Log("\"...You found a way. After all this. You actually—\" It kneels. The fight is over.");
                // Yield: the mirror stops resisting, so the encounter resolves as a victory.
                mirror.Sheet.TakeDamage(mirror.Sheet.currentHitPoints);
            }
        }
    }
}
