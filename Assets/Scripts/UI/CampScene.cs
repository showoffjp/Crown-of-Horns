using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Core;
using SunderedCrown.Items;

namespace SunderedCrown.UI
{
    /// <summary>
    /// THE CAMP (OnGUI). A quiet beat between dangers: take a long rest to restore the party's HP and
    /// spell slots, or sit with whoever's grown closest to you for a campfire night-talk (gated on
    /// approval, one per companion). Set onLeave and drop it in a mode; zero setup.
    /// </summary>
    public class CampScene : MonoBehaviour
    {
        public System.Action onLeave;
        public System.Action onRested;   // optional: the director autosaves here

        private bool _rested;
        private CampContent.NightTalk _talk;   // currently-open monologue, or null
        private RomanceContent.Beat _beat;     // currently-open romance beat, or null
        private string _beatAfter;             // narration shown after a romance choice
        private RuptureContent.Rupture _rift;  // currently-open rupture confrontation, or null
        private string _riftAfter;             // narration shown after a rupture choice
        private bool _riftResolved;            // true once mended/patched/parted (vs a failed amends)
        private CampGroupBanter.Exchange _banter; // currently-open party cross-talk, or null
        private string _giftMsg;                  // last gift-giving flavour line
        private Vector2 _scroll;

        void OnGUI()
        {
            const float w = 760;
            float h = Screen.height - 80;
            float x = Screen.width / 2f - w / 2f, y = 40;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            _scroll = GUILayout.BeginScrollView(_scroll);

            if (_rift != null) DrawRupture();
            else if (_beat != null) DrawRomance();
            else if (_banter != null) DrawBanter();
            else if (_talk != null) DrawTalk();
            else DrawCamp();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawCamp()
        {
            GUILayout.Label("<size=24><b>🔥 CAMP</b></size>");
            GUILayout.Label("<color=#c9a0ff><i>The fire is low. For a little while, nothing is hunting you.</i></color>");
            GUILayout.Space(12);

            // --- A fraying bond (urgent: someone's had enough) ---
            var rift = RuptureContent.Pending();
            if (rift != null)
            {
                GUILayout.Label("<size=16><b><color=#e57>A Bond Frays</color></b></size>");
                GUILayout.Label("<color=#e99><i>Someone is standing between you and the fire, and they are not smiling.</i></color>");
                if (GUILayout.Button($"⚠ {rift.title}", LeftBtn(), GUILayout.Height(32)))
                {
                    _rift = rift;
                    _riftAfter = null;
                }
                GUILayout.Space(14);
            }

            // --- Long rest ---
            GUILayout.Label("<size=16><b>Long Rest</b></size>");
            if (!_rested)
            {
                GUILayout.Label("<color=#aaa>Restore your fielded party to full hit points and refresh all spell slots.</color>");
                if (GUILayout.Button("Take a long rest", GUILayout.Height(36)))
                    DoRest();
            }
            else
            {
                GUILayout.Label("<color=#8f8>✔ The party wakes whole — wounds closed, magic gathered again." +
                                (onRested != null ? "  <color=#9fd>(progress saved)</color>" : "") + "</color>");
            }
            GUILayout.Space(14);

            // --- Night-talk ---
            GUILayout.Label("<size=16><b>Sit With Someone</b></size>");
            var best = CampContent.Best();
            if (best != null)
            {
                GUILayout.Label($"<color=#aaa>Someone by the fire looks ready to talk.</color>");
                if (GUILayout.Button($"▸ {best.title}", LeftBtn(), GUILayout.Height(32)))
                {
                    _talk = best;
                    GameFlags.Current.SetBool($"camp.nighttalk.{best.DoneKey}.done", true);
                    GameFlags.Current.AdjustApproval(best.id, 5); // the listening itself earns a little
                }
            }
            else
            {
                GUILayout.Label("<color=#888><i>No one's ready to open up tonight. Win their trust in the world " +
                                "above — and field them — and the fire will loosen tongues.</i></color>");
            }
            GUILayout.Space(14);

            // --- Party cross-talk ---
            GUILayout.Label("<size=16><b>The Party Falls to Talking</b></size>");
            var ex = CampGroupBanter.Best();
            if (ex != null)
            {
                GUILayout.Label("<color=#aaa>Two of them have got to talking across the fire. Listen in?</color>");
                if (GUILayout.Button($"▸ {ex.title}", LeftBtn(), GUILayout.Height(32)))
                {
                    _banter = ex;
                    GameFlags.Current.SetBool($"camp.banter.{ex.id}.done", true);
                    GameFlags.Current.AdjustApproval(ex.aMatch.ToLower(), 2); // shared fire warms them both
                    GameFlags.Current.AdjustApproval(ex.bMatch.ToLower(), 2);
                }
            }
            else
            {
                GUILayout.Label("<color=#888><i>The fire is quiet tonight. Field different companions together to " +
                                "hear them spark off one another.</i></color>");
            }
            GUILayout.Space(14);

            // --- Give a gift (spend a consumable to warm a bond) ---
            GUILayout.Label("<size=16><b>Give a Gift</b></size>");
            var party = Party.Instance;
            var gift = FindGiftable();
            if (party != null && gift != null)
            {
                GUILayout.Label($"<color=#aaa>You could share the <b>{gift.displayName}</b> from your pack with someone.</color>");
                foreach (var m in party.active)
                {
                    string id = CompanionId(m?.displayName);
                    if (id == null) continue;
                    if (GUILayout.Button($"Give {Short(m.displayName)} the {gift.displayName}  (+approval)", LeftBtn(), GUILayout.Height(28)))
                    {
                        party.inventory.Remove(gift.itemId, 1);
                        GameFlags.Current.AdjustApproval(id, 3);
                        _giftMsg = $"{Short(m.displayName)} turns the {gift.displayName} over once, then pockets it. " +
                                   "\"...You thought of me. Noted.\"";
                    }
                }
                if (!string.IsNullOrEmpty(_giftMsg))
                    GUILayout.Label($"<color=#8f8><i>{_giftMsg}</i></color>");
            }
            else
            {
                GUILayout.Label("<color=#888><i>Nothing to give just now — pick up a consumable on the road and you can " +
                                "share it at the fire.</i></color>");
            }
            GUILayout.Space(14);

            // --- Romance (the slow burn) ---
            GUILayout.Label("<size=16><b>Grow Closer</b></size>");
            var beat = RomanceContent.Best();
            if (beat != null)
            {
                GUILayout.Label("<color=#f6a><i>A moment by the fire feels like it could be more, if you let it.</i></color>");
                if (GUILayout.Button($"♥ {beat.title}", LeftBtn(), GUILayout.Height(32)))
                {
                    _beat = beat;
                    _beatAfter = null;
                }
            }
            else
            {
                GUILayout.Label("<color=#888><i>No hearts are ready for their next turn tonight. Affinity, their " +
                                "personal quest, and the slow earning of trust open the way.</i></color>");
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Break camp", GUILayout.Height(34)))
                onLeave?.Invoke();
        }

        private void DrawRupture()
        {
            GUILayout.Label($"<size=18><b><color=#e57>⚠ {_rift.title}</color></b></size>");
            GUILayout.Space(10);
            foreach (var line in _rift.lines)
            {
                GUILayout.Label(line);
                GUILayout.Space(8);
            }
            GUILayout.Space(6);

            if (_riftAfter == null)
            {
                if (GUILayout.Button(_rift.amendsLabel, LeftBtn(), GUILayout.Height(34)))
                {
                    if (RuptureContent.HasStanding(GameFlags.Current, _rift.id)) MendRupture();
                    else { _riftAfter = _rift.amendsThin; _riftResolved = false; } // words alone aren't enough
                }
                GUILayout.Space(4);
                if (GUILayout.Button(_rift.patchLabel, LeftBtn(), GUILayout.Height(34))) PatchRupture();
                GUILayout.Space(4);
                if (GUILayout.Button(_rift.partLabel, LeftBtn(), GUILayout.Height(34))) PartRupture();
            }
            else
            {
                GUILayout.Label($"<color=#c9a0ff><i>{_riftAfter}</i></color>");
                GUILayout.Space(10);
                if (!_riftResolved)
                {
                    GUILayout.Label("<color=#e99>It isn't enough on its own. So — which is it?</color>");
                    GUILayout.Space(4);
                    if (GUILayout.Button(_rift.patchLabel, LeftBtn(), GUILayout.Height(34))) PatchRupture();
                    GUILayout.Space(4);
                    if (GUILayout.Button(_rift.partLabel, LeftBtn(), GUILayout.Height(34))) PartRupture();
                }
                else if (GUILayout.Button("Back to the fire", GUILayout.Height(32)))
                {
                    _rift = null; _riftAfter = null; _riftResolved = false;
                }
            }
        }

        private void MendRupture()
        {
            var f = GameFlags.Current;
            f.SetBool($"rupture.{_rift.id}.mended", true);
            f.SetInt($"companion.{_rift.id}.approval", Mathf.Max(f.GetInt($"companion.{_rift.id}.approval"), 20));
            _riftAfter = _rift.amendsWin; _riftResolved = true;
        }

        private void PatchRupture()
        {
            var f = GameFlags.Current;
            f.SetBool($"rupture.{_rift.id}.uneasy", true);
            f.SetInt($"companion.{_rift.id}.approval", Mathf.Max(f.GetInt($"companion.{_rift.id}.approval"), 0));
            _riftAfter = _rift.patchResult; _riftResolved = true;
        }

        private void PartRupture()
        {
            var f = GameFlags.Current;
            f.SetBool($"rupture.{_rift.id}.broken", true);
            f.SetBool($"companion.{_rift.id}.left", true);
            RemoveFromParty(_rift.nameMatch);
            _riftAfter = _rift.partResult; _riftResolved = true;
        }

        /// <summary>The first Consumable in the party stash (resolved via ItemDatabase), or null.</summary>
        private static ItemDefinition FindGiftable()
        {
            var inv = Party.Instance != null ? Party.Instance.inventory : null;
            if (inv == null) return null;
            foreach (var s in inv.stacks)
            {
                if (s == null || s.count <= 0) continue;
                var def = ItemDatabase.Get(s.itemId);
                if (def != null && def.kind == ItemKind.Consumable) return def;
            }
            return null;
        }

        private static readonly System.Collections.Generic.Dictionary<string, string> _companionIds =
            new System.Collections.Generic.Dictionary<string, string>
        {
            { "Garrow", "garrow" }, { "Roen", "roen" }, { "Varra", "varra" },
            { "Naeve", "naeve" }, { "Ilfaeril", "ilfaeril" }, { "Maerin", "maerin" },
        };

        private static string CompanionId(string displayName)
        {
            if (string.IsNullOrEmpty(displayName)) return null;
            foreach (var kv in _companionIds)
                if (displayName.Contains(kv.Key)) return kv.Value;
            return null;
        }

        private static string Short(string name)
        {
            if (string.IsNullOrEmpty(name)) return "?";
            int sp = name.IndexOf(' ');
            return sp > 2 ? name.Substring(0, sp) : name;
        }

        private void RemoveFromParty(string nameMatch)
        {
            var party = Party.Instance;
            if (party == null) return;
            CharacterSheet found = null;
            foreach (var m in party.roster)
                if (m?.displayName != null && m.displayName.Contains(nameMatch)) { found = m; break; }
            if (found != null) party.Remove(found);
        }

        private void DrawRomance()
        {
            GUILayout.Label($"<size=18><b>♥ {_beat.title}</b></size>");
            GUILayout.Space(10);
            foreach (var line in _beat.lines)
            {
                GUILayout.Label(line);
                GUILayout.Space(8);
            }
            GUILayout.Space(6);

            if (_beatAfter == null)
            {
                if (GUILayout.Button(_beat.pursueLabel, LeftBtn(), GUILayout.Height(34)))
                {
                    GameFlags.Current.SetBool(_beat.setFlag, true);
                    GameFlags.Current.AdjustApproval(_beat.id, 8); // choosing them moves the meter
                    _beatAfter = _beat.pursueResult;
                }
                GUILayout.Space(4);
                if (GUILayout.Button(_beat.holdLabel, LeftBtn(), GUILayout.Height(34)))
                {
                    // non-destructive: no flag set, so the beat remains available another night
                    _beatAfter = _beat.holdResult;
                }
            }
            else
            {
                GUILayout.Label($"<color=#c9a0ff><i>{_beatAfter}</i></color>");
                GUILayout.Space(10);
                if (GUILayout.Button("Back to the fire", GUILayout.Height(32)))
                {
                    _beat = null;
                    _beatAfter = null;
                }
            }
        }

        private void DrawTalk()
        {
            GUILayout.Label($"<size=18><b>{_talk.title}</b></size>");
            GUILayout.Space(10);
            foreach (var line in _talk.lines)
            {
                GUILayout.Label(line);
                GUILayout.Space(8);
            }
            GUILayout.Space(8);
            if (GUILayout.Button("Back to the fire", GUILayout.Height(32)))
                _talk = null;
        }

        private void DrawBanter()
        {
            GUILayout.Label($"<size=18><b>{_banter.title}</b></size>");
            GUILayout.Space(10);
            foreach (var line in _banter.lines)
            {
                GUILayout.Label(line);
                GUILayout.Space(8);
            }
            GUILayout.Space(8);
            if (GUILayout.Button("Back to the fire", GUILayout.Height(32)))
                _banter = null;
        }

        private void DoRest()
        {
            var party = Party.Instance;
            if (party != null)
            {
                foreach (var m in party.active)
                {
                    if (m == null) continue;
                    m.Heal(m.maxHitPoints);            // to full
                    m.spellSlots?.RestoreAll();        // slots back
                }
            }
            _rested = true;
            GameFlags.Current.AddInt("camp.rests", 1);
            onRested?.Invoke();   // the director autosaves on rest (a natural checkpoint)
        }

        private static GUIStyle _left;
        private GUIStyle LeftBtn()
        {
            if (_left == null)
                _left = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft, richText = true };
            return _left;
        }
    }
}
