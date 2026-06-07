using UnityEngine;
using SunderedCrown.Core;

namespace SunderedCrown.UI
{
    /// <summary>
    /// ACCESSIBILITY: applies <see cref="GameSettings.UiScale"/> to the shared built-in IMGUI skin so every
    /// OnGUI screen in the game gets larger or smaller body text without each one having to opt in. It works
    /// by caching each style's base font size once, then resetting the live skin to base * scale at the start
    /// of each frame's GUI pass. Inline &lt;size=&gt; tags (used for headers) still override, so headings stay
    /// proportional. Drop one instance on the bootstrap object; it never draws anything itself.
    /// </summary>
    public class UiScaler : MonoBehaviour
    {
        private bool _cached;
        private int _label, _button, _box, _toggle, _textField, _textArea, _window;
        private float _appliedScale = -1f;

        void OnGUI()
        {
            var s = GUI.skin;
            if (s == null) return;

            if (!_cached)
            {
                _label     = Fallback(s.label.fontSize, 12);
                _button    = Fallback(s.button.fontSize, 12);
                _box       = Fallback(s.box.fontSize, 12);
                _toggle    = Fallback(s.toggle.fontSize, 12);
                _textField = Fallback(s.textField.fontSize, 12);
                _textArea  = Fallback(s.textArea.fontSize, 12);
                _window    = Fallback(s.window.fontSize, 12);
                _cached = true;
            }

            float scale = Mathf.Clamp(GameSettings.UiScale, 0.85f, 1.6f);
            // Re-apply every frame: the built-in skin is shared global state that other components may touch.
            s.label.fontSize     = Scaled(_label, scale);
            s.button.fontSize    = Scaled(_button, scale);
            s.box.fontSize       = Scaled(_box, scale);
            s.toggle.fontSize    = Scaled(_toggle, scale);
            s.textField.fontSize = Scaled(_textField, scale);
            s.textArea.fontSize  = Scaled(_textArea, scale);
            s.window.fontSize    = Scaled(_window, scale);
            _appliedScale = scale;
        }

        private static int Fallback(int v, int def) => v > 0 ? v : def;
        private static int Scaled(int baseSize, float scale) => Mathf.RoundToInt(baseSize * scale);
    }
}
