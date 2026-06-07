using UnityEngine;
using SunderedCrown.Core;

namespace SunderedCrown.UI
{
    /// <summary>
    /// The Journey tracker (toggle with J). Reads the playthrough out of GameFlags and shows the
    /// campaign as a checklist — the Cinderhaunt, the four eras, the Breach, the Lady's riddle — plus
    /// which tier of ending you've earned the right to choose. Makes the saga legible as a quest.
    /// </summary>
    public class JourneyScreen : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.J;
        private bool _open;
        private Vector2 _scroll;

        void Update() { if (Input.GetKeyDown(toggleKey)) _open = !_open; }

        void OnGUI()
        {
            if (!_open) return;
            var f = GameFlags.Current;

            const float w = 460, h = 600;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            GUILayout.Label($"<size=18><b>📜 THE JOURNEY</b></size>   <size=11>(press {toggleKey} to close)</size>");
            GUILayout.Space(6);
            _scroll = GUILayout.BeginScrollView(_scroll);

            Item(f.GetBool("prologue.cleared"), "Prologue — escape the Cinderhaunt beneath the Gate");
            Item(f.GetBool("aldric.met"), "Tea with a Heretic — meet Aldric Morn");
            Item(f.GetBool("act2.breach_done"), "The Breach — pull Maerin from the Wall (a permanent cost)");
            GUILayout.Space(4);
            GUILayout.Label("<b>The Eras of History</b>");
            Item(f.GetBool("netheril.cleared"), "Netheril — survive the falling city (Karsus's Folly)");
            Item(f.GetBool("crownwars.cleared"), "The Crown Wars — the first damnation", f.GetBool("crownwars.verdict_spared") ? "verdict argued DOWN" : null);
            Item(f.GetBool("act4.toot_done"), "The Time of Troubles — the Crown forged from Myrkul's skull");
            Item(f.GetBool("act4.spellplague_done"), "The Spellplague — where causality runs like ink");
            GUILayout.Space(4);
            GUILayout.Label("<b>Companion Threads</b>");
            int threads = 0;
            foreach (var hook in SunderedCrown.Content.CompanionQuests.Hooks)
            {
                if (!f.GetBool($"camp.nighttalk.{hook.id}.done")) continue;
                threads++;
                string short_ = hook.label.Contains("(") ? hook.label.Substring(hook.label.IndexOf('(') + 1).TrimEnd(')') : hook.label;
                Item(f.GetBool(hook.resolvedFlag), short_, QuestBeat(f, hook.id));
            }
            if (threads == 0)
                GUILayout.Label("  <color=#888><i>Share a campfire night-talk to open a companion's personal thread.</i></color>");

            GUILayout.Space(4);
            GUILayout.Label("<b>Bonds</b>");
            int bonds = 0;
            foreach (var id in new[] { "garrow", "roen", "naeve", "varra" })
            {
                string stage = RomanceStage(f, id);
                if (stage == null) continue;
                bonds++;
                string name = char.ToUpper(id[0]) + id.Substring(1);
                bool sealed_ = f.GetBool($"romance.{id}.consummated");
                Item(sealed_, name, stage);
            }
            if (bonds == 0)
                GUILayout.Label("  <color=#888><i>No hearts kindled yet. The fire — and time — open the way.</i></color>");

            if (f.GetBool("prologue.cleared"))
            {
                GUILayout.Space(4);
                GUILayout.Label($"<b>The Lower City</b>   <size=11><color=#9fd>(standing {f.GetInt("reputation.lowcity")})</color></size>");
                Item(f.GetBool("quest.widow.resolved"), "The Widow's Coin — Mhaere and her lost son",
                    f.GetBool("quest.widow.hope") ? "his name, kept" : f.GetBool("quest.widow.truth") ? "the hard truth" : f.GetBool("quest.widow.lie") ? "a kind lie" : null);
                Item(f.GetBool("quest.fist.resolved"), "The Fist and the Faithless — a beggar at the Wall's edge",
                    f.GetBool("quest.fist.freed") ? "freed" : f.GetBool("quest.fist.bribed") ? "bought loose" : f.GetBool("quest.fist.lawful") ? "let to the law" : null);
                Item(f.GetBool("quest.choir.resolved"), "The Faithless Choir — a preacher of the Unmade",
                    f.GetBool("quest.choir.doubted") ? "given doubt" : f.GetBool("quest.choir.suppressed") ? "silenced by the Fist" : f.GetBool("quest.choir.favored") ? "spoken for" : null);
                if (f.GetBool("quest.choir.resolved"))
                    Item(f.GetBool("quest.choir.cell_cleared"), "The Choir's undercroft — the Hollow Cantor",
                        f.GetBool("quest.choir.cell_cleared") ? "cell broken" : "a cell gathers");
                Item(f.GetBool("quest.tithe.resolved"), "The Tithe Collector — who pays for the dead's rest",
                    f.GetBool("quest.tithe.freed") ? "ledger torn up" : f.GetBool("quest.tithe.paid") ? "debts paid yourself" : f.GetBool("quest.tithe.corrupt") ? "took a cut" : null);
                Item(f.GetBool("quest.letter.resolved"), "The Last Letter — Old Davyn's old wrong",
                    f.GetBool("quest.letter.delivered") ? "delivered" : f.GetBool("quest.letter.burned") ? "burned" : f.GetBool("quest.letter.read") ? "read aloud" : null);
                if (f.GetBool("docks.ferryman_resolved"))
                    Item(true, "The Sinking Skiff — old Pell in the river",
                        f.GetBool("docks.ferryman_saved") ? "you went in after him" : "you walked on");
                if (f.GetBool("market.urchin_resolved"))
                    Item(true, "A Finger or the Cells — a child thief and the Fist",
                        f.GetBool("market.urchin_freed") ? "you stood surety" : "you let the law take them");
                if (f.GetBool("almshouse.deathbed_resolved"))
                    Item(true, "Old Hensley's Last Question — a dying man and an absent son",
                        f.GetBool("almshouse.deathbed_lie") ? "you gave him a kind lie" : "you gave him the truth");
                if (f.GetBool("safehouse.informant_resolved"))
                    Item(true, "The Bound Informant — a snitch's fate at the Niche",
                        f.GetBool("safehouse.informant_freed") ? "you cut her loose" : "you turned her");
                if (f.GetBool("lowcity.allies"))
                    GUILayout.Label("  <color=#8f8>✔</color> <color=#9fd>The quarter stands with you — they'll be there at the end.</color>");
            }

            // Field record — a compact line of combat tallies.
            int wonB = f.GetInt("combat.victories"); int slain = f.GetInt("slain.total");
            if (wonB > 0 || slain > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label($"<b>Field Record</b>   <size=11><color=#9fd>{wonB} battles won · {slain} foes laid low</color></size>");
            }

            // Standing with the Realms' powers — surfaced once any of them has noticed you.
            DrawStandings(f);

            GUILayout.Space(4);
            GUILayout.Label("<b>The Lady in the Margins</b>");
            Item(f.GetInt("riddle.solvedCount") > 0, $"The Vault of Tens — riddles solved: {f.GetInt("riddle.solvedCount")}/11", $"amusement {f.GetInt("riddle.amusement")}");
            Item(f.GetBool("readers_boon"), "Read her name — the Reader's Boon");

            GUILayout.Space(10);
            int endings = EndingResolver.Available().Count;
            bool golden = EndingResolver.Available().Contains(Ending.JergalsKeyhole) || EndingResolver.Available().Contains(Ending.BreakTheLoop);
            GUILayout.Label($"<b>Endings unlocked:</b> {endings}/6   {(golden ? "<color=#ffd866>★ a golden road is open</color>" : "<color=#888>(deeper truths unlock the golden roads)</color>")}");
            if (f.GetBool("netheril.cleared"))
                GUILayout.Label("<color=#c9a0ff>The Court of the Dead awaits in the hub.</color>");
            else
                GUILayout.Label("<color=#888>Walk at least one era to open the Court of the Dead.</color>");

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static readonly (string flag, string name)[] Factions =
        {
            ("faction.kelemvor.reputation", "The Doomguides of Kelemvor"),
            ("faction.choir.reputation", "The Faithless Choir"),
            ("faction.ashpact.reputation", "The Ash Pact"),
        };

        /// <summary>Show each faction whose standing is non-zero, with a descriptive tier.</summary>
        private void DrawStandings(GameFlags f)
        {
            bool any = false;
            foreach (var (flag, _) in Factions) if (f.GetInt(flag) != 0) { any = true; break; }
            if (!any) return;

            GUILayout.Space(4);
            GUILayout.Label("<b>Standing with the Powers</b>");
            foreach (var (flag, name) in Factions)
            {
                int rep = f.GetInt(flag);
                if (rep == 0) continue;
                string tier = RepTier(rep);
                string col = rep > 0 ? "#9fd" : "#d99";
                GUILayout.Label($"  <color={col}>{name}</color> — <i>{tier}</i> <size=11><color=#888>({rep:+0;-0})</color></size>");
            }
        }

        private static string RepTier(int rep) =>
            rep >= 5 ? "honored" : rep >= 2 ? "trusted" : rep >= 1 ? "known" :
            rep <= -5 ? "reviled" : rep <= -2 ? "distrusted" : "marked";

        private void Item(bool done, string label, string note = null)
        {
            string mark = done ? "<color=#8f8>✔</color>" : "<color=#666>○</color>";
            string extra = string.IsNullOrEmpty(note) ? "" : $"  <color=#9fd><i>({note})</i></color>";
            GUILayout.Label($"  {mark} {(done ? label : $"<color=#aaa>{label}</color>")}{extra}");
        }

        /// <summary>The current beat of a companion's personal quest, read straight from the flags.</summary>
        private static string QuestBeat(GameFlags f, string id)
        {
            if (f.GetBool($"quest.{id}.resolved")) return "resolved — " + Resolution(f, id);
            if (f.GetBool($"{id}.quest.cleared"))  return "fought — awaiting your call";
            if (f.GetBool($"{id}.quest.arrived"))  return "at the scene";
            if (f.GetBool($"quest.{id}.started"))  return "thread opened";
            return "not yet followed";
        }

        private static string Resolution(GameFlags f, string id)
        {
            switch (id)
            {
                case "roen":
                    if (f.GetBool("quest.roen.double_agent")) return "Wrenna turned double-agent";
                    if (f.GetBool("quest.roen.wrenna_saved")) return "Wrenna spared";
                    if (f.GetBool("quest.roen.harper_boon"))  return "Wrenna turned in";
                    break;
                case "varra":
                    if (f.GetBool("quest.varra.patron_bound")) return "patron bound";
                    if (f.GetBool("quest.varra.debt_taken"))   return "her debt carried";
                    if (f.GetBool("quest.varra.freed"))        return "contract burned";
                    break;
                case "garrow":
                    if (f.GetBool("quest.garrow.doctrine_won")) return "doctrine on trial";
                    if (f.GetBool("quest.garrow.left_faith"))   return "left the faith";
                    if (f.GetBool("quest.garrow.recanted"))     return "recanted, kept the grey";
                    break;
                case "naeve":
                    if (f.GetBool("quest.naeve.rekindled")) return "Weave rekindled";
                    if (f.GetBool("quest.naeve.released"))  return "laid to rest";
                    if (f.GetBool("quest.naeve.preserved")) return "frozen in stasis";
                    break;
                case "ilfaeril":
                    if (f.GetBool("quest.ilfaeril.commission")) return "forgiveness as a commission";
                    if (f.GetBool("quest.ilfaeril.forgiven"))   return "forgiveness accepted";
                    if (f.GetBool("quest.ilfaeril.penance"))    return "keeps paying";
                    break;
            }
            return "done";
        }

        private static readonly (string key, string label)[] RomanceLadder =
        {
            ("consummated", "the last night"), ("choosing", "committed"), ("crisis", "through the crisis"),
            ("turn", "the turn"), ("trust", "trust"), ("spark", "a spark"),
        };

        /// <summary>The furthest romance stage reached with a companion, or null if none.</summary>
        private static string RomanceStage(GameFlags f, string id)
        {
            foreach (var (key, label) in RomanceLadder)
                if (f.GetBool($"romance.{id}.{key}")) return label;
            return null;
        }
    }
}
