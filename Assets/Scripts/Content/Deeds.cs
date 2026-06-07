using System;
using System.Collections.Generic;
using SunderedCrown.Core;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Deeds — lightweight achievements, derived purely from the flags the game already sets (no new
    /// hooks). Each is a title + a predicate over GameFlags; the Chronicle (C) shows how many you've
    /// earned and which. A quiet meta-layer that rewards thoroughness and marks the shape of a run.
    /// </summary>
    public static class Deeds
    {
        public class Deed
        {
            public string id;
            public string title;
            public string desc;
            public Func<GameFlags, bool> earned;
        }

        private static bool All(GameFlags f, params string[] keys)
        {
            foreach (var k in keys) if (!f.GetBool(k)) return false;
            return true;
        }
        private static bool Any(GameFlags f, params string[] keys)
        {
            foreach (var k in keys) if (f.GetBool(k)) return true;
            return false;
        }

        public static readonly List<Deed> All_ = new List<Deed>
        {
            new Deed { id="returned", title="The Returned", desc="Walk back out of death and escape the Cinderhaunt.",
                earned = f => f.GetBool("prologue.cleared") },
            new Deed { id="tea", title="Tea with a Heretic", desc="Sit down with Aldric Morn.",
                earned = f => f.GetBool("aldric.met") },
            new Deed { id="ages", title="Walker of Ages", desc="Walk all four eras of history.",
                earned = f => All(f, "netheril.cleared", "crownwars.cleared", "act4.toot_done", "act4.spellplague_done") },
            new Deed { id="minibosses", title="Four Foes for Four Ages", desc="Put down every era's optional miniboss.",
                earned = f => All(f, "netheril.boss_down", "crownwars.boss_down", "toot.avatar_down", "spellplague.herald_down") },
            new Deed { id="verdict", title="The Verdict, Argued Down", desc="Talk an ancient court out of a damnation.",
                earned = f => f.GetBool("crownwars.verdict_spared") },
            new Deed { id="threads", title="Five Threads Pulled", desc="Resolve every companion's personal quest.",
                earned = f => All(f, "quest.roen.resolved", "quest.varra.resolved", "quest.garrow.resolved", "quest.naeve.resolved", "quest.ilfaeril.resolved") },
            new Deed { id="beloved", title="Beloved", desc="See a romance through to the last night.",
                earned = f => Any(f, "romance.garrow.consummated", "romance.roen.consummated", "romance.naeve.consummated", "romance.varra.consummated") },
            new Deed { id="heartbreak", title="The Bond That Broke", desc="Drive a companion out of the company for good.",
                earned = f => Any(f, "companion.garrow.left", "companion.roen.left", "companion.varra.left", "companion.naeve.left", "companion.ilfaeril.left") },
            new Deed { id="quarter", title="The Quarter's Champion", desc="Earn the Lower City's pledge.",
                earned = f => f.GetBool("lowcity.allies") },
            new Deed { id="doomguide_friend", title="Marked as a Question", desc="Earn the high regard of Kelemvor's Doomguides.",
                earned = f => f.GetInt("faction.kelemvor.reputation") >= 5 },
            new Deed { id="choir_voice", title="The Choir Sings Your Name", desc="Earn the high regard of the Faithless Choir.",
                earned = f => f.GetInt("faction.choir.reputation") >= 5 },
            new Deed { id="reader", title="Reader of the Lady", desc="Read the Lady in the Margins' true name.",
                earned = f => f.GetBool("readers_boon") },
            new Deed { id="twice_told", title="Twice-Told", desc="Begin the saga again, on a New Game+.",
                earned = f => f.GetBool("ng.plus") },
            new Deed { id="keepsakes", title="Keeper of Mementos", desc="Carry five keepsakes at once.",
                earned = f => Keepsakes.Earned().Count >= 5 },
            new Deed { id="ended", title="The Saga Ended", desc="Reach the Court of the Dead and choose.",
                earned = f => f.GetBool("game.ended") },
            new Deed { id="golden", title="The Golden Road", desc="Earn — and choose — one of the golden endings.",
                earned = f => f.GetBool("game.ended") && EndingResolver.IsGolden((Ending)f.GetInt("game.ending")) },
            new Deed { id="hard_way", title="The Hard Way", desc="Reach the Court of the Dead on Hard difficulty.",
                earned = f => f.GetBool("game.ended") && GameSettings.Mode == GameSettings.Difficulty.Hard },
            new Deed { id="hunter", title="Hunter of the Returned", desc="Lay low twenty-five foes across the ages.",
                earned = f => f.GetInt("slain.total") >= 25 },
            new Deed { id="veteran", title="Veteran", desc="Win fifteen battles.",
                earned = f => f.GetInt("combat.victories") >= 15 },
            new Deed { id="clutch", title="Pulled From the Brink", desc="Win a battle in which a companion went down.",
                earned = f => f.GetBool("combat.clutch_win") },
            new Deed { id="dealer", title="Wheeler-Dealer", desc="Both buy from and sell to a Lower City vendor.",
                earned = f => All(f, "shop.bought", "shop.sold") },
            new Deed { id="whole_company", title="The Whole Company", desc="Gather every companion the saga offers.",
                earned = f => All(f, "companion.roen.recruited", "companion.varra.recruited", "companion.naeve.recruited", "companion.ilfaeril.recruited", "companion.maerin.recruited") },
            new Deed { id="gentle", title="The Gentle Returned", desc="Show the Lower City every mercy it asked of you.",
                earned = f => All(f, "quest.widow.hope", "quest.fist.freed", "quest.tithe.freed", "quest.choir.doubted") },
            new Deed { id="small_mercies", title="Small Mercies", desc="Pull a stranger from the river and buy a child's hand back from the Fist.",
                earned = f => All(f, "docks.ferryman_saved", "market.urchin_freed") },
            new Deed { id="conscience", title="The Quarter's Conscience", desc="Face every hard small choice the Lower City put before you.",
                earned = f => All(f, "docks.ferryman_resolved", "market.urchin_resolved", "almshouse.deathbed_resolved", "safehouse.informant_resolved") },
            new Deed { id="wanderer", title="Knew Every Door", desc="Walk all three rooms of the Lower City — almshouse, market, and docks.",
                earned = f => All(f, "lowcity.visited_almshouse", "lowcity.visited_market", "lowcity.visited_docks") },
            new Deed { id="tactician", title="Master Tactician", desc="Use every tactical action — Dodge, Dash, Help, Disengage, and Shove.",
                earned = f => All(f, "combat.used_dodge", "combat.used_dash", "combat.used_help", "combat.used_disengage", "combat.used_shove") },
            new Deed { id="pincer", title="Pincer Movement", desc="Land a flanking attack with an ally opposite a foe.",
                earned = f => f.GetBool("combat.used_flank") },
            new Deed { id="confidant", title="Trusted at the Niche", desc="Be welcomed into the Harper safehouse.",
                earned = f => f.GetBool("lowcity.visited_safehouse") },
            new Deed { id="loremaster", title="Loremaster", desc="Fill thirty entries of the Codex.",
                earned = f => CodexContent.Known().Count >= 30 },
            new Deed { id="archivist", title="Archivist of the Returned", desc="Fill fifty entries of the Codex.",
                earned = f => CodexContent.Known().Count >= 50 },
            new Deed { id="heart_company", title="Heart of the Company", desc="Hear every companion open up at the fire.",
                earned = f => All(f, "camp.nighttalk.garrow.done", "camp.nighttalk.roen.done", "camp.nighttalk.varra.done",
                                     "camp.nighttalk.naeve.done", "camp.nighttalk.ilfaeril.done", "camp.nighttalk.maerin.done") },
        };

        public static int Total => All_.Count;
        public static int EarnedCount()
        {
            var f = GameFlags.Current; int n = 0;
            foreach (var d in All_) if (d.earned(f)) n++;
            return n;
        }
    }
}
