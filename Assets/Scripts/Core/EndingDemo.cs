using UnityEngine;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK ENDINGS PREVIEW. Drop on an empty GameObject and press Play to open the Court of the
    /// Dead with a sample playthrough's flags set, so all six endings are available to read — each with
    /// its prose and epilogue slides (a lost companion, a spared Verdict, the Lady's witness, Aldric's
    /// fate). The real finale reads *your* flags; this just unlocks everything for a tour.
    /// </summary>
    public class EndingDemo : MonoBehaviour
    {
        void Start()
        {
            var f = GameFlags.Current;
            // Unlock the deeper endings and seed some epilogue flavour for the preview.
            f.SetBool("readers_boon", true);
            f.SetBool("crownwars.verdict_spared", true);
            f.SetInt("faction.kelemvor.reputation", 5);
            f.SetBool("aldric.provisional", true);
            f.SetBool("companion.roen.recruited", true);
            f.SetBool("companion.varra.recruited", true);
            f.SetBool("companion.varra.lost", true);     // show a permanent-loss slide
            f.SetBool("companion.naeve.recruited", true);
            f.SetBool("companion.ilfaeril.recruited", true);
            f.SetBool("companion.maerin.recruited", true);

            var go = new GameObject("Finale");
            var es = go.AddComponent<SunderedCrown.UI.EndingScreen>();
            es.onLeave = () => { Destroy(go); Debug.Log("[EndingDemo] Fin. Pick another to compare."); };

            Debug.Log("[EndingDemo] The Court of the Dead. All six endings unlocked for preview — choose one to " +
                      "read its prose + epilogue, then 'Reconsider' to try another.");
        }
    }
}
