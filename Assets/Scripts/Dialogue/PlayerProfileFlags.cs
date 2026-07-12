using System;
using UnityEngine;
using SunderedCrown.Core;
using SunderedCrown.Stats;
using SunderedCrown.Characters;

namespace SunderedCrown.Dialogue
{
    /// <summary>
    /// The character's chargen choices (gender, faith, alignment, background) plus the derived facts on
    /// the sheet (race, class, ability scores), mirrored into GameFlags under the <c>pc.*</c> convention
    /// so that dialogue can gate on them with the ordinary FlagClause vocabulary the DialogueRunner
    /// already understands — no special-case evaluation in the runner.
    ///
    /// Keys written:
    ///   pc.gender.&lt;Female|Male&gt;      pc.deity.&lt;Kelemvor|None|…&gt;   pc.race.&lt;Human|Tiefling|…&gt;
    ///   pc.class.&lt;Fighter|Cleric|…&gt;   pc.background.&lt;Acolyte|…&gt;     pc.law.&lt;Lawful|Neutral|Chaotic&gt;
    ///   pc.morality.&lt;Good|Neutral|Evil&gt;   pc.score.&lt;Strength|…&gt; (int)
    ///
    /// CALL THIS ONCE at character creation (and again if the build changes), e.g.
    ///   PlayerProfileFlags.Apply(GameFlags.Current, playerSheet,
    ///       new PlayerProfile { gender = "Female", deity = "Kelemvor", law = "Lawful", morality = "Good", background = "Acolyte" });
    /// "None" for deity marks the Faithless — the game's central reactive axis.
    /// </summary>
    public struct PlayerProfile
    {
        public string gender;
        public string deity;      // "None" = Faithless
        public string law;        // Lawful / Neutral / Chaotic
        public string morality;   // Good / Neutral / Evil
        public string background;
    }

    public static class PlayerProfileFlags
    {
        public static void Apply(GameFlags flags, CharacterSheet sheet, PlayerProfile profile)
        {
            if (flags == null) return;

            SetCategory(flags, "pc.gender.", profile.gender);
            SetCategory(flags, "pc.deity.", profile.deity);
            SetCategory(flags, "pc.law.", profile.law);
            SetCategory(flags, "pc.morality.", profile.morality);
            SetCategory(flags, "pc.background.", profile.background);

            if (sheet != null)
            {
                if (sheet.raceDef != null) SetCategory(flags, "pc.race.", sheet.raceDef.raceName);
                if (sheet.classDef != null) SetCategory(flags, "pc.class.", sheet.classDef.className);
                if (sheet.abilities != null)
                    foreach (Ability a in Enum.GetValues(typeof(Ability)))
                        flags.SetInt("pc.score." + a, sheet.abilities.Get(a));
            }
        }

        private static void SetCategory(GameFlags flags, string prefix, string value)
        {
            if (!string.IsNullOrEmpty(value)) flags.SetBool(prefix + value, true);
        }
    }
}
