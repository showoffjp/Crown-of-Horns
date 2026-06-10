using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Content;
using SunderedCrown.Grid;
using SunderedCrown.Quests;
using SunderedCrown.Stats;
using SunderedCrown.UI;

namespace SunderedCrown.Core
{
    /// <summary>
    /// THE PLAYABLE PROLOGUE. Put this on an empty GameObject in a fresh scene and press
    /// Play. It runs the full vertical-slice loop end to end:
    ///   1) Character creation (race/class/background/point-buy) → your hero, the Returned.
    ///   2) The opening dialogue with Aldric's herald (skill checks, approval, flags).
    ///   3) A turn-based skirmish vs the Doomguides at the door, with the uGUI combat HUD.
    ///   4) Victory awards XP (and can trigger a level-up) and completes the first quest.
    ///
    /// All FR content is built in code (SwordCoastContent) so the slice is self-contained.
    /// </summary>
    public class PrologueBootstrap : MonoBehaviour
    {
        private SwordCoastContent _content;
        private CharacterSheet _hero;
        private GridSystem _grid;
        private TurnManager _turns;

        void Start()
        {
            EnsureCore();
            _content = new SwordCoastContent();

            // Register the first quest.
            var qmGO = new GameObject("QuestManager");
            var qm = qmGO.AddComponent<QuestManager>();
            qm.allQuests.Add(_content.FirstQuest);
            StartCoroutine(StartQuestNextFrame(qm));

            // Show character creation; continue when the hero is built.
            var creationGO = new GameObject("CharacterCreation");
            var creation = creationGO.AddComponent<CharacterCreationScreen>();
            creation.races = _content.Races;
            creation.classes = _content.Classes;
            creation.backgrounds = _content.Backgrounds;
            creation.OnComplete += OnHeroCreated;
        }

        private IEnumerator StartQuestNextFrame(QuestManager qm)
        {
            yield return null; // let QuestManager.Start subscribe to GameFlags
            qm.StartQuest(_content.FirstQuest.questId);
        }

        private void EnsureCore()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();
        }

        // ---- after creation: play the intro dialogue ----
        private void OnHeroCreated(CharacterSheet hero)
        {
            _hero = hero;
            if (hero.knownAbilities.Count > 0) hero.equippedWeaponAbility = hero.knownAbilities[0];
            if (hero.classDef != null && hero.classDef.isSpellcaster) hero.spellSlots.max[1] = 2;

            var dlgGO = new GameObject("DialogueScreen");
            var dlg = dlgGO.AddComponent<DialogueScreen>();
            dlg.playerSpeaker = hero;
            dlg.autoPlay = _content.PrologueDialogue;
            dlg.onFinished = () => StartCoroutine(BeginCombat());
        }

