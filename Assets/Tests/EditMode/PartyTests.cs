using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SunderedCrown.Characters;

namespace SunderedCrown.Tests.EditMode
{
    /// <summary>
    /// The roster: recruit into roster + field party (capped at maxActive), permanent
    /// removal (the Breach / death / departure), bench/field toggling, and wipe detection.
    /// The list logic is pure, so we drive the component directly without play mode.
    /// </summary>
    public class PartyTests
    {
        private readonly List<Object> _spawned = new List<Object>();

        private Party MakeParty(int maxActive)
        {
            var go = new GameObject("Party");
            _spawned.Add(go);
            var p = go.AddComponent<Party>();
            p.maxActive = maxActive;
            return p;
        }

        private CharacterSheet Member(string name, bool alive = true)
        {
            return new CharacterSheet
            {
                displayName = name,
                maxHitPoints = 10,
                currentHitPoints = alive ? 10 : 0
            };
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        [Test]
        public void Recruit_AddsToRosterAndField()
        {
            var p = MakeParty(4);
            var hero = Member("Hero");
            p.Recruit(hero);
            CollectionAssert.Contains(p.roster, hero);
            CollectionAssert.Contains(p.active, hero);
        }

        [Test]
        public void Recruit_BeyondMaxActive_BenchesExtras()
        {
            var p = MakeParty(2);
            var a = Member("A"); var b = Member("B"); var c = Member("C");
            p.Recruit(a); p.Recruit(b); p.Recruit(c);

            Assert.AreEqual(3, p.roster.Count, "Everyone joins the roster.");
            Assert.AreEqual(2, p.active.Count, "The field party is capped at maxActive.");
            CollectionAssert.DoesNotContain(p.active, c);
        }

        [Test]
        public void Recruit_DuplicateOrNull_IsNoOp()
        {
            var p = MakeParty(4);
            var hero = Member("Hero");
            p.Recruit(hero);
            p.Recruit(hero);
            p.Recruit(null);
            Assert.AreEqual(1, p.roster.Count);
        }

        [Test]
        public void Remove_DropsFromRosterAndField()
        {
            var p = MakeParty(4);
            var hero = Member("Hero");
            p.Recruit(hero);
            p.Remove(hero);
            CollectionAssert.DoesNotContain(p.roster, hero);
            CollectionAssert.DoesNotContain(p.active, hero);
        }

        [Test]
        public void SetActive_True_FailsWhenFieldFullOrNotRostered()
        {
            var p = MakeParty(1);
            var a = Member("A"); var b = Member("B");
            p.Recruit(a);          // fills the single active slot
            p.Recruit(b);          // benched (roster only)

            Assert.IsFalse(p.SetActive(b, true), "Field is full ⇒ cannot field another.");

            var stranger = Member("Stranger");
            Assert.IsFalse(p.SetActive(stranger, true), "Non-roster member cannot be fielded.");
        }

        [Test]
        public void SetActive_TogglesFieldMembershipWithoutLeavingRoster()
        {
            var p = MakeParty(4);
            var hero = Member("Hero");
            p.Recruit(hero);

            Assert.IsTrue(p.SetActive(hero, false));
            CollectionAssert.DoesNotContain(p.active, hero);
            CollectionAssert.Contains(p.roster, hero, "Benching keeps them in the roster.");

            Assert.IsTrue(p.SetActive(hero, true));
            CollectionAssert.Contains(p.active, hero);
        }

        [Test]
        public void IsWiped_TrueOnlyWhenNoActiveMemberLives()
        {
            var p = MakeParty(4);
            var alive = Member("Alive", alive: true);
            var dead = Member("Dead", alive: false);
            p.Recruit(dead);
            p.Recruit(alive);
            Assert.IsFalse(p.IsWiped(), "One living member ⇒ not wiped.");

            var allDead = MakeParty(4);
            allDead.Recruit(Member("D1", alive: false));
            allDead.Recruit(Member("D2", alive: false));
            Assert.IsTrue(allDead.IsWiped());
        }
    }
}
