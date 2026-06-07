using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Grid;
using SunderedCrown.Stats;

namespace SunderedCrown.Tests.PlayMode
{
    /// <summary>
    /// PlayMode coverage for the one major system EditMode can't reach: TurnManager drives
    /// combat through GridUnit MonoBehaviours that place themselves on a live GridSystem in
    /// Start(). These pin initiative ordering (living only), instant win/loss, round advance,
    /// the incapacitation skip, and per-turn action economy. The full MonoBehaviour lifecycle
    /// runs, so this is where it belongs.
    /// </summary>
    public class TurnManagerPlayTests
    {
        private readonly List<Object> _spawned = new List<Object>();
        private GridSystem _grid;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var go = new GameObject("Grid");
            _spawned.Add(go);
            _grid = go.AddComponent<GridSystem>();
            _grid.width = 12;
            _grid.height = 12;
            _grid.Build();
            yield return null; // let Awake/Start settle
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var o in _spawned) if (o != null) Object.DestroyImmediate(o);
            _spawned.Clear();
        }

        private GridUnit Unit(string name, Faction faction, int x, int y,
            int dex = 10, int hp = 50, bool alive = true, StatusEffectDefinition effect = null)
        {
            var go = new GameObject(name);
            _spawned.Add(go);
            var u = go.AddComponent<GridUnit>();
            u.faction = faction;
            u.startCoord = new Vector2Int(x, y);

            var s = new CharacterSheet { displayName = name, maxHitPoints = hp, currentHitPoints = alive ? hp : 0, baseArmorClass = 12 };
            s.abilities.Set(Ability.Dexterity, dex);
            if (effect != null) s.AddEffect(effect, 3, "test");
            u.Sheet = s;
            return u;
        }

        private TurnManager Manager()
        {
            var go = new GameObject("CombatManager");
            _spawned.Add(go);
            return go.AddComponent<TurnManager>();
        }

        private StatusEffectDefinition Incapacitating()
        {
            var d = ScriptableObject.CreateInstance<StatusEffectDefinition>();
            d.effectName = "Stunned";
            d.condition = Condition.Stunned;
            d.incapacitates = true;
            _spawned.Add(d);
            return d;
        }

        [UnityTest]
        public IEnumerator StartCombat_OrdersOnlyLivingParticipants()
        {
            var p1 = Unit("Hero", Faction.Player, 1, 1);
            var e1 = Unit("Goblin", Faction.Enemy, 5, 5);
            var dead = Unit("Corpse", Faction.Enemy, 6, 6, alive: false);
            yield return null; // run GridUnit.Start (placement)

            var tm = Manager();
            tm.StartCombat(new[] { p1, e1, dead });

            Assert.IsTrue(tm.InCombat);
            Assert.AreEqual(1, tm.RoundNumber);
            Assert.AreEqual(2, tm.TurnOrder.Count, "Dead combatants are excluded from the order.");
            CollectionAssert.DoesNotContain(tm.TurnOrder, dead);
            Assert.IsNotNull(tm.ActiveUnit);
            Assert.AreSame(tm.TurnOrder[0], tm.ActiveUnit, "The first turn goes to the top of initiative.");
        }

        [UnityTest]
        public IEnumerator OneFactionOnly_EndsCombatImmediately()
        {
            var p1 = Unit("Hero", Faction.Player, 1, 1);
            var p2 = Unit("Ally", Faction.Ally, 2, 2);
            yield return null;

            var tm = Manager();
            bool ended = false;
            tm.OnCombatEnded += () => ended = true;
            tm.StartCombat(new[] { p1, p2 });

            Assert.IsFalse(tm.InCombat, "No enemies ⇒ combat resolves at once.");
            Assert.IsTrue(ended);
        }

        [UnityTest]
        public IEnumerator NextTurn_AdvancesActiveUnit_AndIncrementsRoundOnWrap()
        {
            var p1 = Unit("Hero", Faction.Player, 1, 1, hp: 999);
            var e1 = Unit("Goblin", Faction.Enemy, 5, 5, hp: 999);
            yield return null;

            var tm = Manager();
            tm.StartCombat(new[] { p1, e1 });
            var first = tm.ActiveUnit;

            tm.NextTurn();
            Assert.AreNotSame(first, tm.ActiveUnit, "Second turn goes to the next combatant.");
            Assert.AreEqual(1, tm.RoundNumber);

            tm.NextTurn();
            Assert.AreSame(first, tm.ActiveUnit, "Order wraps back to the top.");
            Assert.AreEqual(2, tm.RoundNumber, "Wrapping starts a new round.");
        }

        [UnityTest]
        public IEnumerator IncapacitatedUnit_LosesItsTurn()
        {
            var stunned = Unit("Stunned", Faction.Player, 1, 1, effect: Incapacitating());
            var enemy = Unit("Goblin", Faction.Enemy, 5, 5, hp: 999);
            yield return null;

            var tm = Manager();
            tm.StartCombat(new[] { stunned, enemy });

            // Advance until the stunned unit is active (guard against infinite loop).
            for (int i = 0; i < 6 && tm.ActiveUnit != stunned; i++) tm.NextTurn();
            Assert.AreSame(stunned, tm.ActiveUnit, "Expected to reach the stunned unit's turn.");

            Assert.IsFalse(tm.ActionAvailable, "An incapacitated unit gets no action.");
            Assert.AreEqual(0, tm.MovementRemaining, "An incapacitated unit gets no movement.");
        }

        [UnityTest]
        public IEnumerator ActionEconomy_AllowsOneActionPerTurn()
        {
            var p1 = Unit("Hero", Faction.Player, 1, 1, hp: 999);
            var e1 = Unit("Goblin", Faction.Enemy, 5, 5, hp: 999);
            yield return null;

            var tm = Manager();
            tm.StartCombat(new[] { p1, e1 });
            // ActiveUnit is a non-incapacitated combatant at the top of initiative.

            Assert.IsTrue(tm.TrySpendAction(), "Action available at the start of a turn.");
            Assert.IsFalse(tm.TrySpendAction(), "Only one action per turn.");

            Assert.IsTrue(tm.TrySpendBonusAction(), "Bonus action available once.");
            Assert.IsFalse(tm.TrySpendBonusAction(), "Only one bonus action per turn.");
        }
    }
}
