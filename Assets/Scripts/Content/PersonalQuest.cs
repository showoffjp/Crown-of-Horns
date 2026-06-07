using UnityEngine;
using SunderedCrown.Dialogue;

namespace SunderedCrown.Content
{
    /// <summary>
    /// A data-driven description of a companion personal quest, so the same scene/flow code runs every
    /// companion's arc — only the content differs. Holds the three conversations (arrival / reveal /
    /// resolution), the flags that gate the beats, and the on-map labels. The combat itself is built by
    /// the director keyed on <see cref="fightId"/>.
    /// </summary>
    public class PersonalQuest
    {
        public string id;                 // "roen", "varra", …

        // Flags driving the beat state machine.
        public string arrivedFlag;        // set when arrival dialogue plays (suppresses replay)
        public string clearedFlag;        // set by the director on the fight win
        public string resolvedFlag;       // set by the resolution choice

        // Conversations.
        public DialogueGraph arrival;
        public DialogueGraph reveal;      // pre-fight NPC (optional)
        public DialogueGraph resolution;  // post-fight moral choice

        // On-map presentation.
        public Color background = new Color(0.10f, 0.10f, 0.13f);
        public string examineLabel = "A scene";
        [TextArea(2, 5)] public string examineText = "";
        public string revealNpcLabel = "Someone, cornered";
        public string fightId = "personal";
        public string fightLabel = "Fight (battle)";
        public string resolutionNpcLabel = "Make the call";
        public string leaveLabel = "Leave — back to the Gate";
    }
}
