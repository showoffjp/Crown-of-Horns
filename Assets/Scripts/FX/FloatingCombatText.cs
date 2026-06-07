using System.Collections.Generic;
using UnityEngine;

namespace SunderedCrown.FX
{
    /// <summary>
    /// Fire-and-forget floating combat numbers: damage, heals, MISS/RESIST, crits, and conditions pop off
    /// a world position, drift up, and fade. Self-bootstraps a hidden driver on first use; no scene setup.
    /// Pairs with UnitNameplates to make a cube-fight readable at a glance. Pure IMGUI, zero art.
    /// </summary>
    public static class FloatingCombatText
    {
        public static readonly Color Damage   = new Color(1f, 0.5f, 0.45f);
        public static readonly Color Crit     = new Color(1f, 0.75f, 0.25f);
        public static readonly Color Heal     = new Color(0.5f, 1f, 0.6f);
        public static readonly Color Miss     = new Color(0.8f, 0.8f, 0.85f);
        public static readonly Color Condition = new Color(0.8f, 0.6f, 1f);

        private static Driver _driver;

        public static void Spawn(Vector3 worldPos, string text, Color color, float size = 16f)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!SunderedCrown.Core.GameSettings.ShowFloatingText) return; // accessibility toggle
            if (_driver == null)
            {
                var go = new GameObject("FloatingCombatText");
                Object.DontDestroyOnLoad(go);
                _driver = go.AddComponent<Driver>();
            }
            _driver.Add(new Popup
            {
                world = worldPos + Vector3.up * 0.9f,
                jitterX = Random.Range(-18f, 18f),
                text = text, color = color, size = size,
                bornAt = Time.time, life = 1.1f,
            });
        }

        private struct Popup
        {
            public Vector3 world;
            public float jitterX;
            public string text;
            public Color color;
            public float size;
            public float bornAt, life;
        }

        private class Driver : MonoBehaviour
        {
            private readonly List<Popup> _popups = new List<Popup>();
            public void Add(Popup p) => _popups.Add(p);

            void OnGUI()
            {
                var cam = Camera.main;
                if (cam == null) return;

                for (int i = _popups.Count - 1; i >= 0; i--)
                {
                    var p = _popups[i];
                    float age = Time.time - p.bornAt;
                    if (age >= p.life) { _popups.RemoveAt(i); continue; }

                    Vector3 sp = cam.WorldToScreenPoint(p.world);
                    if (sp.z < 0) continue;

                    float t = age / p.life;
                    float x = sp.x + p.jitterX;
                    float y = Screen.height - sp.y - t * 46f;        // drift upward
                    float alpha = t < 0.7f ? 1f : Mathf.InverseLerp(1f, 0.7f, t); // fade out late

                    var style = new GUIStyle(GUI.skin.label)
                    { alignment = TextAnchor.MiddleCenter, fontSize = Mathf.RoundToInt(p.size), fontStyle = FontStyle.Bold };
                    var rect = new Rect(x - 60, y - 12, 120, 24);

                    var prev = GUI.color;
                    GUI.color = new Color(0f, 0f, 0f, alpha * 0.6f);       // cheap shadow for legibility
                    GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), p.text, style);
                    GUI.color = new Color(p.color.r, p.color.g, p.color.b, alpha);
                    GUI.Label(rect, p.text, style);
                    GUI.color = prev;
                }
            }
        }
    }
}
