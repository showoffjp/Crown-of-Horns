using System.Collections.Generic;
using UnityEngine;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Companion personal-quest hooks. After a campfire night-talk opens a companion up, a pointer to
    /// their Act II personal quest appears in the hub — a place, a rumour, a thread to pull. These are
    /// the teasers (examine markers that set a quest.&lt;id&gt;.started flag); the full quests are the
    /// next content layer. Pure data so the hub just places whatever's eligible.
    /// </summary>
    public static class CompanionQuests
    {
        public class Hook
        {
            public string id;            // companion id; gate is camp.nighttalk.<id>.done
            public string label;         // marker name in the hub
            public Vector2Int coord;     // where it sits
            public string startedFlag;   // quest.<id>.started
            public string tease;         // examine text shown on first interact
            public bool playable;        // true if a PersonalQuestScene exists for this companion
            public string followLabel;   // hub marker text for entering the playable quest
            public string resolvedFlag => $"quest.{id}.resolved";
        }

        public static readonly List<Hook> Hooks = new List<Hook>
        {
            new Hook {
                id = "roen", label = "A folded Harper cipher (Roen's quest)", coord = new Vector2Int(11, 9),
                startedFlag = "quest.roen.started", playable = true,
                followLabel = "Follow the cipher — the silent safehouse (Roen)",
                tease = "A cipher slip, pressed into your hand by a child who ran before you could ask. Decoded, it " +
                        "names a Harper safehouse gone silent — and the person Roen swore he'd pull back from the " +
                        "dark, if returning were possible. He didn't tell you they were still alive to lose. " +
                        "[Roen's personal quest: \"The Honest Lie\" — begins in Act II.]" },
            new Hook {
                id = "varra", label = "A wax-sealed invoice (Varra's quest)", coord = new Vector2Int(13, 9),
                startedFlag = "quest.varra.started", playable = true,
                followLabel = "Answer the invoice — the deconsecrated chapel (Varra)",
                tease = "An envelope sealed in black wax, addressed to Varra in a hand that predates her birth. No " +
                        "postage. No sender. Inside, a single line: ACCOUNT MATURING. Her patron is calling in the " +
                        "contract early — and the only way to renegotiate runs through the one who signed it for her " +
                        "when she was six. [Varra's personal quest: \"The Bill Comes Due\" — begins in Act II.]" },
            new Hook {
                id = "garrow", label = "A Doomguide's summons (Garrow's quest)", coord = new Vector2Int(9, 9),
                startedFlag = "quest.garrow.started", playable = true,
                followLabel = "Answer the summons — the heresy tribunal (Garrow)",
                tease = "A black-edged writ from the temple of Kelemvor: Sister Garrow is summoned to answer for " +
                        "heresy. She left a god over the Wall, and gods keep ledgers too. She'll go — but whether " +
                        "she goes to recant or to burn it down is, she says, \"rather up to what we find in there.\" " +
                        "[Garrow's personal quest: \"A God-Shaped Hole\" — begins in Act II.]" },
            new Hook {
                id = "naeve", label = "A shard of dead Weave (Naeve's quest)", coord = new Vector2Int(7, 9),
                startedFlag = "quest.naeve.started", playable = true,
                followLabel = "Follow the resonance — the falling fragment (Naeve)",
                tease = "A sliver of crystal that should be inert — and instead hums with a frequency Naeve hasn't " +
                        "heard since the sky fell. Somewhere in the present, a fragment of Netheril survived the " +
                        "fall, and it is calling its last daughter home. [Naeve's personal quest: \"After the Sky " +
                        "Fell\" — begins in Act II.]" },
            new Hook {
                id = "ilfaeril", label = "An elven reliquary (Ilfaeril's quest)", coord = new Vector2Int(5, 7),
                startedFlag = "quest.ilfaeril.started", playable = true,
                followLabel = "Open the reliquary — the forgotten name (Ilfaeril)",
                tease = "A reliquary of ancient make, its lock keyed to a name no living elf remembers — except " +
                        "Ilfaeril, who cast the vote that damned its owner. The soul he helped unmake left one " +
                        "thing behind, and it has waited ten thousand years for him to find the courage to open it. " +
                        "[Ilfaeril's personal quest: \"The Vote\" — begins in Act II.]" },
        };
    }
}
