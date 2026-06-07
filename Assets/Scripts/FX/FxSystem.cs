using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.FX
{
    /// <summary>
    /// Fire-and-forget visual effects. Drop frame sprites into a Resources folder named
    /// <c>Assets/Resources/FX/&lt;effect&gt;/</c> (any per-frame sprite names — they're
    /// sorted naturally) and call <see cref="PlayAt"/>. If the effect folder is missing,
    /// it no-ops gracefully, so the game runs with or without art.
    ///
    /// See docs/ASSET_INTEGRATION.md for the exact folder layout and the pixel-FX pack
    /// mapping.
    /// </summary>
    public static class FxSystem
    {
        private static readonly Dictionary<string, Sprite[]> _cache = new Dictionary<string, Sprite[]>();

        /// <summary>Spawn an effect at a world position. Returns the GameObject (or null if no frames).</summary>
        public static GameObject PlayAt(string effect, Vector3 position, float fps = 16f, float scale = 1f)
        {
            if (string.IsNullOrEmpty(effect)) return null;
            var frames = Load(effect);
            if (frames == null || frames.Length == 0) return null;

            var go = new GameObject("FX_" + effect);
            go.transform.position = position + new Vector3(0, 0, -1f);
            go.transform.localScale = Vector3.one * scale;
            var anim = go.AddComponent<FxAnimator>();
            anim.frames = frames;
            anim.fps = fps;
            return go;
        }

        /// <summary>
        /// Convenience: pick a sensible default impact effect for a damage type, falling
        /// back to a generic "impact" folder. Lets abilities show VFX without authoring.
        /// </summary>
        public static void PlayImpact(DamageType type, Vector3 position)
        {
            string name = type switch
            {
                DamageType.Fire      => "impact_fire",
                DamageType.Cold      => "impact_ice",
                DamageType.Lightning => "impact_lightning",
                DamageType.Radiant   => "impact_holy",
                DamageType.Necrotic  => "impact_dark",
                DamageType.Acid      => "impact_acid",
                DamageType.Poison    => "impact_poison",
                _                    => "impact"
            };
            // Try the specific effect; fall back to the generic one.
            if (PlayAt(name, position) == null && name != "impact")
                PlayAt("impact", position);
        }

        private static Sprite[] Load(string effect)
        {
            if (_cache.TryGetValue(effect, out var cached)) return cached;
            var sprites = Resources.LoadAll<Sprite>("FX/" + effect);
            System.Array.Sort(sprites, (a, b) => NaturalCompare(a.name, b.name));
            _cache[effect] = sprites;
            return sprites;
        }

        /// <summary>Sort "2" before "10" by comparing embedded numbers, not ASCII.</summary>
        private static int NaturalCompare(string a, string b)
        {
            int ia = ExtractNumber(a), ib = ExtractNumber(b);
            if (ia != ib) return ia.CompareTo(ib);
            return string.CompareOrdinal(a, b);
        }

        private static int ExtractNumber(string s)
        {
            int n = 0; bool any = false;
            foreach (char c in s)
            {
                if (char.IsDigit(c)) { n = n * 10 + (c - '0'); any = true; }
                else if (any) break;
            }
            return any ? n : 0;
        }
    }
}
