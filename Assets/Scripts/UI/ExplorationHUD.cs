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
            DrawParty();

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

        // Portrait card geometry for a party row.
        private const float RowH = 46f, FaceW = 34f, FaceH = 42f;

        /// <summary>The party as portrait cards with real HP bars. The old panel was
        /// three columns of text; a face and a bar say the same thing at a glance and
        /// tie the HUD to the same painted fleet the world and dialogue use.</summary>
        private void DrawParty()
        {
            var party = Party.Instance;
            if (party == null || party.active.Count == 0) return;

            bool wounded = false;
            foreach (var m in party.active)
                if (m.maxHitPoints > 0 && m.currentHitPoints < m.maxHitPoints * 0.5f) { wounded = true; break; }

            float h = 30f + RowH * party.active.Count + (wounded ? 20f : 0f);
            var panel = new Rect(10, 10, 268, h);
            GUI.Box(panel, GUIContent.none);
            GUI.Label(new Rect(panel.x + 12, panel.y + 6, 200, 18), "<b>Party</b>");

            float y = panel.y + 26f;
            foreach (var m in party.active)
            {
                float frac = m.maxHitPoints > 0 ? Mathf.Clamp01(m.currentHitPoints / (float)m.maxHitPoints) : 0f;

                var face = new Rect(panel.x + 12, y + 2, FaceW, FaceH);
                var art = Rendering.WorldArt.Portrait(m.displayName);
                GUI.color = new Color(0f, 0f, 0f, 0.55f);
                GUI.DrawTexture(new Rect(face.x - 1, face.y - 1, face.width + 2, face.height + 2), Texture2D.whiteTexture);
                GUI.color = Color.white;
                if (art != null && art.texture != null)
                    GUI.DrawTexture(face, art.texture, ScaleMode.ScaleAndCrop);

                GUI.Label(new Rect(face.xMax + 8, y, 200, 18), $"{m.displayName}  <color=#8a7448>Lv {m.level}</color>");
                Bar(new Rect(face.xMax + 8, y + 22, 120, 10), frac,
                    frac < 0.34f ? new Color(0.73f, 0.24f, 0.24f)
                  : frac < 0.67f ? new Color(0.78f, 0.60f, 0.25f)
                                 : new Color(0.36f, 0.58f, 0.34f));
                GUI.Label(new Rect(face.xMax + 134, y + 16, 80, 18),
                    $"<size=11>{m.currentHitPoints}/{m.maxHitPoints}</size>");
                y += RowH;
            }

            if (wounded)
                GUI.Label(new Rect(panel.x + 12, y - 2, 240, 18),
                          "<size=11><color=#9c9>Rest at camp to heal.</color></size>");
        }

        /// <summary>A flat two-tone bar. IMGUI has no progress bar, and a row of
        /// characters is not one.</summary>
        private static void Bar(Rect r, float frac, Color fill)
        {
            var prev = GUI.color;
            GUI.color = new Color(0.04f, 0.03f, 0.06f, 0.95f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = fill;
            GUI.DrawTexture(new Rect(r.x + 1, r.y + 1, Mathf.Max(0f, (r.width - 2) * frac), r.height - 2),
                            Texture2D.whiteTexture);
            GUI.color = prev;
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
