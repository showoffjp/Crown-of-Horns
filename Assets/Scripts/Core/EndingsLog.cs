using UnityEngine;

namespace SunderedCrown.Core
{
    /// <summary>
    /// LEGACY / NEW-GAME+ MEMORY. A tiny PlayerPrefs-backed record of which endings the player has reached
    /// *across all playthroughs* — survives saves, deletes, and fresh starts. The finale records its choice
    /// here; the main menu surfaces the tally ("Endings discovered: N/6") so completionists can see what's
    /// left without a guide. Pure static; no Unity object needed. The Ending enum is the source of truth for
    /// Total, so adding an ending widens the log automatically.
    /// </summary>
    public static class EndingsLog
    {
        private const string K_Mask  = "legacy.endings.mask";  // bitmask of Ending values reached
        private const string K_Runs  = "legacy.runs.finished";  // count of campaigns carried to the Court
        private const string K_Golden = "legacy.golden.reached"; // 1 once any golden ending is seen

        public static int Total => System.Enum.GetValues(typeof(Ending)).Length;

        public static int Mask
        {
            get => PlayerPrefs.GetInt(K_Mask, 0);
            private set { PlayerPrefs.SetInt(K_Mask, value); PlayerPrefs.Save(); }
        }

        public static bool IsSeen(Ending e) => (Mask & (1 << (int)e)) != 0;

        public static int SeenCount
        {
            get { int m = Mask, n = 0; while (m != 0) { n += m & 1; m >>= 1; } return n; }
        }

        public static int RunsFinished => PlayerPrefs.GetInt(K_Runs, 0);
        public static bool GoldenReached => PlayerPrefs.GetInt(K_Golden, 0) == 1;

        /// <summary>Call once when an ending is chosen at the Court. Idempotent per ending bit, but always
        /// counts the finished run.</summary>
        public static void Record(Ending e)
        {
            Mask |= (1 << (int)e);
            PlayerPrefs.SetInt(K_Runs, RunsFinished + 1);
            if (EndingResolver.IsGolden(e)) PlayerPrefs.SetInt(K_Golden, 1);
            PlayerPrefs.Save();
        }

        /// <summary>A one-line legacy summary for the title screen, or null if nothing's been finished yet.</summary>
        public static string MenuLine()
        {
            if (RunsFinished <= 0) return null;
            string golden = GoldenReached ? "  <color=#ffd866>★ golden road walked</color>" : "";
            string runs = RunsFinished == 1 ? "1 saga completed" : $"{RunsFinished} sagas completed";
            return $"<color=#9ad>Legacy:</color> {runs} · endings discovered {SeenCount}/{Total}{golden}";
        }

        /// <summary>Wipe the legacy record (for a true clean slate). Not wired to any button by default.</summary>
        public static void Clear()
        {
            PlayerPrefs.DeleteKey(K_Mask);
            PlayerPrefs.DeleteKey(K_Runs);
            PlayerPrefs.DeleteKey(K_Golden);
            PlayerPrefs.Save();
        }
    }
}
