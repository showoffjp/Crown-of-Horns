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

        public static Sprite Sprite(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (_cache.TryGetValue(name, out var cached)) return cached;
            var sp = Resources.Load<Sprite>("Sprites/" + name);
            _cache[name] = sp; // cache the miss too, so we don't hit Resources every frame
            return sp;
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
