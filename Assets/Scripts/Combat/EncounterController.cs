using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Grid;
using SunderedCrown.FX;

namespace SunderedCrown.Combat
{
    /// <summary>
    /// Turns a scene of GridUnits into a running encounter. Collects participants,
    /// starts the TurnManager, and on each ENEMY turn runs a simple AI: pick an
    /// ability, close to within its range, then use it via AbilityRunner. Player and
    /// ally turns wait for input.
    /// </summary>
    public class EncounterController : MonoBehaviour
    {
        public bool autoStartOnPlay = true;
        private TurnManager _turns;

        void Start()
        {
            _turns = TurnManager.Instance;
            _turns.OnTurnStarted += HandleTurnStarted;
            _turns.OnCombatEnded += HandleCombatEnded;
            if (autoStartOnPlay)
                _turns.StartCombat(FindObjectsByType<GridUnit>(FindObjectsSortMode.None));
        }

        void OnDestroy()
        {
            if (_turns != null)
            {
                _turns.OnTurnStarted -= HandleTurnStarted;
                _turns.OnCombatEnded -= HandleCombatEnded;
            }
        }

        /// <summary>Combat is over: branch on whether anyone friendly is still standing.</summary>
        private void HandleCombatEnded()
        {
            bool anyAlive = _turns.TurnOrder.Any(u => u != null && u.Sheet.IsAlive &&
                (u.faction == Faction.Player || u.faction == Faction.Ally));
            if (anyAlive) AwardExperience();
            else ShowDefeat();
        }

        /// <summary>Total-party defeat: surface a recovery screen (load last save / return to title) on its
        /// own root object, so it outlives the campaign it replaces.</summary>
        private void ShowDefeat()
        {
            _turns.Log("The battlefield falls silent. But you have never been good at staying gone...");
            var go = new GameObject("DefeatScreen");
            go.AddComponent<SunderedCrown.UI.DefeatScreen>();
        }

        /// <summary>On victory, pool defeated enemies' XP + loot and distribute to the party.</summary>
        private void AwardExperience()
        {
            SunderedCrown.Core.GameFlags.Current.AddInt("combat.victories", 1); // tally fights won
            int pool = 0, gold = 0;
            var drops = new List<Items.ItemDefinition>();
            var survivors = new List<GridUnit>();
            foreach (var u in _turns.TurnOrder)
            {
                bool friendly = u.faction == Faction.Player || u.faction == Faction.Ally;
                if (u.faction == Faction.Enemy && !u.Sheet.IsAlive)
                {
                    pool += u.Sheet.experienceValue;
                    gold += u.Sheet.lootGold;
                    if (u.Sheet.loot != null) drops.AddRange(u.Sheet.loot);
                    if (u.Sheet.displayName != null) // bestiary: tally foes laid low (by name)
                    {
                        SunderedCrown.Core.GameFlags.Current.AddInt("slain." + u.Sheet.displayName, 1);
                        SunderedCrown.Core.GameFlags.Current.AddInt("slain.total", 1);
                    }
                }
                else if (friendly && u.Sheet.IsAlive) survivors.Add(u);
            }
            if (survivors.Count == 0) return;

            // XP split among survivors.
            if (pool > 0)
            {
                int each = Mathf.Max(1, pool / survivors.Count);
                foreach (var u in survivors)
                {
                    int levels = Progression.AwardExperience(u.Sheet, each);
                    _turns.Log($"{u.Sheet.displayName} gains {each} XP" +
                               (levels > 0 ? $" and reaches level {u.Sheet.level}!" : "."));
                    if (levels > 0)
                    {
                        FloatingCombatText.Spawn(u.transform.position + Vector3.up * 0.6f, "LEVEL UP!", FloatingCombatText.Heal, 18f);
                        SunderedCrown.FX.AudioSystem.PlaySfx("levelup"); // art-optional
                    }
                    // Announce any ability unlocked by this level (per-level kit grant).
                    if (levels > 0)
                    {
                        var kit = u.Sheet.classDef != null ? u.Sheet.classDef.startingAbilities : null;
                        int idx = u.Sheet.level - 1;
                        if (kit != null && idx >= 0 && idx < kit.Length && kit[idx] != null)
                            _turns.Log($"  ✦ {u.Sheet.displayName} learns {kit[idx].abilityName}!");
                    }
                }
            }

            // Loot + gold to the shared party stash.
            if (Party.Instance != null)
            {
                if (gold > 0) { Party.Instance.inventory.AddGold(gold); _turns.Log($"Looted {gold} gold."); }
                foreach (var item in drops)
                {
                    Party.Instance.inventory.Add(item);
                    _turns.Log($"Looted: {item.displayName}.");
                }
            }

            // The party won (someone's standing). Two consequences:
            //  1) Downed companions are knocked out, NOT dead — stabilize them after the fight. (Permanent
            //     loss in this game is reserved for the Breach, by design.)
            //  2) The bond breathes: surviving a fight together nudges fielded companions' approval a touch —
            //     a little more for anyone you had to pull back from the brink.
            foreach (var u in _turns.TurnOrder)
            {
                if (u == null) continue;
                bool friendly = u.faction == Faction.Player || u.faction == Faction.Ally;
                if (!friendly) continue;

                string id = CompanionId(u.Sheet?.displayName);
                bool wasDowned = u.Sheet != null && !u.Sheet.IsAlive;

                if (wasDowned)
                {
                    u.Sheet.Heal(Mathf.Max(1, u.Sheet.maxHitPoints / 4)); // stabilized, on their feet
                    _turns.Log($"{u.Sheet.displayName} is pulled back from the brink as the dust settles.");
                    SunderedCrown.Core.GameFlags.Current.SetBool("combat.clutch_win", true); // won with a downed ally
                }

                if (id != null)
                    SunderedCrown.Core.GameFlags.Current.AdjustApproval(id, wasDowned ? 3 : 1);
            }
        }

