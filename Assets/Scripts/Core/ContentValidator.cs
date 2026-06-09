using System.Collections.Generic;
using SunderedCrown.Content;
using SunderedCrown.Dialogue;
using SunderedCrown.Items;

namespace SunderedCrown.Core
{
    /// <summary>
    /// A runtime content sanity-check. Walks every authored DialogueGraph we can reach (the five personal
    /// quests, the Act II side content, Aldric's tea, the Lady's riddles) and flags broken node references
    /// — a `nextNodeId`/`failNodeId`/`autoNextNodeId`/`startNodeId` that points at a node that doesn't
    /// exist, plus duplicate or empty node ids. This is the bug class the sandbox can't catch (it has no
    /// Unity compiler), so `ValidationDemo` runs it on Play and logs a PASS/FAIL report. Pure logic.
    /// </summary>
    public static class ContentValidator
    {
        public static List<string> Validate(out int graphCount, out int nodeCount)
        {
            var issues = new List<string>();
            var graphs = new List<(string label, DialogueGraph g)>();

            void Add(string label, DialogueGraph g) { if (g != null) graphs.Add((label, g)); }
            void AddQuest(string id, PersonalQuest q)
            {
                if (q == null) { issues.Add($"[{id}] PersonalQuest is null"); return; }
                Add(id + ".arrival", q.arrival);
                Add(id + ".reveal", q.reveal);
                Add(id + ".resolution", q.resolution);
            }

            AddQuest("roen", new RoenQuestContent().Quest);
            AddQuest("varra", new VarraQuestContent().Quest);
            AddQuest("garrow", new GarrowQuestContent().Quest);
            AddQuest("naeve", new NaeveQuestContent().Quest);
            AddQuest("ilfaeril", new IlfaerilQuestContent().Quest);

            foreach (var npc in new ActTwoContent().Npcs)
                Add("act2:" + npc.label, npc.dialogue);

            Add("aldric.tea", new AldricContent().TeaDialogue);
            var riddles = new RiddleContent();
            Add("tally.intro", riddles.TallyIntro);
            Add("tally.roaming", riddles.RoamingRiddle);

            // Reactive era-witness graphs (the others are built inside MonoBehaviour scenes and can't be
            // reached statically; these two are pure static builders, so we can sanity-check them here).
            Add("era.garrow_toot", EraWitness.GarrowTimeOfTroubles());
            Add("era.varra_spellplague", EraWitness.VarraSpellplague());

            // Cross-era echoes (the world naming an upstream choice) — pure static builders, validate them too.
            Add("era.echo_toot", EraEchoes.TimeOfTroubles());
            Add("era.echo_spellplague", EraEchoes.Spellplague());

            graphCount = graphs.Count;
            nodeCount = 0;
            foreach (var (label, g) in graphs)
            {
                nodeCount += g.nodes != null ? g.nodes.Count : 0;
                CheckGraph(label, g, issues);
            }

            CheckData(issues);
            return issues;
        }

