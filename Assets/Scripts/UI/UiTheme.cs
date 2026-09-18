using UnityEngine;

namespace SunderedCrown.UI
{
    /// <summary>
    /// Gives every IMGUI screen in the game one look.
    /// <para>
    /// All ~25 screens draw with <c>OnGUI</c> against Unity's built-in skin, which is
    /// the flat grey editor chrome — so the HUD, the menu, dialogue and every panel
    /// read as an unstyled debug overlay sitting on top of the world.
    /// </para>
    /// <para>
    /// Rather than restyle twenty-five files, this repaints the shared skin itself.
    /// <c>GUI.skin</c> is a single global object; mutating its styles carries to every
    /// component that draws afterwards, which is the same mechanism
    /// <see cref="UiScaler"/> already uses for text size. Panels become dark violet
    /// with a dull-gold hairline, buttons get three real states, and body text goes
    /// parchment — all from textures generated here, so it costs no assets.
    /// </para>
    /// <para>
    /// Only colour, background and padding are touched. Font sizes are left alone so
    /// the accessibility scale stays in charge of them.
    /// </para>
    /// </summary>
    public static class UiTheme
    {
        // The grey's palette, shared with the web build.
        private static readonly Color Ink       = new Color(0.086f, 0.075f, 0.110f, 0.94f); // panel fill
        private static readonly Color InkLift   = new Color(0.133f, 0.118f, 0.165f, 0.94f); // panel fill, top
        private static readonly Color Edge      = new Color(0.039f, 0.031f, 0.055f, 1.00f); // outer keyline
        private static readonly Color Gilt      = new Color(0.545f, 0.451f, 0.243f, 1.00f); // dull gold rule
        private static readonly Color GiltBright= new Color(0.788f, 0.663f, 0.376f, 1.00f);
        private static readonly Color Parchment = new Color(0.898f, 0.875f, 0.816f, 1.00f);

        private static readonly Color BtnFill   = new Color(0.141f, 0.122f, 0.176f, 0.96f);
        private static readonly Color BtnHover  = new Color(0.208f, 0.180f, 0.251f, 0.98f);
        private static readonly Color BtnActive = new Color(0.078f, 0.067f, 0.102f, 0.98f);

        private const int Slice = 8;     // 9-slice border, in pixels
        private static bool _applied;

        /// <summary>Repaint the shared skin. Idempotent and cheap after the first call,
        /// so it is safe to call from every frame's GUI pass.</summary>
        public static void Apply()
        {
            if (_applied) return;
            var s = GUI.skin;
            if (s == null) return;

            var panel  = Panel(Ink, InkLift, Gilt);
            var button = Panel(BtnFill, Lift(BtnFill), Gilt);
            var hover  = Panel(BtnHover, Lift(BtnHover), GiltBright);
            var active = Panel(BtnActive, BtnActive, Gilt);
            var field  = Panel(new Color(0.055f, 0.047f, 0.071f, 0.98f),
                               new Color(0.075f, 0.063f, 0.094f, 0.98f), Gilt);

            Dress(s.box, panel, panel, panel, Parchment, new RectOffset(10, 10, 8, 8));
            Dress(s.window, panel, panel, panel, Parchment, new RectOffset(12, 12, 20, 12));
            Dress(s.button, button, hover, active, Parchment, new RectOffset(12, 12, 6, 6));
            Dress(s.textField, field, field, field, Parchment, new RectOffset(7, 7, 4, 4));
            Dress(s.textArea, field, field, field, Parchment, new RectOffset(7, 7, 4, 4));

            s.button.hover.textColor = GiltBright;
            s.button.active.textColor = GiltBright;
            s.label.normal.textColor = Parchment;
            s.label.wordWrap = true;          // long journal lines used to run off the panel
            s.toggle.normal.textColor = Parchment;
            s.toggle.onNormal.textColor = GiltBright;

            _applied = true;
        }

        private static void Dress(GUIStyle style, Texture2D normal, Texture2D hover, Texture2D active,
                                  Color text, RectOffset padding)
        {
            if (style == null) return;
            style.normal.background = normal;
            style.hover.background = hover;
            style.active.background = active;
            style.onNormal.background = active;
            style.onHover.background = hover;
            style.onActive.background = active;
            style.normal.textColor = text;
            style.onNormal.textColor = text;
            style.border = new RectOffset(Slice, Slice, Slice, Slice);
            style.padding = padding;
            style.richText = true;
        }

        private static Color Lift(Color c) =>
            new Color(Mathf.Min(1f, c.r * 1.35f), Mathf.Min(1f, c.g * 1.35f), Mathf.Min(1f, c.b * 1.35f), c.a);

        /// <summary>A 9-sliceable panel: a hard outer keyline, a dull-gold rule inset
        /// one pixel from it, and a soft vertical gradient in the middle.</summary>
        private static Texture2D Panel(Color fill, Color fillTop, Color rule)
        {
            const int N = Slice * 3;                 // 24px: corners never stretch into each other
            var tex = new Texture2D(N, N, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,       // keep the hairline exactly one pixel
                hideFlags = HideFlags.DontSave,
            };
            var px = new Color[N * N];
            for (int y = 0; y < N; y++)
            {
                float t = 1f - y / (float)(N - 1);   // texture y is bottom-up; panels lighten at the top
                var body = Color.Lerp(fill, fillTop, t * t);
                for (int x = 0; x < N; x++)
                {
                    int inset = Mathf.Min(Mathf.Min(x, y), Mathf.Min(N - 1 - x, N - 1 - y));
                    px[y * N + x] = inset == 0 ? Edge : inset == 2 ? rule : body;
                }
            }
            tex.SetPixels(px);
            tex.Apply();
            return tex;
        }
    }

    /// <summary>Drives <see cref="UiTheme.Apply"/> from a GUI pass, which is the only
    /// context where <c>GUI.skin</c> exists. AutoBoot puts one on a DontDestroyOnLoad
    /// object so the title screen is styled too — <see cref="UiScaler"/> is added by
    /// CampaignBootstrap and so only starts once a campaign is running.</summary>
    public class UiThemeApplier : MonoBehaviour
    {
        void OnGUI() => UiTheme.Apply();
    }
}