        /// <summary>Map a fielded unit's name to its companion approval id (null for the hero / enemies).</summary>
        private static string CompanionId(string displayName)
        {
            if (string.IsNullOrEmpty(displayName)) return null;
            foreach (var pair in _companionNames)
                if (displayName.Contains(pair.Key)) return pair.Value;
            return null;
        }

        private static readonly Dictionary<string, string> _companionNames = new Dictionary<string, string>
        {
            { "Garrow", "garrow" }, { "Roen", "roen" }, { "Varra", "varra" },
            { "Naeve", "naeve" }, { "Ilfaeril", "ilfaeril" }, { "Maerin", "maerin" },
        };

        private void HandleTurnStarted(GridUnit unit)
        {
            if (unit.faction == Faction.Enemy)
                StartCoroutine(RunEnemyTurn(unit));
        }

        private IEnumerator RunEnemyTurn(GridUnit enemy)
        {
            yield return new WaitForSeconds(SunderedCrown.Core.GameSettings.CombatDelay(0.4f));

            // Incapacitated or no targets → pass.
            if (enemy.Sheet.IsIncapacitated) { _turns.NextTurn(); yield break; }
            var ability = ChooseAttack(enemy);
            var target = ChooseTarget(enemy, ability);
            if (target == null || ability == null) { _turns.NextTurn(); yield break; }

            int range = Mathf.Max(1, ability.rangeTiles);

            // Kiting: a ranged attacker cornered in melee steps back one tile to open distance, then fires.
            if (range >= 3 && GridSystem.ManhattanDistance(enemy.Cell, target.Cell) <= 1)
            {
                var retreat = FindRetreatCell(enemy, target);
                if (retreat != null)
                {
                    var enemyStart = enemy.Cell;
                    _turns.TrySpendMovement(1);
                    yield return enemy.StartCoroutine(enemy.FollowPath(new List<GridCell> { retreat }));
                    Reactions.OnMoveCompleted(_turns, enemy, enemyStart); // stepping back may draw an OA
                    if (!enemy.Sheet.IsAlive) { _turns.NextTurn(); yield break; }
                }
            }

            // Close the distance until within the ability's range (movement budget permitting).
            if (GridSystem.ManhattanDistance(enemy.Cell, target.Cell) > range)
            {
                var path = Pathfinding.FindPath(GridSystem.Instance, enemy.Cell, target.Cell);
                if (path != null && path.Count > 0)
                {
                    // Stop as soon as we're in range; never path onto the target's tile.
                    int stopAt = path.Count;
                    for (int i = 0; i < path.Count; i++)
                        if (GridSystem.ManhattanDistance(path[i], target.Cell) <= range) { stopAt = i + 1; break; }
                    if (stopAt > 0 && path[stopAt - 1] == target.Cell) stopAt--;

                    var trimmed = path.GetRange(0, Mathf.Min(stopAt, Mathf.Min(path.Count, _turns.MovementRemaining)));
                    if (trimmed.Count > 0)
                    {
                        var enemyStart = enemy.Cell;
                        _turns.TrySpendMovement(trimmed.Count);
                        yield return enemy.StartCoroutine(enemy.FollowPath(trimmed));
                        Reactions.OnMoveCompleted(_turns, enemy, enemyStart); // party may strike as it flees melee
                        if (!enemy.Sheet.IsAlive) { _turns.NextTurn(); yield break; } // an OA dropped it
                    }
                }
            }

            // Attack if now in range; otherwise, a wounded enemy that couldn't reach anyone hunkers down
            // (Dodge), making it harder to finish off when you can't close the gap either.
            if (AbilityRunner.InRange(enemy, target, ability))
                AbilityRunner.TryUse(_turns, enemy, target, ability, _turns.TurnOrder.ToList());
            else if (enemy.Sheet.maxHitPoints > 0 &&
                     enemy.Sheet.currentHitPoints <= enemy.Sheet.maxHitPoints * 0.35f)
            {
                enemy.Sheet.IsDodging = true; // defensive stance; not the player's tracked action
                _turns.Log($"  🛡 {enemy.Sheet.displayName}, wounded and cornered, takes a defensive stance.");
            }

            yield return new WaitForSeconds(SunderedCrown.Core.GameSettings.CombatDelay(0.4f));
            _turns.NextTurn();
        }

