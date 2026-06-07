using UnityEngine;
using SunderedCrown.Grid;
using SunderedCrown.Combat;

namespace SunderedCrown.UI
{
    /// <summary>
    /// World-space nameplates: a small name label + HP bar floating over every GridUnit, color-coded by
    /// faction, with the active combatant highlighted. Turns the "colored cubes" into something legible at
    /// a glance — who's who, who's hurt, whose turn it is — with zero art. Toggle with N. Drop on any
    /// persistent object (the campaign director adds one); it finds units itself.
    /// </summary>
    public class UnitNameplates : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.N;
        public bool show = true;

        private GridUnit[] _units = new GridUnit[0];
        private static Texture2D _white;

        void Update()
        {
            if (Input.GetKeyDown(toggleKey)) show = !show;
            // Refresh the unit list once per frame (cheap at this scale; avoids per-OnGUI churn).
            _units = FindObjectsByType<GridUnit>(FindObjectsSortMode.None);
        }

        void OnGUI()
        {
            if (!show) return;
            var cam = Camera.main;
            if (cam == null) return;
            if (_white == null) { _white = new Texture2D(1, 1); _white.SetPixel(0, 0, Color.white); _white.Apply(); }

            var active = TurnManager.Instance != null ? TurnManager.Instance.ActiveUnit : null;

            foreach (var u in _units)
            {
                if (u == null || u.Sheet == null) continue;

                Vector3 sp = cam.WorldToScreenPoint(u.transform.position + Vector3.up * 0.8f);
                if (sp.z < 0) continue; // behind the camera
                float x = sp.x, y = Screen.height - sp.y;

                const float w = 64f, barH = 6f;
                Color tint = Faction(u.faction);
                bool isActive = u == active;
                bool down = !u.Sheet.IsAlive;

                // Name label.
                var label = new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, fontSize = 11, richText = true };
                string nm = Short(u.Sheet.displayName);
                string tag = isActive ? $"<b>▶ {nm}</b>" : down ? $"<color=#888>{nm} (down)</color>" : nm;
                var nameRect = new Rect(x - w, y - 26, w * 2f, 16);
                var prev = GUI.color;
                GUI.color = isActive ? new Color(1f, 0.9f, 0.5f) : tint;
                GUI.Label(nameRect, tag, label);
                GUI.color = prev;

                // HP bar (skip for downed — the label says it).
                if (!down)
                {
                    float pct = u.Sheet.maxHitPoints > 0 ? Mathf.Clamp01((float)u.Sheet.currentHitPoints / u.Sheet.maxHitPoints) : 0f;
                    var bg = new Rect(x - w / 2f, y - 8, w, barH);
                    Bar(bg, new Color(0f, 0f, 0f, 0.6f));
                    var fill = new Rect(bg.x, bg.y, bg.width * pct, bg.height);
                    Bar(fill, HpColor(pct));
                    if (isActive) Outline(bg, new Color(1f, 0.9f, 0.5f, 0.9f));

                    // Compact status line under the bar (conditions + dodge/disengage stance).
                    string cond = Conditions(u.Sheet);
                    if (!string.IsNullOrEmpty(cond))
                    {
                        var cs = new GUIStyle(GUI.skin.label)
                        { alignment = TextAnchor.MiddleCenter, fontSize = 9, richText = true };
                        GUI.Label(new Rect(x - w, y + 1, w * 2f, 12), cond, cs);
                    }
                }
            }
        }

        private static void Bar(Rect r, Color c)
        {
            var prev = GUI.color; GUI.color = c;
            GUI.DrawTexture(r, _white);
            GUI.color = prev;
        }

        private static void Outline(Rect r, Color c)
        {
            Bar(new Rect(r.x - 1, r.y - 1, r.width + 2, 1), c);
            Bar(new Rect(r.x - 1, r.yMax, r.width + 2, 1), c);
            Bar(new Rect(r.x - 1, r.y - 1, 1, r.height + 2), c);
            Bar(new Rect(r.xMax, r.y - 1, 1, r.height + 2), c);
        }

        private static Color Faction(Faction f) => f switch
        {
            Grid.Faction.Player => new Color(0.55f, 0.75f, 1f),
            Grid.Faction.Ally   => new Color(0.6f, 1f, 0.7f),
            Grid.Faction.Enemy  => new Color(1f, 0.55f, 0.55f),
            _                   => new Color(0.8f, 0.8f, 0.85f),
        };

        private static Color HpColor(float pct) =>
            pct > 0.5f ? new Color(0.4f, 0.85f, 0.4f)
            : pct > 0.25f ? new Color(0.9f, 0.8f, 0.3f)
            : new Color(0.9f, 0.35f, 0.3f);

        /// <summary>A short, color-coded condition string for a unit's nameplate: active status effects
        /// (with rounds) plus the Dodging/Disengaged stance. Empty when there's nothing to show.</summary>
        private static string Conditions(SunderedCrown.Characters.CharacterSheet s)
        {
            var parts = new System.Collections.Generic.List<string>();
            if (s.IsDodging) parts.Add("<color=#8cf>Dodge</color>");
            if (s.IsDisengaging) parts.Add("<color=#9c9>Diseng</color>");
            if (s.activeEffects != null)
            {
                int shown = 0;
                foreach (var e in s.activeEffects)
                {
                    if (e?.def == null) continue;
                    string col = e.def.isBeneficial ? "#9f9" : "#f99";
                    parts.Add($"<color={col}>{e.def.effectName}{(e.remainingRounds > 0 ? " " + e.remainingRounds : "")}</color>");
                    if (++shown >= 3) break;
                }
            }
            return parts.Count == 0 ? "" : string.Join(" ", parts);
        }

        private static string Short(string name)
        {
            if (string.IsNullOrEmpty(name)) return "?";
            int sp = name.IndexOf(' ');
            return sp > 2 ? name.Substring(0, sp) : name; // first name / short label
        }
    }
}
