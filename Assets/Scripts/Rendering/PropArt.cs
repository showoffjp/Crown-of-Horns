using System.Collections.Generic;
using UnityEngine;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Maps a marker's display label onto one of the painted world props in
    /// <c>Resources/Props</c> (tools/gen-props.py).
    /// <para>
    /// Markers that are not people — chests, doors, stairways, braziers, notice
    /// boards, rifts — have no portrait, so <see cref="WorldArt.Portrait"/> found
    /// nothing for them and they stayed tinted cubes. Their labels are written for
    /// the player ("Ash-Caked Chest", "Cinderhaunt Stairs", "The Skip — a
    /// shimmering rift"), so the label itself says what the thing is; this reads it.
    /// </para>
    /// <para>
    /// Rules are checked in order and the first hit wins, so put the specific ones
    /// first — "Iron Door (use Cinderhaunt Key)" has to reach "door" before
    /// "Cinderhaunt Stairs" claims anything containing "stair".
    /// </para>
    /// </summary>
    public static class PropArt
    {
        /// <summary>A prop name plus the world height, in units, to draw it at.
        /// One tile is 1.0 wide by 0.5 high, so a chest at 0.7 sits about a tile
        /// and a half tall on screen — readable without swallowing its neighbours.</summary>
        public readonly struct Prop
        {
            public readonly string Name;
            public readonly float Height;
            public Prop(string name, float height) { Name = name; Height = height; }
            public bool Valid => !string.IsNullOrEmpty(Name);
        }

        private static readonly (string[] Keys, string Prop, float Height)[] Rules =
        {
            // Order matters: the first hit wins, so the narrow rules come first.
            // "The Stilled Maw" is a dead boss, not a fight, and has to reach
            // brazier_cold before the battle rule claims every "maw".

            // --- containers ---------------------------------------------------
            (new[] { "chest", "strongbox", "coffer", "reliquary", "lockbox", "casket", "cache" }, "chest", 0.46f),
            (new[] { "barrel", "crate", "keg", "sack", "supplies" }, "barrel", 0.52f),

            // --- a fight lives here ---------------------------------------------
            (new[] { "cold brazier", "stilled", "cold ash" }, "brazier_cold", 0.82f),
            (new[] { "(battle)", "miniboss", "hold the line", "face the", "break the",
                     "defend", "unbound maw", "cell gathers", "damnation-rite" }, "battle", 0.82f),

            // --- ways through ---------------------------------------------------
            (new[] { "door", "gate", "portal", "hatch", "postern", "(enter)", "almshouse" }, "door", 1.20f),
            (new[] { "stairs up", "climb back", "back to", "back out", "step back",
                     "leave the", "way out", "up to", "ride the wreckage" }, "stairs_up", 0.52f),
            (new[] { "stair", "steps", "descend", "climb", "down to", "down into" }, "stairs_down", 0.52f),

            // --- fire -------------------------------------------------------------
            (new[] { "brazier", "pyre", "torch", "candle" }, "brazier", 0.88f),
            (new[] { "campfire", "camp", "fire (", "hearth", "rest & talk" }, "campfire", 0.54f),

            // --- written things -----------------------------------------------------
            (new[] { "board", "notice", "broadside", "slogan", "poster", "proclamation", "pinned" }, "board", 0.88f),
            (new[] { "ledger", "book", "tome", "accounts", "record", "codex", "scroll" }, "ledger", 0.74f),

            // --- the grey -------------------------------------------------------------
            (new[] { "wall of the faithless", "wall of names", "the wall" }, "wall", 1.18f),
            (new[] { "rift", "tear", "skip", "spellplague", "god-wound", "shimmer",
                     "breach", "falling sky" }, "rift", 0.96f),
            (new[] { "knotted rope", "knotted cord", "memorial of knotted" }, "bones", 0.88f),
            (new[] { "shrine", "memorial", "altar", "grave", "tomb", "cairn" }, "shrine", 0.88f),
            (new[] { "banner", "standard", "flag", "pennant" }, "banner", 1.16f),
            (new[] { "statue", "idol", "effigy", "monument", "colossus" }, "statue", 1.20f),

            // --- people-shaped scenery ------------------------------------------------
            (new[] { "onlookers", "crowd", "gawking", "knot of", "gathering", "congregation" }, "crowd", 0.74f),
            (new[] { "hooded", "figure", "watchful", "stranger", "a robed", "sympathizer" }, "figure", 1.02f),
            (new[] { "corpse", "the fallen", "remains", "lie where they fell", "dead body" }, "corpse", 0.32f),

            // --- places -----------------------------------------------------------------
            (new[] { "market", "sundries", "fence", "stall", "wares", "goods", "shop", "merchant" }, "market", 0.82f),
            (new[] { "docks", "waterfront", "streets", "niche", "safehouse", "quarter",
                     "court of", "taproom" }, "signpost", 0.98f),
            (new[] { "chairs", "corner with", "table" }, "chairs", 0.58f),
            (new[] { "debris", "wreckage", "rubble", "tide-line", "ruin" }, "debris", 0.36f),
        };

        private static readonly Dictionary<string, Prop> _cache = new Dictionary<string, Prop>();

        /// <summary>The prop this label describes, or an invalid Prop if none fits.</summary>
        public static Prop Match(string label)
        {
            if (string.IsNullOrEmpty(label)) return default;
            if (_cache.TryGetValue(label, out var hit)) return hit;

            string s = label.ToLowerInvariant();
            var found = default(Prop);
            foreach (var rule in Rules)
            {
                bool any = false;
                foreach (var k in rule.Keys) if (s.Contains(k)) { any = true; break; }
                if (!any) continue;
                found = new Prop(rule.Prop, rule.Height);
                break;
            }
            _cache[label] = found;
            return found;
        }

        /// <summary>Loads Resources/Props/&lt;name&gt;, caching misses too.</summary>
        public static Sprite Load(string prop)
        {
            if (string.IsNullOrEmpty(prop)) return null;
            return WorldArt.FromFolder("Props", prop);
        }
    }
}
