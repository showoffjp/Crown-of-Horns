using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Core;
using SunderedCrown.Items;
using SunderedCrown.Content;

namespace SunderedCrown.UI
{
    /// <summary>
    /// The Vault of Tens solver UI (OnGUI). A pedestal opens an *object* riddle (place a token from
    /// the party pouch); the brazier opens a *spoken* riddle (type the word). Correct answers solve;
    /// genuinely clever-wrong tokens earn her *amusement* instead; dull answers cost amusement. She
    /// never kills you — she only gets bored. Tracks state in GameFlags (riddle.&lt;id&gt;.solved,
    /// riddle.amusement, riddle.solvedCount).
    /// </summary>
    public class RiddleVaultUI : MonoBehaviour
    {
        public RiddleContent content;

        private Riddle _current;
        private string _typed = "";
        private string _result = "";
        private Vector2 _scroll;

        public bool IsOpen => _current != null;

        /// <summary>True while any riddle panel is open — exploration input checks this so clicking the
        /// panel doesn't also move the leader.</summary>
        public static bool AnyOpen { get; private set; }

        public void OpenObject(Riddle r) { _current = r; _result = ""; _typed = ""; AnyOpen = true; }
        public void OpenSpoken(Riddle r) { _current = r; _result = ""; _typed = ""; AnyOpen = true; }
        public void Close() { _current = null; _result = ""; AnyOpen = false; }
        void OnDestroy() { AnyOpen = false; }

        public int Amusement => GameFlags.Current.GetInt("riddle.amusement");
        public int SolvedCount => GameFlags.Current.GetInt("riddle.solvedCount");
        public bool Solved(string id) => GameFlags.Current.GetBool($"riddle.{id}.solved");

        void OnGUI()
        {
            // Persistent corner readout of her interest.
            GUI.Label(new Rect(Screen.width - 230, 60, 220, 24),
                $"<b>Her amusement:</b> {Amusement}   ·   solved {SolvedCount}/11");

            if (_current == null) return;

            const float w = 640, h = 420;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

            GUILayout.Label("<b><color=#c9a0ff>The Lady in the Margins asks…</color></b>");
            GUILayout.Label($"<i>“{_current.prompt}”</i>");
            GUILayout.Space(8);

            if (Solved(_current.id))
                GUILayout.Label("<color=#8f8>(Already answered. She taps her temple, pleased.)</color>");
            else if (_current.locked && !GameFlags.Current.GetBool("act5"))
                GUILayout.Label("<color=#fc8>She lays a finger to your lips before you can speak. “Not that one. Not yet.”</color>");
            else if (_current.spoken)
                DrawSpoken();
            else
                DrawObject();

            if (!string.IsNullOrEmpty(_result))
            {
                GUILayout.Space(6);
                GUILayout.Label($"<color=#c9a0ff>“{_result}”</color>");
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Step back from the pedestal")) Close();
            GUILayout.EndArea();
        }

        private void DrawSpoken()
        {
            GUILayout.Label("Speak your answer:");
            _typed = GUILayout.TextField(_typed, GUILayout.Width(300));
            if (GUILayout.Button("Say it", GUILayout.Width(120)))
            {
                string a = (_typed ?? "").Trim().ToLowerInvariant();
                bool correct = false;
                if (_current.spokenAnswers != null)
                    foreach (var s in _current.spokenAnswers) if (s == a) { correct = true; break; }
                if (correct) Resolve(true, false);
                else Resolve(false, false);
            }
        }

        private void DrawObject()
        {
            GUILayout.Label("Place a token from your pouch:");
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(210));
            var inv = Party.Instance != null ? Party.Instance.inventory : null;
            if (inv == null) { GUILayout.Label("(no pouch)"); GUILayout.EndScrollView(); return; }

            foreach (var stack in inv.stacks)
            {
                var item = ItemDatabase.Get(stack.itemId);
                if (item == null || content == null || !content.Tokens.ContainsKey(item.itemId)) continue; // tokens only
                if (GUILayout.Button(item.displayName))
                {
                    bool correct = item.itemId == _current.answerToken;
                    bool clever = !correct && Contains(_current.cleverTokens, item.itemId);
                    Resolve(correct, clever);
                }
            }
            GUILayout.EndScrollView();
        }

        private void Resolve(bool correct, bool clever)
        {
            var f = GameFlags.Current;
            if (correct)
            {
                f.SetBool($"riddle.{_current.id}.solved", true);
                f.AddInt("riddle.solvedCount", 1);
                f.AddInt("riddle.amusement", 2);
                _result = _current.onSolve;
                GrantRewards();
            }
            else if (clever)
            {
                f.AddInt("riddle.amusement", 3); // wit is worth more than truth
                f.SetBool($"riddle.{_current.id}.clever", true);
                _result = _current.onClever;
            }
            else
            {
                f.AddInt("riddle.amusement", -1);
                _result = _current.onDull;
            }
        }

        // ---- her rewards (granted once, at milestones) ----
        private void GrantRewards()
        {
            int solved = SolvedCount;
            if (solved >= 5)
                Grant("five", "coin_tenth", 1,
                    "Five. You've earned a little luck. Spend it when fate needs a nudge.");
            if (solved >= 10 && Amusement >= 15)
                Grant("ten", "her_favour", 1,
                    "Ten — and you kept me *entertained.* Take my Favour. Use it the moment you'd otherwise lose someone.");
            if (_current.id == "hername")
                Grant("name", "readers_boon", 1,
                    "You read me. Keep this — proof that, once, the margin smiled.");
        }

        private void Grant(string milestone, string itemId, int count, string line)
        {
            var f = GameFlags.Current;
            if (f.GetBool($"riddle.reward.{milestone}")) return;
            f.SetBool($"riddle.reward.{milestone}", true);

            ItemDefinition item = (content != null && content.Rewards.ContainsKey(itemId)) ? content.Rewards[itemId] : null;
            if (item != null && Party.Instance != null) Party.Instance.inventory.Add(item, count);

            string name = item != null ? item.displayName : itemId;
            _result += $"\n\n<color=#ffd866>[She presses something into your hand: {name}{(count > 1 ? " ×" + count : "")}.]</color>\n“{line}”";
        }

        private static bool Contains(string[] arr, string v)
        {
            if (arr == null) return false;
            foreach (var s in arr) if (s == v) return true;
            return false;
        }
    }
}
