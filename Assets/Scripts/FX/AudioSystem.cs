using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Stats;

namespace SunderedCrown.FX
{
    /// <summary>
    /// The audio seam — the sound equivalent of <see cref="FxSystem"/>/<see cref="WorldArt"/>. Plays SFX
    /// one-shots from <c>Resources/SFX/&lt;name&gt;</c> and looping music from <c>Resources/Music/&lt;name&gt;</c>,
    /// and no-ops gracefully when a clip is missing — so the game is silent until you drop audio in, then
    /// *just plays it*. Master volume rides on `AudioListener.volume` (set from GameSettings), so the
    /// options slider already governs everything. Self-bootstraps a hidden, persistent driver.
    /// </summary>
    public static class AudioSystem
    {
        private static AudioSource _sfx, _music;
        private static string _currentMusic;
        private static readonly Dictionary<string, AudioClip> _cache = new Dictionary<string, AudioClip>();

        private static void Ensure()
        {
            if (_sfx != null) return;
            var go = new GameObject("AudioSystem");
            Object.DontDestroyOnLoad(go);
            _sfx = go.AddComponent<AudioSource>(); _sfx.playOnAwake = false;
            _music = go.AddComponent<AudioSource>(); _music.playOnAwake = false; _music.loop = true;
        }

        private static AudioClip Load(string folder, string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            string key = folder + "/" + name;
            if (_cache.TryGetValue(key, out var cached)) return cached;
            var clip = Resources.Load<AudioClip>(key);
            _cache[key] = clip; // cache misses too
            return clip;
        }

        /// <summary>Play a one-shot from Resources/SFX/. No-op if the clip is missing.</summary>
        public static void PlaySfx(string name, float volume = 1f)
        {
            var clip = Load("SFX", name);
            if (clip == null) return;
            Ensure();
            _sfx.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        /// <summary>Impact SFX by damage type: tries <c>hit_&lt;type&gt;</c>, falls back to <c>hit</c>.</summary>
        public static void PlayHit(DamageType type)
        {
            var specific = Load("SFX", "hit_" + SfxName(type));
            if (specific != null) { Ensure(); _sfx.PlayOneShot(specific); return; }
            PlaySfx("hit");
        }

        /// <summary>Swap looping music from Resources/Music/. If the track is missing, the current music
        /// keeps playing (we never cut to silence just because a mode has no dedicated track).</summary>
        public static void PlayMusic(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (_currentMusic == name && _music != null && _music.isPlaying) return;
            var clip = Load("Music", name);
            if (clip == null) return;
            Ensure();
            _currentMusic = name;
            _music.clip = clip;
            _music.Play();
        }

        public static void StopMusic()
        {
            if (_music != null) { _music.Stop(); _currentMusic = null; }
        }

        private static string SfxName(DamageType type) => type switch
        {
            DamageType.Fire => "fire", DamageType.Cold => "ice", DamageType.Lightning => "lightning",
            DamageType.Radiant => "holy", DamageType.Necrotic => "dark", DamageType.Acid => "acid",
            DamageType.Poison => "poison", _ => "physical",
        };
    }
}
