using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Core;
using SunderedCrown.Quests;
using SunderedCrown.World;

namespace SunderedCrown.UI
{
    /// <summary>
    /// Lightweight exploration HUD (OnGUI): the active party with HP, the journal of
    /// active quests and their objectives (ticked live from GameFlags), an interaction
    /// prompt for the nearest object, and any examine flavor text. Replace with a
    /// styled uGUI overlay later.
    /// </summary>
    public class ExplorationHUD : MonoBehaviour
    {
        /// <summary>The current place name, shown as a top-center banner. Scenes set this in their Begin().</summary>
        public static string Location = "";

        void OnGUI()
        {
            // Location banner (top-center).
            if (!string.IsNullOrEmpty(Location))
            {
                var style = new GUIStyle(GUI.skin.box) { fontSize = 15, alignment = TextAnchor.MiddleCenter, wordWrap = false };
                GUI.Label(new Rect(Screen.width / 2f - 230, 8, 460, 28), $"📍 {Location}", style);
            }

            // Party panel (top-left).
            if (Party.Instance != null && Party.Instance.active.Count > 0)
            {
                bool wounded = false;
                foreach (var m in Party.Instance.active)
                    if (m.maxHitPoints > 0 && m.currentHitPoints < m.maxHitPoints * 0.5f) { wounded = true; break; }

                GUILayout.BeginArea(new Rect(10, 10, 260, 28 + 22 * Party.Instance.active.Count + (wounded ? 20 : 0)), GUI.skin.box);
                GUILayout.Label("<b>Party</b>");
                foreach (var m in Party.Instance.active)
                {
                    bool hurt = m.maxHitPoints > 0 && m.currentHitPoints < m.maxHitPoints * 0.5f;
                    string hp = hurt ? $"<color=#e88>HP {m.currentHitPoints}/{m.maxHitPoints}</color>" : $"HP {m.currentHitPoints}/{m.maxHitPoints}";
                    GUILayout.Label($"{m.displayName}   {hp}   Lv {m.level}");
                }
                if (wounded) GUILayout.Label("<size=11><color=#9c9>💤 Rest at camp to heal.</color></size>");
                GUILayout.EndArea();
            }

            // Quest journal (top-right).
            DrawJournal();

            // Interaction prompt + examine text (bottom-center).
            var ex = ExplorationController.Active;
            if (ex != null)
            {
                if (ex.Nearby != null)
                {
                    string verb = ex.Nearby.kind switch
                    {
                        InteractionKind.Talk => "Talk to",
                        InteractionKind.Exit => "Enter",
                        _ => "Examine"
                    };
                    GUI.Box(new Rect(Screen.width / 2f - 170, Screen.height - 70, 340, 34),
                        $"[E] {verb} {ex.Nearby.label}   ·   (or click)");
                }

                if (Time.time < ex.ExamineUntil && !string.IsNullOrEmpty(ex.ExamineText))
                    GUI.Box(new Rect(Screen.width / 2f - 280, Screen.height - 130, 560, 50), ex.ExamineText);
            }
        }

        private void DrawJournal()
        {
            var qm = QuestManager.Instance;
            if (qm == null) return;

            GUILayout.BeginArea(new Rect(Screen.width - 320, 10, 310, 240), GUI.skin.box);
            GUILayout.Label("<b>Journal</b>");
            foreach (var quest in qm.allQuests)
            {
                if (quest == null) continue;
                var status = qm.StatusOf(quest.questId);
                if (status == QuestStatus.Unstarted) continue;

                string tag = status == QuestStatus.Completed ? " <color=#8f8>(done)</color>"
                           : status == QuestStatus.Failed ? " <color=#f88>(failed)</color>" : "";
                GUILayout.Label($"<b>{quest.title}</b>{tag}");
                foreach (var obj in quest.objectives)
                {
                    if (obj.hidden && status == QuestStatus.Active && !GameFlags.Current.GetBool(obj.completionFlag)) continue;
                    bool done = GameFlags.Current.GetBool(obj.completionFlag);
                    GUILayout.Label($"   {(done ? "✔" : "•")} {obj.description}");
                }
            }
            GUILayout.EndArea();
        }
    }
}