        /// <summary>Sanity-check the data-only content (shop ids resolve, Codex/Keepsakes/Deeds non-empty
        /// + unique). Builds SwordCoastContent first so the ItemDatabase is populated.</summary>
        private static void CheckData(List<string> issues)
        {
            var content = new SwordCoastContent(); // registers items into ItemDatabase

            foreach (var o in ShopContent.Stock)
                if (ItemDatabase.Get(o.itemId) == null)
                    issues.Add($"[shop] fence offer '{o.itemId}' does not resolve in ItemDatabase");
            foreach (var o in ShopContent.SmugglerStock)
                if (ItemDatabase.Get(o.itemId) == null)
                    issues.Add($"[shop] smuggler offer '{o.itemId}' does not resolve in ItemDatabase");

            // --- Abilities: data integrity per definition ---
            foreach (var kv in content.Abilities)
            {
                var a = kv.Value;
                if (a == null) { issues.Add($"[ability] '{kv.Key}' is null"); continue; }
                if (a.targeting == SunderedCrown.Characters.TargetingMode.AreaBurst && a.areaRadiusTiles <= 0)
                    issues.Add($"[ability] '{kv.Key}' is AreaBurst but areaRadiusTiles is {a.areaRadiusTiles}");
                if (a.isHeal && string.IsNullOrWhiteSpace(a.healDice) && string.IsNullOrWhiteSpace(a.damageDice))
                    issues.Add($"[ability] '{kv.Key}' is a heal but has no healDice");
                if (a.spellSlotLevel < 0) issues.Add($"[ability] '{kv.Key}' has negative spellSlotLevel");
                if (a.rangeTiles < 0) issues.Add($"[ability] '{kv.Key}' has negative rangeTiles");
            }

            // --- Classes: every per-level kit entry resolves ---
            foreach (var cls in content.Classes)
            {
                if (cls == null) { issues.Add("[class] null ClassDefinition"); continue; }
                if (cls.startingAbilities == null || cls.startingAbilities.Length == 0)
                { issues.Add($"[class] '{cls.className}' has no startingAbilities"); continue; }
                for (int i = 0; i < cls.startingAbilities.Length; i++)
                    if (cls.startingAbilities[i] == null)
                        issues.Add($"[class] '{cls.className}' startingAbilities[{i}] is null");
            }

            // --- Camp group banters: unique ids, both speakers + lines present ---
            var banterIds = new HashSet<string>();
            foreach (var b in CampGroupBanter.All)
            {
                if (string.IsNullOrEmpty(b.id)) { issues.Add("[banter] entry with empty id"); continue; }
                if (!banterIds.Add(b.id)) issues.Add($"[banter] duplicate id '{b.id}'");
                if (string.IsNullOrEmpty(b.aMatch) || string.IsNullOrEmpty(b.bMatch))
                    issues.Add($"[banter] '{b.id}' missing a speaker match");
                if (b.lines == null || b.lines.Length == 0) issues.Add($"[banter] '{b.id}' has no lines");
            }

            // --- Camp night-talks: unique done-keys + lines present ---
            var talkKeys = new HashSet<string>();
            foreach (var t in CampContent.Talks)
            {
                if (string.IsNullOrEmpty(t.DoneKey)) { issues.Add("[nighttalk] entry with empty key"); continue; }
                if (!talkKeys.Add(t.DoneKey)) issues.Add($"[nighttalk] duplicate done-key '{t.DoneKey}'");
                if (t.lines == null || t.lines.Length == 0) issues.Add($"[nighttalk] '{t.DoneKey}' has no lines");
            }

            var codexIds = new HashSet<string>();
            foreach (var e in CodexContent.All)
            {
                if (string.IsNullOrEmpty(e.id) || string.IsNullOrEmpty(e.title)) issues.Add("[codex] entry with empty id/title");
                else if (!codexIds.Add(e.id)) issues.Add($"[codex] duplicate id '{e.id}'");
            }

            var keepIds = new HashSet<string>();
            foreach (var k in Keepsakes.All)
            {
                if (string.IsNullOrEmpty(k.id) || string.IsNullOrEmpty(k.title)) issues.Add("[keepsakes] entry with empty id/title");
                else if (!keepIds.Add(k.id)) issues.Add($"[keepsakes] duplicate id '{k.id}'");
            }

            var deedIds = new HashSet<string>();
            foreach (var d in Deeds.All_)
            {
                if (string.IsNullOrEmpty(d.id) || d.earned == null) issues.Add($"[deeds] '{d.id}' empty id or null predicate");
                else if (!deedIds.Add(d.id)) issues.Add($"[deeds] duplicate id '{d.id}'");
            }
        }

        private static void CheckGraph(string label, DialogueGraph g, List<string> issues)
        {
            if (g.nodes == null || g.nodes.Count == 0) { issues.Add($"[{label}] has no nodes"); return; }

            var ids = new HashSet<string>();
            foreach (var n in g.nodes)
            {
                if (string.IsNullOrEmpty(n.id)) { issues.Add($"[{label}] a node has an empty id"); continue; }
                if (!ids.Add(n.id)) issues.Add($"[{label}] duplicate node id '{n.id}'");
            }

            if (string.IsNullOrEmpty(g.startNodeId) || !ids.Contains(g.startNodeId))
                issues.Add($"[{label}] startNodeId '{g.startNodeId}' does not exist");

            foreach (var n in g.nodes)
            {
                if (!string.IsNullOrEmpty(n.autoNextNodeId) && !ids.Contains(n.autoNextNodeId))
                    issues.Add($"[{label}] node '{n.id}' autoNext → '{n.autoNextNodeId}' (missing)");

                if (n.choices == null) continue;
                foreach (var c in n.choices)
                {
                    if (!string.IsNullOrEmpty(c.nextNodeId) && !ids.Contains(c.nextNodeId))
                        issues.Add($"[{label}] node '{n.id}' choice → '{c.nextNodeId}' (missing)");
                    if (!string.IsNullOrEmpty(c.failNodeId) && !ids.Contains(c.failNodeId))
                        issues.Add($"[{label}] node '{n.id}' failNode → '{c.failNodeId}' (missing)");
                }
            }
        }
    }
}