        // ---- the prologue skirmish ----
        private IEnumerator BeginCombat()
        {
            // Grid.
            var gridGO = new GameObject("GridSystem");
            _grid = gridGO.AddComponent<GridSystem>();
            _grid.width = 16; _grid.height = 16; _grid.tileWidth = 1f; _grid.tileHeight = 0.5f;

            // Managers + UI.
            var combatGO = new GameObject("CombatManager");
            _turns = combatGO.AddComponent<TurnManager>();
            var enc = combatGO.AddComponent<EncounterController>();
            enc.autoStartOnPlay = false;               // we start manually after spawning
            combatGO.AddComponent<CombatHUD>();         // real uGUI HUD
            var input = combatGO.AddComponent<PlayerCombatInput>();

            // Camera.
            var cam = Camera.main;
            if (cam == null) { var g = new GameObject("Main Camera"); g.tag = "MainCamera"; cam = g.AddComponent<Camera>(); }
            cam.orthographic = true; cam.orthographicSize = 7f;
            cam.transform.position = _grid.GridToWorld(8, 8) + new Vector3(0, 0, -10);
            if (cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() == null)
                cam.gameObject.AddComponent<SunderedCrown.CameraRig.IsometricCameraController>();
            input.worldCamera = cam;

            // Set prologue.cleared on victory (completes the quest's second objective).
            // One-shot: unsubscribe on first fire so it can't linger past this combat and
            // mark the flag when some later battle ends.
            System.Action onPrologueEnd = null;
            onPrologueEnd = () =>
            {
                _turns.OnCombatEnded -= onPrologueEnd;
                bool win = false;
                foreach (var u in _turns.TurnOrder)
                    if ((u.faction == Faction.Player || u.faction == Faction.Ally) && u.Sheet.IsAlive) win = true;
                if (win) GameFlags.Current.SetBool("prologue.cleared", true);
            };
            _turns.OnCombatEnded += onPrologueEnd;

            yield return null; // let component Start() methods run (HUD canvas, etc.)

            // Spawn the hero.
            var party = new List<GridUnit>();
            party.Add(SpawnSheet(_hero, Faction.Player, new Vector2Int(3, 7), Color.blue));

            // Spawn Sister Garrow — your first companion (Cleric of Kelemvor).
            var garrow = MakeNpc("Sister Garrow", _content.Classes.Find(c => c.className == "Cleric"),
                _content.Races[0], 12, 14, level: 2,
                _content.Abilities["mace"], _content.Abilities["cure_wounds"], _content.Abilities["bless"]);
            garrow.spellSlots.max[1] = 3;
            party.Add(SpawnSheet(garrow, Faction.Ally, new Vector2Int(3, 9), Color.cyan));

            // Spawn the Doomguides + an Unbound horror.
            var fighter = _content.Classes.Find(c => c.className == "Fighter");
            var enemies = new List<GridUnit>
            {
                SpawnSheet(MakeNpc("Doomguide Knight", fighter, _content.Races[0], 15, 13, 2, _content.Abilities["longsword"]) , Faction.Enemy, new Vector2Int(12, 6), Color.red, xp: 100),
                SpawnSheet(MakeNpc("Doomguide Knight", fighter, _content.Races[0], 15, 13, 2, _content.Abilities["longsword"]) , Faction.Enemy, new Vector2Int(13, 8), Color.red, xp: 100),
                SpawnSheet(MakeNpc("Doomguide Acolyte", fighter, _content.Races[0], 12, 12, 1, _content.Abilities["mace"])    , Faction.Enemy, new Vector2Int(12, 10), new Color(0.9f,0.4f,0.3f), xp: 75),
                SpawnSheet(MakeNpc("The Unbound", fighter, _content.Races[0], 13, 14, 2, _content.Abilities["rotting_claw"])  , Faction.Enemy, new Vector2Int(11, 8), new Color(0.5f,0.2f,0.4f), xp: 50),
            };

            var all = new List<GridUnit>(party); all.AddRange(enemies);
            _turns.StartCombat(all);
            Debug.Log("[Prologue] The fight begins. Click to move, arm abilities on the bar, click targets, End Turn.");
        }

        // ---- spawn / build helpers ----

        private CharacterSheet MakeNpc(string name, ClassDefinition cls, RaceDefinition race,
            int str, int con, int level, params AbilityDefinition[] abilities)
        {
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = race, level = level, baseArmorClass = 13 };
            s.abilities.Set(Ability.Strength, str);
            s.abilities.Set(Ability.Dexterity, 13);
            s.abilities.Set(Ability.Constitution, con);
            s.abilities.Set(Ability.Wisdom, 14);
            s.RecalculateMaxHitPoints();
            foreach (var a in abilities) s.knownAbilities.Add(a);
            if (abilities.Length > 0) s.equippedWeaponAbility = abilities[0];
            return s;
        }

        private GridUnit SpawnSheet(CharacterSheet sheet, Faction faction, Vector2Int coord, Color color, int xp = 0)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = sheet.displayName;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            go.GetComponent<Renderer>().material.color = color;
            var unit = go.AddComponent<GridUnit>();
            unit.faction = faction; unit.startCoord = coord;
            sheet.experienceValue = xp;
            unit.Sheet = sheet;
            return unit;
        }
    }
}
