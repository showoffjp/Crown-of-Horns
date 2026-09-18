using System.Collections.Generic;
using UnityEngine;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Optional art loader. Looks up a sprite by name in <c>Assets/Resources/Sprites/&lt;name&gt;</c>
    /// (imported as a Sprite). Returns null when no art pack is present, so the game runs fine with
    /// placeholder cubes — and *automatically* shows portraits/units the moment you drop in art named
    /// after a character (e.g. <c>Sprites/Sable.png</c>, <c>Sprites/Naeve.png</c>). See
    /// docs/ASSET_INTEGRATION.md.
    /// </summary>
    public static class WorldArt
    {
        private static readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        public static Sprite Sprite(string name) => FromFolder("Sprites", name);

        /// <summary>Loads <c>Resources/&lt;folder&gt;/&lt;name&gt;</c> as a Sprite, caching
        /// misses too so a name with no art doesn't hit Resources every frame.</summary>
        public static Sprite FromFolder(string folder, string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(folder)) return null;
            string key = folder + "/" + name;
            if (_cache.TryGetValue(key, out var cached)) return cached;
            var sp = Resources.Load<Sprite>(key);
            _cache[key] = sp;
            return sp;
        }

        /// <summary>A world standee for a soul: the portrait's figure, cut out on
        /// transparency, with a bottom-centre pivot (tools/gen-standees.py). Falls
        /// back to the speaker's first word so "Doomguide Knight" finds "Doomguide".
        /// <para>
        /// This is what world markers should use. A dialogue portrait is a 320x400
        /// opaque card with a painted backdrop; stood on a floor tile it reads as a
        /// framed picture hovering over the ground rather than a person.
        /// </para></summary>
        public static Sprite Standee(string speaker)
        {
            if (string.IsNullOrEmpty(speaker)) return null;
            string key = "standee:" + speaker;
            if (_cache.TryGetValue(key, out var cached)) return cached;

            string first = speaker;
            int sp = speaker.IndexOf(' ');
            if (sp > 0) first = speaker.Substring(0, sp);

            var art = FromFolder("Standees", speaker) ?? FromFolder("Standees", first);
            _cache[key] = art;
            return art;
        }

        /// <summary>A dialogue portrait for a speaker: <c>Resources/Portraits/&lt;name&gt;</c> first, then
        /// the map sprite, then the speaker's first word — so one portrait can cover "Doomguide Knight",
        /// "Doomguide Enforcer", … . Returns null (no portrait) when no art is present. Cached.</summary>
        public static Sprite Portrait(string speaker)
        {
            if (string.IsNullOrEmpty(speaker)) return null;
            string key = "portrait:" + speaker;
            if (_cache.TryGetValue(key, out var cached)) return cached;

            string first = speaker;
            int sp = speaker.IndexOf(' ');
            if (sp > 0) first = speaker.Substring(0, sp);

            var art = Resources.Load<Sprite>("Portraits/" + speaker)
                   ?? Resources.Load<Sprite>("Portraits/" + first)
                   ?? Sprite(speaker)
                   ?? Sprite(first);
            _cache[key] = art;
            return art;
        }
    }
}
