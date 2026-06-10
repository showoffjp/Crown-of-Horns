using UnityEngine;

namespace SunderedCrown.Core
{
    /// <summary>
    /// Player options, persisted across sessions via PlayerPrefs and applied immediately. Three knobs:
    /// ambient party banter on/off, master volume, and dialogue text speed (chars/second, or instant).
    /// Pure static state with Load/Save/Apply — the SettingsScreen edits it, and AmbientBanter /
    /// DialogueScreen read it. Loads once on first access.
    /// </summary>
    public static class GameSettings
    {
        public enum Difficulty { Story, Normal, Hard }

        private const string K_Banter  = "settings.banter";
        private const string K_Volume  = "settings.volume";
        private const string K_Speed   = "settings.textspeed";
        private const string K_Instant = "settings.textinstant";
        private const string K_Diff    = "settings.difficulty";
        private const string K_UiScale = "settings.uiscale";
        private const string K_CombatSpeed = "settings.combatspeed";
        private const string K_FloatText = "settings.floattext";
        private const string K_CamFocus = "settings.camfocus";
        private const string K_HitChance = "settings.hitchance";
        private const string K_ConfirmEnd = "settings.confirmendturn";
        private const string K_AutoEnd = "settings.autoendturn";
        private const string K_Autosave = "settings.autosave";
        private const string K_ScreenShake = "settings.screenshake";

        public static bool       BanterEnabled       { get; set; } = true;
        public static float      MasterVolume        { get; set; } = 0.8f;   // 0..1
        public static float      TextCharsPerSecond  { get; set; } = 45f;    // used when !InstantText
        public static bool       InstantText         { get; set; } = false;
        public static Difficulty Mode                { get; set; } = Difficulty.Normal;
        public static float      UiScale             { get; set; } = 1f;     // 0.85..1.6 — accessibility text size
        public static float      CombatSpeed         { get; set; } = 1f;     // 0.5..2.5 — enemy-turn pacing multiplier
        public static bool       ShowFloatingText    { get; set; } = true;   // accessibility: pop combat numbers/words
        public static bool       CameraAutoFocus     { get; set; } = true;   // glide the camera to the active combatant
        public static bool       ShowHitChance       { get; set; } = true;   // QoL: show the hit/crit/damage forecast on targets
        public static bool       ConfirmEndTurn      { get; set; } = true;   // QoL: warn before ending a turn with actions unspent
        public static bool       AutoEndTurn         { get; set; } = false;  // QoL: end the turn automatically once a unit is spent
        public static bool       AutosaveEnabled     { get; set; } = true;   // QoL: write the campaign autosave at checkpoints
        public static bool       ScreenShake         { get; set; } = true;   // accessibility: camera shake on big hits

        /// <summary>A delay (seconds) scaled by the combat-speed setting — higher speed = shorter waits.
        /// Used to pace enemy turns so combat can be sped up or slowed for comfort.</summary>
        public static float CombatDelay(float baseSeconds) => baseSeconds / Mathf.Clamp(CombatSpeed, 0.5f, 2.5f);

        /// <summary>Multiplier on damage enemies deal to the party (Story softens, Hard sharpens).</summary>
        public static float EnemyDamageMult => Mode switch
        { Difficulty.Story => 0.6f, Difficulty.Hard => 1.4f, _ => 1f };

        /// <summary>Multiplier on damage the party deals (Story gives the player a little more punch).</summary>
        public static float PlayerDamageMult => Mode switch
        { Difficulty.Story => 1.35f, Difficulty.Hard => 0.9f, _ => 1f };

        /// <summary>Multiplier on enemy max HP (Story makes foes frailer, Hard makes them spongier).</summary>
        public static float EnemyHpMult => Mode switch
        { Difficulty.Story => 0.8f, Difficulty.Hard => 1.3f, _ => 1f };

        public static string UiScaleBlurb =>
            UiScale <= 0.9f  ? "Compact — more on screen at once." :
            UiScale >= 1.45f ? "Extra Large — easiest to read." :
            UiScale >= 1.15f ? "Large — a touch bigger for comfort." :
                               "Standard text size.";

        public static string DifficultyBlurb => Mode switch
        {
            Difficulty.Story => "Story — for the narrative; foes hit soft, you hit hard.",
            Difficulty.Hard  => "Hard — foes hit harder and you hit a touch softer. Tactics matter.",
            _                => "Normal — the intended 5e balance.",
        };

        static GameSettings() => Load();

        public static void Load()
        {
            BanterEnabled      = PlayerPrefs.GetInt(K_Banter, 1) == 1;
            MasterVolume       = PlayerPrefs.GetFloat(K_Volume, 0.8f);
            TextCharsPerSecond = PlayerPrefs.GetFloat(K_Speed, 45f);
            InstantText        = PlayerPrefs.GetInt(K_Instant, 0) == 1;
            Mode               = (Difficulty)Mathf.Clamp(PlayerPrefs.GetInt(K_Diff, (int)Difficulty.Normal), 0, 2);
            UiScale            = Mathf.Clamp(PlayerPrefs.GetFloat(K_UiScale, 1f), 0.85f, 1.6f);
            CombatSpeed        = Mathf.Clamp(PlayerPrefs.GetFloat(K_CombatSpeed, 1f), 0.5f, 2.5f);
            ShowFloatingText   = PlayerPrefs.GetInt(K_FloatText, 1) == 1;
            CameraAutoFocus    = PlayerPrefs.GetInt(K_CamFocus, 1) == 1;
            ShowHitChance      = PlayerPrefs.GetInt(K_HitChance, 1) == 1;
            ConfirmEndTurn     = PlayerPrefs.GetInt(K_ConfirmEnd, 1) == 1;
            AutoEndTurn        = PlayerPrefs.GetInt(K_AutoEnd, 0) == 1;
            AutosaveEnabled    = PlayerPrefs.GetInt(K_Autosave, 1) == 1;
            ScreenShake        = PlayerPrefs.GetInt(K_ScreenShake, 1) == 1;
            Apply();
        }

        public static void Save()
        {
            PlayerPrefs.SetInt(K_Banter, BanterEnabled ? 1 : 0);
            PlayerPrefs.SetFloat(K_Volume, MasterVolume);
            PlayerPrefs.SetFloat(K_Speed, TextCharsPerSecond);
            PlayerPrefs.SetInt(K_Instant, InstantText ? 1 : 0);
            PlayerPrefs.SetInt(K_Diff, (int)Mode);
            PlayerPrefs.SetFloat(K_UiScale, UiScale);
            PlayerPrefs.SetFloat(K_CombatSpeed, CombatSpeed);
            PlayerPrefs.SetInt(K_FloatText, ShowFloatingText ? 1 : 0);
            PlayerPrefs.SetInt(K_CamFocus, CameraAutoFocus ? 1 : 0);
            PlayerPrefs.SetInt(K_HitChance, ShowHitChance ? 1 : 0);
            PlayerPrefs.SetInt(K_ConfirmEnd, ConfirmEndTurn ? 1 : 0);
            PlayerPrefs.SetInt(K_AutoEnd, AutoEndTurn ? 1 : 0);
            PlayerPrefs.SetInt(K_Autosave, AutosaveEnabled ? 1 : 0);
            PlayerPrefs.SetInt(K_ScreenShake, ScreenShake ? 1 : 0);
            PlayerPrefs.Save();
            Apply();
        }

        /// <summary>Push settings into the running game (currently: master volume).</summary>
        public static void Apply()
        {
            AudioListener.volume = Mathf.Clamp01(MasterVolume);
        }
    }
}