        /// <summary>Pick the enemy's best damaging ability it can afford (skip heals/self).</summary>
        private AbilityDefinition ChooseAttack(GridUnit enemy)
        {
            foreach (var ab in enemy.Sheet.knownAbilities)
                if (ab != null && !ab.isHeal && ab.targeting != TargetingMode.Self &&
                    AbilityRunner.CanAfford(_turns, enemy, ab))
                    return ab;
            return enemy.Sheet.DefaultAttack;
        }

        /// <summary>An adjacent free, walkable tile that increases distance from <paramref name="foe"/> — for a
        /// ranged enemy kiting out of melee. Null if no neighbor opens distance.</summary>
        private GridCell FindRetreatCell(GridUnit enemy, GridUnit foe)
        {
            var grid = GridSystem.Instance;
            if (grid == null || enemy.Cell == null || foe.Cell == null) return null;
            int curDist = GridSystem.ManhattanDistance(enemy.Cell, foe.Cell);
            GridCell best = null; int bestDist = curDist;
            int[] dx = { 1, -1, 0, 0 }, dy = { 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                var c = grid.GetCell(enemy.Cell.x + dx[i], enemy.Cell.y + dy[i]);
                if (c == null || !c.IsFree) continue;
                int d = GridSystem.ManhattanDistance(c, foe.Cell);
                if (d > bestDist) { bestDist = d; best = c; }
            }
            return best;
        }

        private GridUnit NearestPlayer(GridUnit from)
        {
            GridUnit best = null;
            int bestDist = int.MaxValue;
            foreach (var u in _turns.TurnOrder)
            {
                if (!u.Sheet.IsAlive) continue;
                if (u.faction != Faction.Player && u.faction != Faction.Ally) continue;
                int d = GridSystem.ManhattanDistance(from.Cell, u.Cell);
                if (d < bestDist) { bestDist = d; best = u; }
            }
            return best;
        }

        /// <summary>Smarter target pick: among foes this enemy could actually reach and strike this turn,
        /// focus-fire the lowest-HP one (finish kills, concentrate damage); break ties by proximity. If
        /// nothing is in reach, fall back to closing on the nearest target.</summary>
        private GridUnit ChooseTarget(GridUnit enemy, AbilityDefinition ability)
        {
            int range = Mathf.Max(1, ability != null ? ability.rangeTiles : 1);
            int budget = enemy.Sheet.SpeedTiles; // movement available before any is spent this turn

            // Area attacks: center the burst on the reachable foe whose tile catches the most party members.
            if (ability != null && ability.targeting == TargetingMode.AreaBurst)
            {
                GridUnit bestCenter = null; int bestCaught = 0, bestCenterDist = int.MaxValue;
                foreach (var c in _turns.TurnOrder)
                {
                    if (c == null || !c.Sheet.IsAlive) continue;
                    if (c.faction != Faction.Player && c.faction != Faction.Ally) continue;
                    if (GridSystem.ManhattanDistance(enemy.Cell, c.Cell) - range > budget) continue; // can't reach to cast
                    int caught = 0;
                    foreach (var v in _turns.TurnOrder)
                        if (v != null && v.Sheet.IsAlive && (v.faction == Faction.Player || v.faction == Faction.Ally) &&
                            GridSystem.ManhattanDistance(c.Cell, v.Cell) <= ability.areaRadiusTiles) caught++;
                    int d = GridSystem.ManhattanDistance(enemy.Cell, c.Cell);
                    if (caught > bestCaught || (caught == bestCaught && d < bestCenterDist))
                    { bestCaught = caught; bestCenterDist = d; bestCenter = c; }
                }
                if (bestCenter != null) return bestCenter;
            }

            GridUnit bestReach = null; int bestHp = int.MaxValue, bestReachDist = int.MaxValue;
            foreach (var u in _turns.TurnOrder)
            {
                if (u == null || !u.Sheet.IsAlive) continue;
                if (u.faction != Faction.Player && u.faction != Faction.Ally) continue;
                int dist = GridSystem.ManhattanDistance(enemy.Cell, u.Cell);
                bool reachable = dist - range <= budget; // can close to within range this turn
                if (!reachable) continue;
                int hp = u.Sheet.currentHitPoints;
                if (hp < bestHp || (hp == bestHp && dist < bestReachDist))
                { bestHp = hp; bestReachDist = dist; bestReach = u; }
            }
            return bestReach ?? NearestPlayer(enemy); // nothing strikeable → advance on the nearest
        }
    }
}
