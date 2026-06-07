using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Core;
using SunderedCrown.Grid;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.PlayMode
{
    /// <summary>
    /// Opportunity attacks (Reactions.OnMoveCompleted): leaving an adversary's melee reach
    /// without Disengaging provokes one free strike, once per reactor per round. Asserted
    /// deterministically by counting the combat-log line (not RNG damage). The FX/audio the
    /// strike triggers all no-op headlessly (no art/clips/camera), so this runs clean.
    /// </summary>
    public class ReactionsPlayTests
    {
        private readonly List<Object> _spawned = new List<Object>();
        private GridSystem _grid;
        private GameFlags _flags;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _flags = GameFlags.Current;
            GameFlags.Replace(new GameFlags());
            var go = new GameObject("Grid");
            _spawned.Add(go);
            _grid = go.AddComponent<GridSystem>();
            _grid.width = 12; _grid.height = 12;
            _grid.Build();
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
            GameFlags.Replace(_flags ?? new GameFlags());
        }

        private GridUnit Unit(string name, Faction faction, int x, int y, bool withAttack = false)
        {
            var go = new GameObject(name);
            _spawned.Add(go);
            var u = go.AddComponent<GridUnit>();
            u.faction = faction;
            u.startCoord = new Vector2Int(x, y);
            var s = new CharacterSheet { displayName = name, maxHitPoints = 999, currentHitPoints = 999, baseArmorClass = 12 };
            s.abilities.Set(Ability.Strength, 14);
            s.abilities.Set(Ability.Dexterity, 10);
            if (withAttack)
            {
                var atk = ScriptableObject.CreateInstance<AbilityDefinition>();
                atk.abilityName = "Claw"; atk.isAttackRoll = true; atk.rangeTiles = 1;
                atk.damageDice = "1d6"; atk.addAbilityModToDamage = true; atk.damageType = DamageType.Slashing;
                _spawned.Add(atk);
                s.knownAbilities.Add(atk); // becomes DefaultAttack
            }
            u.Sheet = s;
            return u;
        }

        // Returns a live counter of opportunity-attack log lines emitted by this fight.
        private TurnManager StartFight(params GridUnit[] units)
        {
            var go = new GameObject("CombatManager");
            _spawned.Add(go);
            var tm = go.AddComponent<TurnManager>();
            int count = 0;
            tm.OnCombatLog += msg => { if (msg != null && msg.Contains("opportunity attack")) count++; };
            _oa = () => count; // closes over the live local
            tm.StartCombat(units);
            return tm;
        }

        private System.Func<int> _oa;

        [UnityTest]
        public IEnumerator LeavingMeleeReach_ProvokesOneOpportunityAttack()
        {
            var reactor = Unit("Goblin", Faction.Enemy, 5, 5, withAttack: true);
            var mover = Unit("Hero", Faction.Player, 5, 6); // adjacent to the goblin
            yield return null;

            var tm = StartFight(reactor, mover);
            var startCell = mover.Cell;            // (5,6), within the goblin's reach
            mover.PlaceAt(_grid.GetCell(5, 8));    // step well out of reach

            Reactions.OnMoveCompleted(tm, mover, startCell);

            Assert.AreEqual(1, _oa(), "Leaving reach should provoke exactly one OA.");
            Assert.AreEqual(tm.RoundNumber, reactor.Sheet.lastReactionRound, "The reactor spent its reaction this round.");
        }

        [UnityTest]
        public IEnumerator OpportunityAttack_IsLimitedToOncePerRound()
        {
            var reactor = Unit("Goblin", Faction.Enemy, 5, 5, withAttack: true);
            var mover = Unit("Hero", Faction.Player, 5, 6);
            yield return null;

            var tm = StartFight(reactor, mover);

            Reactions.OnMoveCompleted(tm, mover, _grid.GetCell(5, 6)); // first: provokes
            mover.PlaceAt(_grid.GetCell(5, 6));                         // shuffle back adjacent
            Reactions.OnMoveCompleted(tm, mover, _grid.GetCell(5, 6));  // same round: must not re-fire

            Assert.AreEqual(1, _oa(), "A reactor gets only one opportunity attack per round.");
        }

        [UnityTest]
        public IEnumerator Disengaging_SuppressesTheOpportunityAttack()
        {
            var reactor = Unit("Goblin", Faction.Enemy, 5, 5, withAttack: true);
            var mover = Unit("Hero", Faction.Player, 5, 6);
            yield return null;

            var tm = StartFight(reactor, mover);
            mover.Sheet.IsDisengaging = true;
            var startCell = mover.Cell;
            mover.PlaceAt(_grid.GetCell(5, 8));

            Reactions.OnMoveCompleted(tm, mover, startCell);

            Assert.AreEqual(0, _oa(), "Disengage means moving provokes nothing.");
        }

        [UnityTest]
        public IEnumerator StayingInReach_DoesNotProvoke()
        {
            var reactor = Unit("Goblin", Faction.Enemy, 5, 5, withAttack: true);
            var mover = Unit("Hero", Faction.Player, 5, 6);
            yield return null;

            var tm = StartFight(reactor, mover);
            var startCell = mover.Cell;          // (5,6) adjacent
            mover.PlaceAt(_grid.GetCell(4, 5));  // still adjacent to (5,5)

            Reactions.OnMoveCompleted(tm, mover, startCell);

            Assert.AreEqual(0, _oa(), "Shuffling within reach provokes nothing.");
        }
    }
}
