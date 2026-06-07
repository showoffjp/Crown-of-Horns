using System.Collections.Generic;
using SunderedCrown.Characters;

namespace SunderedCrown.Content
{
    /// <summary>
    /// The party's idle chatter. A pool of short banter lines; some are general, some only fire when a
    /// particular companion is in the field (matched loosely by name). Pure data — the AmbientBanter
    /// ticker picks from whatever's eligible given who's currently walking with you.
    /// </summary>
    public static class PartyBanter
    {
        public class Line
        {
            public string speaker;
            public string text;
            public string requires;   // substring that must appear in some active member's name; null = anyone
        }

        public static readonly List<Line> Pool = new List<Line>
        {
            // General
            new Line { speaker = "Roen", text = "Roen: \"You walked out of death. I've walked out of worse weddings. We'll get along.\"", requires = "Roen" },
            new Line { speaker = "Roen", text = "Roen: \"Every lock in this city has a story. Most of them end with me on the right side of it.\"", requires = "Roen" },
            new Line { speaker = "Varra", text = "Varra: \"Do you ever feel watched? No? Lucky you. I've felt watched since I was six.\"", requires = "Varra" },
            new Line { speaker = "Varra", text = "Varra: \"My patron's been quiet lately. That's not comforting. That's the quiet before the bill.\"", requires = "Varra" },
            new Line { speaker = "Garrow", text = "Garrow: \"Keep your shield up and your god closer. Oh — right. You haven't got one. Keep the shield up, then.\"", requires = "Garrow" },
            new Line { speaker = "Naeve", text = "Naeve: \"In my city we had a word for people like you. We had a word for everything. Then the words fell out of the sky.\"", requires = "Naeve" },
            new Line { speaker = "Naeve", text = "Naeve: \"You feel time the way I feel the dead Weave — like a missing tooth your tongue keeps finding.\"", requires = "Naeve" },
            new Line { speaker = "Ilfaeril", text = "Ilfaeril: \"My people voted, once, on whether a soul deserved to exist. I have been ashamed for ten thousand years.\"", requires = "Ilfaeril" },

            // Anyone-eligible reflections
            new Line { speaker = "—", text = "Someone, quietly: \"Do you think the ones in the Wall can hear us out here?\"", requires = null },
            new Line { speaker = "—", text = "A whisper at the edge of hearing — almost a page turning. You decide it was the wind.", requires = null },
            new Line { speaker = "—", text = "Someone hums an old Gate lullaby. It's the one mothers sing so the dead don't take the child.", requires = null },
        };

        /// <summary>A line eligible given the current active party, or null if none fit.</summary>
        public static Line Pick(System.Random rng)
        {
            var party = Party.Instance;
            var eligible = new List<Line>();
            foreach (var line in Pool)
            {
                if (string.IsNullOrEmpty(line.requires)) { eligible.Add(line); continue; }
                if (party == null) continue;
                foreach (var m in party.active)
                    if (m.displayName != null && m.displayName.Contains(line.requires)) { eligible.Add(line); break; }
            }
            if (eligible.Count == 0) return null;
            return eligible[rng.Next(eligible.Count)];
        }
    }
}
