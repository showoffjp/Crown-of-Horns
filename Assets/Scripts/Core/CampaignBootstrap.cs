using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Content;
using SunderedCrown.Grid;
using SunderedCrown.Items;
using SunderedCrown.Quests;
using SunderedCrown.Stats;
using SunderedCrown.UI;

namespace SunderedCrown.Core
{
    /// <summary>
    /// THE CAMPAIGN. The flagship entry point and mode director — drop on one empty
    /// GameObject and press Play to run the whole loop:
    ///   1) Character creation → your hero, the Returned.
    ///   2) Explore the Baldur's Gate hub (walk, talk, take the quest, recruit Roen).
    ///   3) Descend into the multi-room Cinderhaunt (loot, a locked door, a guard fight,
    ///      and a boss) — your whole active party (recruits included) fights.
    ///   4) Clear it → XP/level, quest completes, the road to Aldric opens; back to town.
    ///
    /// Persistent managers (GameManager, Party, QuestManager) live across modes; each mode
    /// (hub / dungeon / encounter) is built under a disposable root the director swaps out.
    /// </summary>
    public class CampaignBootstrap : MonoBehaviour
    {
        private SwordCoastContent _content;
        private NetherilContent _netheril;
        private CrownWarsContent _crownwars;
        private LateEraContent _lateEras;
        private FugueContent _fugue;
        private AldricContent _aldric;
        private RiddleContent _riddles;
        private RoenQuestContent _roenQuest;
        private VarraQuestContent _varraQuest;
        private GarrowQuestContent _garrowQuest;
        private NaeveQuestContent _naeveQuest;
        private IlfaerilQuestContent _ilfaerilQuest;
        private ActTwoContent _actTwo;
        private CharacterSheet _hero;
        private GameObject _modeRoot;

        [Tooltip("If true, load the saved campaign instead of starting character creation.")]
        public bool continueGame = false;
        private string SaveSlot => SaveSlots.Active; // selectable via the save-slot manager

        // Companions recruited via dialogue flags ("companion.<id>.recruited").
        private readonly Dictionary<string, CharacterSheet> _companions = new Dictionary<string, CharacterSheet>();

        void Start()
        {
            EnsureCore();
            // Persistent overlays (above the swappable mode root): options (O), help (H), chronicle (C),
            // and world-space nameplates/HP bars over every unit (N).
            gameObject.AddComponent<SunderedCrown.UI.UiScaler>(); // accessibility: applies UI text-size first
            gameObject.AddComponent<SunderedCrown.UI.SettingsScreen>();
            gameObject.AddComponent<SunderedCrown.UI.HelpOverlay>();
            gameObject.AddComponent<SunderedCrown.UI.ChronicleScreen>();
            gameObject.AddComponent<SunderedCrown.UI.RelationshipsScreen>();
            gameObject.AddComponent<SunderedCrown.UI.CodexNotifier>(); // toast when new lore unlocks
            gameObject.AddComponent<SunderedCrown.UI.UnitNameplates>();
            gameObject.AddComponent<SunderedCrown.Rendering.UnitSpriteSkinner>(); // art-optional: cubes → sprites if present
            _content = new SwordCoastContent();
            _netheril = new NetherilContent(_content);
            _crownwars = new CrownWarsContent(_content);
            _lateEras = new LateEraContent();
            _fugue = new FugueContent(_content);
            _aldric = new AldricContent();
            _riddles = new RiddleContent();
            _roenQuest = new RoenQuestContent();
            _varraQuest = new VarraQuestContent();
            _garrowQuest = new GarrowQuestContent();
            _naeveQuest = new NaeveQuestContent();
            _ilfaerilQuest = new IlfaerilQuestContent();
            _actTwo = new ActTwoContent();

            var qm = new GameObject("QuestManager").AddComponent<QuestManager>();
            qm.allQuests.Add(_content.FirstQuest);

            if (continueGame && SunderedCrown.Save.SaveSystem.Exists(SaveSlot))
            {
                ContinueGame();
                return;
            }

            // New Game+ awareness: a fresh run after finishing at least one saga. Story content can key off
            // this (the Lady remembers you came this way before) without changing the run's mechanics.
            if (SunderedCrown.Core.EndingsLog.RunsFinished > 0)
            {
                GameFlags.Current.SetBool("ng.plus", true);
                GameFlags.Current.SetInt("ng.priorRuns", SunderedCrown.Core.EndingsLog.RunsFinished);
            }

            var creation = new GameObject("CharacterCreation").AddComponent<CharacterCreationScreen>();
            creation.races = _content.Races;
            creation.classes = _content.Classes;
            creation.backgrounds = _content.Backgrounds;
            creation.OnComplete += OnHeroCreated;
        }

        // ---- save / continue ----

        private void ContinueGame()
        {
            SunderedCrown.Save.SaveSystem.Load(SaveSlot); // restores flags, quests, gold
            BuildCompanions();
            _hero = ReconstructHero(SunderedCrown.Save.SaveSystem.Last);
            Party.Instance.Recruit(_hero);
            RecruitFromFlags();
            GameFlags.Current.OnFlagChanged += OnFlagChanged;
            EnterHub();
        }

        private void Autosave() => SunderedCrown.Save.SaveSystem.Save(SaveSlot, "campaign");

        /// <summary>Build the companion roster sheets (without recruiting them).</summary>
        private void BuildCompanions()
        {
            if (_companions.Count > 0) return;
            _companions["garrow"] = BuildGarrow();
            _companions["roen"] = BuildRoen();
            _companions["varra"] = BuildVarra();
            _companions["naeve"] = _netheril.BuildNaeve();
            _companions["ilfaeril"] = _crownwars.BuildIlfaeril();
            _companions["maerin"] = _fugue.BuildMaerin();
        }

        /// <summary>On Continue, re-recruit everyone the flags say you have (and haven't lost).</summary>
        private void RecruitFromFlags()
        {
            foreach (var kv in _companions)
            {
                bool lost = GameFlags.Current.GetBool($"companion.{kv.Key}.lost");
                bool recruited = kv.Key == "garrow" || GameFlags.Current.GetBool($"companion.{kv.Key}.recruited");
                if (recruited && !lost) { ScaleToHero(kv.Value); Party.Instance.Recruit(kv.Value); }
            }
        }

        private CharacterSheet ReconstructHero(SunderedCrown.Save.SaveSystem.SaveData d)
        {
            var cls = (d != null ? _content.Classes.Find(c => c.className == d.heroClass) : null) ?? _content.Classes[0];
            var race = (d != null ? _content.Races.Find(r => r.raceName == d.heroRace) : null) ?? _content.Races[0];
            var s = new CharacterSheet
            {
                displayName = (d != null && !string.IsNullOrEmpty(d.heroName)) ? d.heroName : "The Returned",
                classDef = cls, raceDef = race, level = (d != null) ? Mathf.Max(1, d.heroLevel) : 1, baseArmorClass = 10
            };
            if (d != null && d.heroScores != null && d.heroScores.Count >= 6)
            {
                int i = 0;
                foreach (Ability a in System.Enum.GetValues(typeof(Ability))) s.abilities.Set(a, d.heroScores[i++]);
            }
            s.RecalculateMaxHitPoints();
            if (cls.startingAbilities != null && cls.startingAbilities.Length > 0 && cls.startingAbilities[0] != null)
            {
                s.knownAbilities.Add(cls.startingAbilities[0]);
                s.equippedWeaponAbility = cls.startingAbilities[0];
            }
            if (cls.isSpellcaster) s.spellSlots.max[1] = 2;
            return s;
        }

        private void EnsureCore()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();
        }

        private void OnHeroCreated(CharacterSheet hero)
        {
            _hero = hero;
            if (hero.knownAbilities.Count > 0) hero.equippedWeaponAbility = hero.knownAbilities[0];
            if (hero.classDef != null && hero.classDef.isSpellcaster) hero.spellSlots.max[1] = 2;
            Party.Instance.Recruit(hero);

            // A modest starting kit so the Returned isn't penniless and bare at the threshold.
            var inv = Party.Instance.inventory;
            inv.AddGold(40);
            inv.Add(_content.Items["healing_potion"], 2);
            inv.Add(_content.Items["leather_armor"], 1);

            // Sister Garrow joins from the start; the rest are recruitable across the game.
            BuildCompanions();
            Party.Instance.Recruit(_companions["garrow"]);

            // Watch for dialogue-driven recruitment.
            GameFlags.Current.OnFlagChanged += OnFlagChanged;

            QuestManager.Instance.StartQuest(_content.FirstQuest.questId);
            EnterHub();
        }

        /// <summary>Bring a recruited companion up to at least the hero's level (never down), refreshing HP and
        /// proficiency, so a late join isn't dead weight. Keeps their hand-authored ability kit intact —
        /// only the level-driven stats (HP, to-hit, proficiency) scale.</summary>
        private void ScaleToHero(CharacterSheet c)
        {
            if (_hero == null || c == null || c.level >= _hero.level) return;
            c.level = _hero.level;
            c.RecalculateMaxHitPoints();
            c.currentHitPoints = c.maxHitPoints;
        }

        private void OnFlagChanged(string key)
        {
            foreach (var kv in _companions)
                if (GameFlags.Current.GetBool($"companion.{kv.Key}.recruited") &&
                    !Party.Instance.roster.Contains(kv.Value))
                {
                    ScaleToHero(kv.Value); // a late recruit shouldn't join under-leveled
                    Party.Instance.Recruit(kv.Value);
                    Debug.Log($"[Campaign] {kv.Value.displayName} joined the party!");
                }

            // Tally beckons you into the Vault of Tens.
            if (key == "vault.requested" && GameFlags.Current.GetBool("vault.requested"))
            {
                GameFlags.Current.SetBool("vault.requested", false); // consume the request
                EnterVault();
            }

            // Pulling Maerin from the Wall triggers the Breach (the permanent loss).
            if (key == "fugue.pull_maerin" && GameFlags.Current.GetBool("fugue.pull_maerin"))
                DoBreach();
        }

        // ---- The Fugue Plane & the Breach (Act II: the first permanent loss) ----

        private void EnterFugue()
        {
            SwapMode("Fugue");
            SunderedCrown.UI.ExplorationHUD.Location = "The Fugue — the Wall of the Faithless";
            var era = _modeRoot.AddComponent<FugueEra>();
            era.content = _content;
            era.fugue = _fugue;
            era.leaderSheet = _hero;
            era.onLeave = EnterHub;
            era.Begin();
        }

        /// <summary>The Wall does not give without taking. A soul out means a soul in — forever.</summary>
        private void DoBreach()
        {
            if (GameFlags.Current.GetBool("act2.breach_done")) return;
            GameFlags.Current.SetBool("act2.breach_done", true);

            var victim = ChooseBreachVictim();
            if (victim == null) return;
            string id = IdOf(victim);
            Party.Instance.Remove(victim);
            if (id != null) GameFlags.Current.SetBool($"companion.{id}.lost", true);
            Debug.Log($"[The Breach] The Wall took {victim.displayName} as its tithe for Maerin — permanently. " +
                      "There is no revival in this game, by design. The party screen will carry the hole forever.");
        }

        /// <summary>Authored fate: who the Wall takes depends on the player's earlier care (doc 15 §II).</summary>
        private CharacterSheet ChooseBreachVictim()
        {
            var cands = new List<CharacterSheet>();
            foreach (var s in Party.Instance.active)
                if (s != _hero && IdOf(s) != "maerin") cands.Add(s);
            if (cands.Count == 0) return null;

            // 1) Varra, if her pact is intact — the canonical default (her Act I 'leash' pays off).
            if (_companions.TryGetValue("varra", out var varra) && cands.Contains(varra) &&
                !GameFlags.Current.GetBool("companion.varra.pact_broken"))
                return varra;

            // 1b) Else the Wall is drawn to what you love most: a committed romance among the candidates.
            //     (Love raises what you stand to lose — doc 16. The cruelest, most thematic tithe.)
            foreach (var rid in new[] { "garrow", "roen", "naeve" })
                if (GameFlags.Current.GetBool($"romance.{rid}.turn") &&
                    _companions.TryGetValue(rid, out var lover) && cands.Contains(lover))
                    return lover;

            // 2) Else a companion volunteers (eyes open, Mordin-style).
            foreach (var vid in new[] { "ilfaeril", "garrow", "naeve" })
                if (_companions.TryGetValue(vid, out var s) && cands.Contains(s))
                    return s;

            // 3) Else the Wall takes the one you let in least.
            CharacterSheet low = null; int lowApproval = int.MaxValue;
            foreach (var s in cands)
            {
                int a = GameFlags.Current.GetInt($"companion.{IdOf(s)}.approval");
                if (a < lowApproval) { lowApproval = a; low = s; }
            }
            return low;
        }

        private string IdOf(CharacterSheet s)
        {
            foreach (var kv in _companions) if (kv.Value == s) return kv.Key;
            return null;
        }

        private CharacterSheet BuildVarra()
        {
            var rogue = _content.Classes.Find(c => c.className == "Rogue");
            var tief = _content.Races.Find(r => r.raceName == "Tiefling") ?? _content.Races[0];
            var s = new CharacterSheet { displayName = "Varra", classDef = rogue, raceDef = tief, level = 2, baseArmorClass = 12 };
            s.abilities.Set(Ability.Charisma, 16);
            s.abilities.Set(Ability.Dexterity, 14);
            s.abilities.Set(Ability.Constitution, 12);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(_content.Abilities["firebolt"]); // eldritch-blast flavour
            s.knownAbilities.Add(_content.Abilities["dagger"]);
            s.equippedWeaponAbility = _content.Abilities["firebolt"];
            return s;
        }

        // ---- The Court of the Dead (the finale) ----

        private void EnterFinale()
        {
            SwapMode("Finale");
            var screen = _modeRoot.AddComponent<EndingScreen>();
            screen.onLeave = EnterHub;
        }

        // ---- The Vault of Tens (the Lady in the Margins' riddle room) ----

        private void EnterCamp()
        {
            SwapMode("Camp");
            var camp = _modeRoot.AddComponent<SunderedCrown.UI.CampScene>();
            camp.onLeave = EnterHub;
            camp.onRested = Autosave;   // a long rest is a natural checkpoint
        }

        private void EnterMarket()
        {
            SwapMode("Market");
            var m = _modeRoot.AddComponent<MarketScene>();
            m.leaderSheet = _hero; m.onLeave = EnterHub; m.Begin();
        }

        private void EnterDocks()
        {
            SwapMode("Docks");
            var d = _modeRoot.AddComponent<DocksScene>();
            d.leaderSheet = _hero; d.onLeave = EnterHub; d.Begin();
        }

        private void EnterSafehouse()
        {
            SwapMode("Safehouse");
            var sh = _modeRoot.AddComponent<HarperSafehouseScene>();
            sh.leaderSheet = _hero; sh.onLeave = EnterHub; sh.Begin();
        }

        private void OpenShop()
        {
            if (_modeRoot == null) return;
            var shop = _modeRoot.AddComponent<SunderedCrown.UI.ShopScreen>(); // overlay on the hub; no mode-swap
            int frep = GameFlags.Current.GetInt("reputation.lowcity");
            shop.vendorQuote = frep >= 5
                ? "\"For you? The good shelf. The *back* shelf. The quarter says you're solid, and Sczerla listens to the quarter — it's the only landlord I've never cheated.\""
                : frep <= -2
                    ? "\"...Your coin spends like anyone's, I suppose. Don't touch what you can't pay for, and don't expect the friendly price. The quarter talks, Returned, and it hasn't been kind about you.\""
                    : "\"Coin's coin, friend. Though I keep my better shelf for folk the quarter speaks well of.\"";
            shop.onClose = () => Destroy(shop);
        }

        private void EnterAlmshouse()
        {
            SwapMode("Almshouse");
            var alms = _modeRoot.AddComponent<AlmshouseScene>();
            alms.leaderSheet = _hero;
            alms.onLeave = EnterHub;
            alms.Begin();
        }

        // ---- companion personal quests (data-driven via PersonalQuestScene) ----

        private PersonalQuest QuestById(string id) => id switch
        {
            "roen"   => _roenQuest.Quest,
            "varra"  => _varraQuest.Quest,
            "garrow"   => _garrowQuest.Quest,
            "naeve"    => _naeveQuest.Quest,
            "ilfaeril" => _ilfaerilQuest.Quest,
            _ => null,
        };

        private void EnterPersonalQuest(string id) => EnterPersonalQuestAt(id, new Vector2Int(3, 7));

        private void EnterPersonalQuestAt(string id, Vector2Int entry)
        {
            var quest = QuestById(id);
            if (quest == null) { EnterHub(); return; }
            SwapMode("Quest_" + id);
            var scene = _modeRoot.AddComponent<PersonalQuestScene>();
            scene.leaderSheet = _hero;
            scene.quest = quest;
            scene.entryCoord = entry;
            scene.onStartFight = StartPersonalFight;
            scene.onLeave = EnterHub;
            scene.Begin();
        }

        private void StartPersonalFight(string id, Vector2Int returnCoord)
        {
            var quest = QuestById(id);
            SwapMode("Encounter");
            var enc = _modeRoot.AddComponent<EncounterBuilder>();
            enc.combatants = id switch
            {
                "varra"  => BuildVarraEncounter(),
                "garrow"   => BuildGarrowEncounter(),
                "naeve"    => BuildNaeveEncounter(),
                "ilfaeril" => BuildIlfaerilEncounter(),
                _ => BuildRoenEncounter(),
            };
            enc.onEnded = win =>
            {
                if (win && quest != null) GameFlags.Current.SetBool(quest.clearedFlag, true);
                EnterPersonalQuestAt(id, returnCoord);
            };
            enc.Begin();
        }

        // ---- Act II skirmish: the Faithless Choir's militant cell (an optional miniboss) ----
        private void EnterChoirCell()
        {
            SwapMode("Encounter");
            var enc = _modeRoot.AddComponent<EncounterBuilder>();
            enc.combatants = BuildChoirCellEncounter();
            enc.onEnded = win =>
            {
                if (win)
                {
                    var f = GameFlags.Current;
                    f.SetBool("quest.choir.cell_cleared", true);
                    f.AddInt("reputation.lowcity", 2);
                    f.AdjustApproval("garrow", 4);
                    f.AdjustApproval("ilfaeril", 4);
                }
                EnterHub();
            };
            enc.Begin();
        }

        private List<Combatant> BuildChoirCellEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var blade = _content.Abilities["longsword"];
            var bolt = _content.Abilities.ContainsKey("firebolt") ? _content.Abilities["firebolt"] : blade;
            var pale = new Color(0.58f, 0.56f, 0.66f);
            // The Hollow Cantor leads a militant cell: two unmaking zealots, two sorrow-wraiths.
            list.Add(new Combatant { sheet = Enemy("The Hollow Cantor", monster, 16, 17, 6, bolt, 280, _content.Items["chain_shirt"]), coord = new Vector2Int(12, 7), color = new Color(0.45f, 0.42f, 0.55f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Unmaking Zealot", monster, 15, 14, 4, blade, 120), coord = new Vector2Int(13, 5), color = pale, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Unmaking Zealot", monster, 15, 14, 4, blade, 120), coord = new Vector2Int(13, 9), color = pale, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Sorrow-wraith", monster, 12, 12, 3, bolt, 75), coord = new Vector2Int(15, 6), color = new Color(0.4f, 0.38f, 0.5f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Sorrow-wraith", monster, 12, 12, 3, bolt, 75), coord = new Vector2Int(15, 8), color = new Color(0.4f, 0.38f, 0.5f), faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildRoenEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var blade = _content.Abilities["longsword"];
            var red = Color.red;
            // A Doomguide cell: two knights, two enforcers, a zealot, an interrogator.
            list.Add(new Combatant { sheet = Enemy("Doomguide Knight", monster, 16, 15, 4, blade, 150, _content.Items["chain_shirt"]), coord = new Vector2Int(13, 5), color = red, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Doomguide Knight", monster, 16, 15, 4, blade, 150, _content.Items["leather_armor"]), coord = new Vector2Int(13, 9), color = red, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Doomguide Enforcer", monster, 14, 13, 3, blade, 90), coord = new Vector2Int(14, 6), color = new Color(0.7f, 0.25f, 0.25f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Doomguide Enforcer", monster, 14, 13, 3, blade, 90), coord = new Vector2Int(14, 8), color = new Color(0.7f, 0.25f, 0.25f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Kelemvorite Zealot", monster, 13, 12, 3, blade, 80), coord = new Vector2Int(15, 7), color = new Color(0.55f, 0.3f, 0.3f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Doomguide Interrogator", monster, 12, 14, 4, blade, 120, _content.Items["healing_potion"]), coord = new Vector2Int(12, 7), color = new Color(0.45f, 0.15f, 0.15f), faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildVarraEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var claw = _content.Abilities.ContainsKey("rotting_claw") ? _content.Abilities["rotting_claw"] : _content.Abilities["longsword"];
            var bolt = _content.Abilities.ContainsKey("firebolt") ? _content.Abilities["firebolt"] : _content.Abilities["longsword"];
            var hot = new Color(0.7f, 0.2f, 0.15f);
            // Quill's collection: the cambion broker, two contract-devils, and lesser imps of the fine print.
            list.Add(new Combatant { sheet = Enemy("Quill, the Broker", monster, 15, 16, 5, bolt, 220, _content.Items["leather_armor"]), coord = new Vector2Int(12, 7), color = new Color(0.55f, 0.15f, 0.2f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Contract-Devil", monster, 15, 14, 4, claw, 130), coord = new Vector2Int(13, 5), color = hot, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Contract-Devil", monster, 15, 14, 4, claw, 130), coord = new Vector2Int(13, 9), color = hot, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Imp of the Fine Print", monster, 11, 11, 2, bolt, 60), coord = new Vector2Int(15, 6), color = new Color(0.8f, 0.4f, 0.2f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Imp of the Fine Print", monster, 11, 11, 2, bolt, 60), coord = new Vector2Int(15, 8), color = new Color(0.8f, 0.4f, 0.2f), faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildGarrowEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var blade = _content.Abilities["longsword"];
            var grey = new Color(0.4f, 0.42f, 0.48f);
            // The Justiciar's tribunal: Veld himself, two templar inquisitors, two grey enforcers of the Doom.
            list.Add(new Combatant { sheet = Enemy("Justiciar Veld", monster, 17, 16, 5, blade, 220, _content.Items["chain_shirt"]), coord = new Vector2Int(12, 7), color = new Color(0.3f, 0.32f, 0.4f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Templar Inquisitor", monster, 15, 14, 4, blade, 120, _content.Items["leather_armor"]), coord = new Vector2Int(13, 5), color = grey, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Templar Inquisitor", monster, 15, 14, 4, blade, 120, _content.Items["leather_armor"]), coord = new Vector2Int(13, 9), color = grey, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Doomguide Enforcer", monster, 14, 13, 3, blade, 90), coord = new Vector2Int(15, 6), color = new Color(0.5f, 0.5f, 0.55f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Doomguide Enforcer", monster, 14, 13, 3, blade, 90), coord = new Vector2Int(15, 8), color = new Color(0.5f, 0.5f, 0.55f), faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildNaeveEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var bolt = _content.Abilities.ContainsKey("firebolt") ? _content.Abilities["firebolt"] : _content.Abilities["longsword"];
            var burst = _content.Abilities.ContainsKey("fireball") ? _content.Abilities["fireball"] : bolt;
            var arc = new Color(0.35f, 0.5f, 0.85f);
            // The core's last protocol wakes: a mythallar ward, two arcane sentinels, two Weave-wraiths.
            list.Add(new Combatant { sheet = Enemy("Mythallar Ward", monster, 16, 16, 5, burst, 220), coord = new Vector2Int(12, 7), color = new Color(0.5f, 0.65f, 1f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Arcane Sentinel", monster, 15, 14, 4, bolt, 120, _content.Items["chain_shirt"]), coord = new Vector2Int(13, 5), color = arc, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Arcane Sentinel", monster, 15, 14, 4, bolt, 120, _content.Items["chain_shirt"]), coord = new Vector2Int(13, 9), color = arc, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Weave-wraith", monster, 12, 12, 3, bolt, 70), coord = new Vector2Int(15, 6), color = new Color(0.45f, 0.4f, 0.7f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Weave-wraith", monster, 12, 12, 3, bolt, 70), coord = new Vector2Int(15, 8), color = new Color(0.45f, 0.4f, 0.7f), faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildIlfaerilEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var blade = _content.Abilities["longsword"];
            var bolt = _content.Abilities.ContainsKey("firebolt") ? _content.Abilities["firebolt"] : blade;
            var pale = new Color(0.62f, 0.6f, 0.7f);
            // The Choir of the Unmade, come for the witness: the Pale Cantor, two heralds, two unmaking acolytes.
            list.Add(new Combatant { sheet = Enemy("The Pale Cantor", monster, 16, 16, 5, bolt, 220, _content.Items["leather_armor"]), coord = new Vector2Int(12, 7), color = new Color(0.5f, 0.48f, 0.62f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Choir Herald", monster, 15, 14, 4, blade, 120, _content.Items["chain_shirt"]), coord = new Vector2Int(13, 5), color = pale, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Choir Herald", monster, 15, 14, 4, blade, 120, _content.Items["chain_shirt"]), coord = new Vector2Int(13, 9), color = pale, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Unmaking Acolyte", monster, 12, 12, 3, bolt, 70), coord = new Vector2Int(15, 6), color = new Color(0.4f, 0.38f, 0.5f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Unmaking Acolyte", monster, 12, 12, 3, bolt, 70), coord = new Vector2Int(15, 8), color = new Color(0.4f, 0.38f, 0.5f), faction = Faction.Enemy });
            return list;
        }

        private void EnterVault()
        {
            SwapMode("Vault");
            SunderedCrown.UI.ExplorationHUD.Location = "The Vault of Tens — the Lady's margins";
            GameFlags.Current.SetBool("riddle.entered", true); // unlocks the Codex entry for the Vault
            var vault = _modeRoot.AddComponent<RiddleVault>();
            vault.content = _riddles;
            vault.leaderSheet = _hero;
            vault.grantTokens = true;
            vault.onLeave = EnterHub;
            vault.Begin();
        }

        // ---- mode transitions ----

        private void EnterHub()
        {
            SwapMode("Hub");
            SunderedCrown.UI.ExplorationHUD.Location = "Baldur's Gate — the Lower City";
            var hub = _modeRoot.AddComponent<BaldursGateHub>();
            hub.content = _content;
            hub.leaderSheet = _hero;
            hub.onEnterDungeon = EnterDungeon;
            hub.onEnterNetheril = EnterNetheril;
            hub.onEnterCrownWars = EnterCrownWars;
            hub.onEnterTimeOfTroubles = EnterTimeOfTroubles;
            hub.onEnterSpellplague = EnterSpellplague;
            hub.onEnterFugue = EnterFugue;
            hub.onEnterFinale = EnterFinale;
            hub.onEnterCamp = EnterCamp;
            hub.onEnterPersonalQuest = EnterPersonalQuest;
            hub.onEnterChoirCell = EnterChoirCell;
            hub.onEnterAlmshouse = EnterAlmshouse;
            hub.onEnterShop = OpenShop;
            hub.onEnterMarket = EnterMarket;
            hub.onEnterDocks = EnterDocks;
            hub.onEnterSafehouse = EnterSafehouse;
            hub.aldricDialogue = _aldric.TeaDialogue;
            hub.tallyDialogue = _riddles.TallyIntro;
            hub.tallyRoamingDialogue = _riddles.RoamingRiddle;
            hub.actTwo = _actTwo;
            hub.Begin();

            Autosave(); // checkpoint every time you reach the hub
        }

        private void EnterDungeon() => EnterDungeonAt(new Vector2Int(2, 7));

        private void EnterDungeonAt(Vector2Int entry)
        {
            SwapMode("Dungeon");
            SunderedCrown.UI.ExplorationHUD.Location = "The Cinderhaunt — beneath the Gate";
            var d = _modeRoot.AddComponent<Cinderhaunt>();
            d.content = _content;
            d.leaderSheet = _hero;
            d.entryCoord = entry;
            d.onStartFight = StartDungeonFight;
            d.onLeaveDungeon = EnterHub;
            d.onReenter = EnterDungeonAt;
            d.Begin();
        }

        private void StartDungeonFight(string id, Vector2Int returnCoord)
        {
            SwapMode("Encounter");
            var enc = _modeRoot.AddComponent<EncounterBuilder>();
            enc.combatants = BuildDungeonEncounter(id);
            enc.onEnded = win =>
            {
                if (!win) { EnterDungeonAt(returnCoord); return; } // wiped → recover at the door (forgiving prototype)

                GameFlags.Current.SetBool($"dungeon.cinder.{id}", true);

                if (id == "guards")
                {
                    if (!Party.Instance.inventory.Has("cinderhaunt_key"))
                        Party.Instance.inventory.Add(_content.Items["cinderhaunt_key"]);
                    GameFlags.Current.SetBool("dungeon.cinder.key", true);
                }

                if (id == "boss")
                {
                    GameFlags.Current.SetBool("prologue.cleared", true);
                    EnterHub(); // the road to Aldric opens; back to town
                    return;
                }

                EnterDungeonAt(returnCoord);
            };
            enc.Begin();
        }

        // ---- Netheril (Act III, era 1: the falling city) ----

        private void EnterNetheril() => EnterNetherilAt(new Vector2Int(3, 7));

        private void EnterNetherilAt(Vector2Int entry)
        {
            SwapMode("Netheril"); SunderedCrown.UI.ExplorationHUD.Location = "Netheril — the Falling Sky (−339 DR)";
            var era = _modeRoot.AddComponent<NetherilEra>();
            era.content = _content;
            era.netheril = _netheril;
            era.leaderSheet = _hero;
            era.entryCoord = entry;
            era.onStartFight = StartNetherilFight;
            era.onLeave = EnterHub;
            era.onReenter = EnterNetherilAt;
            era.Begin();
        }

        private void StartNetherilFight(string id, Vector2Int returnCoord)
        {
            SwapMode("Encounter");
            var enc = _modeRoot.AddComponent<EncounterBuilder>();
            enc.environmentalHazard = true; // THE FALLING CITY — the floor collapses each turn

            if (id == "netheril_boss")
            {
                enc.combatants = BuildNetherilBossEncounter();
                enc.onEnded = win =>
                {
                    if (win) { GameFlags.Current.SetBool("netheril.boss_down", true); GameFlags.Current.AdjustApproval("naeve", 4); }
                    EnterNetherilAt(returnCoord);
                };
                enc.Begin();
                return;
            }

            enc.combatants = BuildNetherilEncounter();
            enc.onEnded = win =>
            {
                if (!win) { EnterNetherilAt(returnCoord); return; }
                GameFlags.Current.SetBool("netheril.cleared", true);
                GameFlags.Current.SetBool("act3.netheril_done", true);
                EnterNetherilAt(returnCoord); // back to the era; the exit is now "ride the wreckage down"
            };
            enc.Begin();
        }

        private List<Combatant> BuildNetherilEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });

            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var slam = _netheril.Abilities["arcane_slam"];
            var lash = _netheril.Abilities["voidlash"];
            var bolt = _netheril.Abilities["arcane_bolt"];

            list.Add(new Combatant { sheet = Enemy("Netherese War-Construct", monster, 17, 18, 5, slam, 300, _content.Items["chain_shirt"]), coord = new Vector2Int(13, 6), color = new Color(0.8f, 0.7f, 0.3f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Shadow-Bound Sentinel", monster, 14, 13, 3, lash, 120), coord = new Vector2Int(14, 9), color = new Color(0.4f, 0.3f, 0.55f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Arcanist Revenant", monster, 12, 12, 3, bolt, 120), coord = new Vector2Int(12, 11), color = new Color(0.5f, 0.5f, 0.8f), faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildNetherilBossEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var slam = _netheril.Abilities["arcane_slam"];
            var bolt = _netheril.Abilities["arcane_bolt"];
            var gold = new Color(0.85f, 0.72f, 0.32f);
            // The mythallar's last guardian — a colossus + two war-constructs, in the collapsing floor.
            list.Add(new Combatant { sheet = Enemy("The Mythallar Colossus", monster, 19, 20, 8, slam, 420, _content.Items["chain_shirt"]), coord = new Vector2Int(12, 7), color = new Color(0.9f, 0.78f, 0.4f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Netherese War-Construct", monster, 16, 16, 5, slam, 180), coord = new Vector2Int(13, 5), color = gold, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Netherese War-Construct", monster, 16, 16, 5, slam, 180), coord = new Vector2Int(13, 9), color = gold, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Arcanist Revenant", monster, 13, 13, 4, bolt, 110), coord = new Vector2Int(15, 7), color = new Color(0.5f, 0.5f, 0.8f), faction = Faction.Enemy });
            return list;
        }

        // ---- Crown Wars (Act III, era 2: the first damnation) ----

        private void EnterCrownWars() => EnterCrownWarsAt(new Vector2Int(3, 7));

        private void EnterCrownWarsAt(Vector2Int entry)
        {
            SwapMode("CrownWars"); SunderedCrown.UI.ExplorationHUD.Location = "The Crown Wars — the First Damnation";
            var era = _modeRoot.AddComponent<CrownWarsEra>();
            era.content = _content;
            era.crownwars = _crownwars;
            era.leaderSheet = _hero;
            era.entryCoord = entry;
            era.onStartFight = StartCrownWarsFight;
            era.onLeave = EnterHub;
            era.onReenter = EnterCrownWarsAt;
            era.Begin();
        }

        private void StartCrownWarsFight(string id, Vector2Int returnCoord)
        {
            SwapMode("Encounter");
            var enc = _modeRoot.AddComponent<EncounterBuilder>();

            if (id == "crownwars_boss")
            {
                enc.combatants = BuildCrownWarsBossEncounter();
                enc.onEnded = win =>
                {
                    if (win) { GameFlags.Current.SetBool("crownwars.boss_down", true); GameFlags.Current.AdjustApproval("ilfaeril", 4); }
                    EnterCrownWarsAt(returnCoord);
                };
                enc.Begin();
                return;
            }

            enc.combatants = BuildCrownWarsEncounter();
            enc.onEnded = win =>
            {
                if (!win) { EnterCrownWarsAt(returnCoord); return; }
                GameFlags.Current.SetBool("crownwars.cleared", true);
                GameFlags.Current.SetBool("act3.crownwars_done", true);
                EnterCrownWarsAt(returnCoord);
            };
            enc.Begin();
        }

        private List<Combatant> BuildCrownWarsBossEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var blade = _content.Abilities["longsword"];
            var bolt = _content.Abilities.ContainsKey("firebolt") ? _content.Abilities["firebolt"] : blade;
            var shade = new Color(0.45f, 0.5f, 0.6f);
            // The First Unmade — the very soul the court voted to erase, risen in grief and fury, + shades.
            list.Add(new Combatant { sheet = Enemy("The First Unmade", monster, 18, 19, 8, bolt, 420, _content.Items["chain_shirt"]), coord = new Vector2Int(12, 7), color = new Color(0.55f, 0.6f, 0.72f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Damned Shade", monster, 15, 15, 5, blade, 170), coord = new Vector2Int(13, 5), color = shade, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Damned Shade", monster, 15, 15, 5, blade, 170), coord = new Vector2Int(13, 9), color = shade, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Echo of the Verdict", monster, 13, 13, 4, bolt, 110), coord = new Vector2Int(15, 7), color = new Color(0.5f, 0.45f, 0.55f), faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildCrownWarsEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });

            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var claw = _crownwars.Abilities["grave_claw"];

            list.Add(new Combatant { sheet = Enemy("Crown-War Revenant", monster, 16, 16, 4, claw, 200, _content.Items["leather_armor"]), coord = new Vector2Int(13, 5), color = new Color(0.5f, 0.5f, 0.6f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Vengeful Shade", monster, 12, 12, 3, claw, 120), coord = new Vector2Int(14, 8), color = new Color(0.3f, 0.35f, 0.45f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Crown-War Revenant", monster, 16, 16, 4, claw, 200), coord = new Vector2Int(12, 10), color = new Color(0.5f, 0.5f, 0.6f), faction = Faction.Enemy });
            return list;
        }

        // ---- Time of Troubles (Act IV, era 3: the forging of the Crown) ----

        private void EnterTimeOfTroubles() => EnterTimeOfTroublesAt(new Vector2Int(3, 7));

        private void EnterTimeOfTroublesAt(Vector2Int entry)
        {
            SwapMode("TimeOfTroubles"); SunderedCrown.UI.ExplorationHUD.Location = "The Time of Troubles (1358 DR)";
            var era = _modeRoot.AddComponent<SimpleEra>();
            era.leaderSheet = _hero; era.entryCoord = entry;
            era.sceneTitle = "TimeOfTroubles"; era.background = new Color(0.18f, 0.12f, 0.10f);
            era.arrivalDialogue = _lateEras.TimeOfTroublesArrival; era.arrivedFlag = "act4.crown_is_myrkul";
            era.clearedFlag = "act4.toot_done";
            era.examineLabel = "The Forging";
            era.examineText = "They are beating a god's skull into a crown of horns. The Crown-Voice that has whispered to you all this way — it is not a tool. It is Myrkul, and it has been riding the road in your pack.";
            era.fightId = "toot"; era.fightLabel = "Cut through the avatar-touched (battle)";
            era.witnessNameMatch = "Garrow"; era.witnessGraph = EraWitness.GarrowTimeOfTroubles;
            era.bonusFightId = "toot_avatar";
            era.bonusFightLabel = "Face the Avatar of Bone (optional miniboss)";
            era.bonusFightDoneFlag = "toot.avatar_down";
            era.onStartFight = StartLateFight; era.onLeave = EnterHub;
            era.Begin();
        }

        // ---- Spellplague (Act IV, era 4: where the ink runs) ----

        private void EnterSpellplague() => EnterSpellplagueAt(new Vector2Int(3, 7));

        private void EnterSpellplagueAt(Vector2Int entry)
        {
            SwapMode("Spellplague"); SunderedCrown.UI.ExplorationHUD.Location = "The Spellplague (1385 DR)";
            var era = _modeRoot.AddComponent<SimpleEra>();
            era.leaderSheet = _hero; era.entryCoord = entry;
            era.sceneTitle = "Spellplague"; era.background = new Color(0.08f, 0.12f, 0.22f);
            era.arrivalDialogue = _lateEras.SpellplagueArrival; era.arrivedFlag = "spellplague.arrived";
            era.clearedFlag = "spellplague.done";
            era.examineLabel = "The Wound in the World";
            era.examineText = "Blue fire pours from a tear in the sky, and the ground floats where it forgot to be ground. Here cause does not reliably precede effect. Here the Unmade comes closest to winning.";
            era.fightId = "spellplague"; era.fightLabel = "Fight through the blue fire (battle)";
            era.witnessNameMatch = "Varra"; era.witnessGraph = EraWitness.VarraSpellplague;
            era.bonusFightId = "spellplague_herald";
            era.bonusFightLabel = "Face the Herald of the Unmade (optional miniboss)";
            era.bonusFightDoneFlag = "spellplague.herald_down";
            era.onStartFight = StartLateFight; era.onLeave = EnterHub;
            era.Begin();
        }

        /// <summary>Both late eras route here; the id decides the roster, the hazard, and the cleared-flag.</summary>
        private void StartLateFight(string id, Vector2Int returnCoord)
        {
            SwapMode("Encounter");
            var enc = _modeRoot.AddComponent<EncounterBuilder>();

            // Optional Time of Troubles miniboss — a god-touched horror at the forging of the Crown.
            if (id == "toot_avatar")
            {
                enc.combatants = BuildAvatarEncounter();
                enc.onEnded = win =>
                {
                    if (win)
                    {
                        var f = GameFlags.Current;
                        f.SetBool("toot.avatar_down", true);
                        f.AdjustApproval("garrow", 5); // a Doomguide, at the death of death's own god
                    }
                    EnterTimeOfTroublesAt(returnCoord);
                };
                enc.Begin();
                return;
            }

            // Optional Spellplague miniboss — the Unmade's herald, in the place it comes closest to winning.
            if (id == "spellplague_herald")
            {
                enc.spellplagueHazard = true;
                enc.combatants = BuildHeraldEncounter();
                enc.onEnded = win =>
                {
                    if (win)
                    {
                        var f = GameFlags.Current;
                        f.SetBool("spellplague.herald_down", true);
                        f.AdjustApproval("naeve", 4);     // the two who understand what it is
                        f.AdjustApproval("ilfaeril", 4);
                    }
                    EnterSpellplagueAt(returnCoord);
                };
                enc.Begin();
                return;
            }

            bool spellplague = id == "spellplague";
            enc.spellplagueHazard = spellplague; // causality-optional in the Spellplague
            enc.combatants = BuildLateEncounter(spellplague);
            enc.onEnded = win =>
            {
                if (!win) { if (spellplague) EnterSpellplagueAt(returnCoord); else EnterTimeOfTroublesAt(returnCoord); return; }
                if (spellplague) { GameFlags.Current.SetBool("spellplague.done", true); GameFlags.Current.SetBool("act4.spellplague_done", true); EnterSpellplagueAt(returnCoord); }
                else { GameFlags.Current.SetBool("act4.toot_done", true); EnterTimeOfTroublesAt(returnCoord); }
            };
            enc.Begin();
        }

        private List<Combatant> BuildLateEncounter(bool spellplague)
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var atk = spellplague ? _lateEras.BlueFire : _lateEras.AvatarTouch;
            string name = spellplague ? "Spellplague Aberration" : "Avatar-Touched Horror";
            var color = spellplague ? new Color(0.3f, 0.5f, 0.9f) : new Color(0.7f, 0.6f, 0.3f);

            list.Add(new Combatant { sheet = Enemy(name, monster, 16, 17, 5, atk, 280, _content.Items["chain_shirt"]), coord = new Vector2Int(13, 5), color = color, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy(name, monster, 14, 14, 4, atk, 160), coord = new Vector2Int(14, 8), color = color, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy(name, monster, 14, 14, 4, atk, 160), coord = new Vector2Int(12, 10), color = color, faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildAvatarEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var touch = _lateEras.AvatarTouch;
            var bone = new Color(0.78f, 0.74f, 0.62f);
            // A fragment of dying divinity at the forge: the Avatar of Bone + two god-touched + a zealot.
            var avatarSheet = Enemy("The Avatar of Bone", monster, 18, 18, 7, touch, 380, _content.Items["half_plate"]);
            avatarSheet.spellSlots.max[5] = 2;                                        // a fragment of godhood: it can call down fire
            avatarSheet.knownAbilities.Insert(0, _content.Abilities["flame_strike"]); // opens with an AoE before reverting to melee
            list.Add(new Combatant { sheet = avatarSheet, coord = new Vector2Int(12, 7), color = new Color(0.85f, 0.8f, 0.6f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("God-Touched Horror", monster, 15, 15, 5, touch, 180), coord = new Vector2Int(13, 5), color = bone, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("God-Touched Horror", monster, 15, 15, 5, touch, 180), coord = new Vector2Int(13, 9), color = bone, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Bone-Zealot of Myrkul", monster, 13, 13, 4, touch, 110), coord = new Vector2Int(15, 7), color = new Color(0.6f, 0.58f, 0.5f), faction = Faction.Enemy });
            return list;
        }

        private List<Combatant> BuildHeraldEncounter()
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 4), new Vector2Int(2, 6), new Vector2Int(2, 8), new Vector2Int(2, 10)
            });
            var monster = _content.Classes.Find(c => c.className == "Fighter");
            var fire = _lateEras.BlueFire;
            var col = new Color(0.5f, 0.45f, 0.66f);
            // The Unmade's herald: a tanky named boss + aberrations + a sorrow, in the causality-optional fire.
            var heraldSheet = Enemy("The Herald of the Unmade", monster, 17, 18, 7, fire, 380, _content.Items["ring_protection"]);
            heraldSheet.spellSlots.max[4] = 2;                                      // calls a freezing storm on the clustered party
            heraldSheet.knownAbilities.Insert(0, _content.Abilities["ice_storm"]);  // opens with AoE, then reverts to melee fire
            list.Add(new Combatant { sheet = heraldSheet, coord = new Vector2Int(12, 7), color = new Color(0.42f, 0.38f, 0.6f), faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Unmade Aberration", monster, 15, 15, 5, fire, 180), coord = new Vector2Int(13, 5), color = col, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("Unmade Aberration", monster, 15, 15, 5, fire, 180), coord = new Vector2Int(13, 9), color = col, faction = Faction.Enemy });
            list.Add(new Combatant { sheet = Enemy("A Sorrow of the Unmade", monster, 13, 13, 4, fire, 110), coord = new Vector2Int(15, 7), color = new Color(0.38f, 0.36f, 0.5f), faction = Faction.Enemy });
            return list;
        }

        private void SwapMode(string name)
        {
            if (_modeRoot != null) Destroy(_modeRoot);
            _modeRoot = new GameObject($"Mode_{name}");
            SunderedCrown.FX.AudioSystem.PlayMusic(MusicFor(name)); // art-optional; no track = music carries over
        }

        /// <summary>Map a mode name to a music track (drop the matching clip in Resources/Music/).</summary>
        private static string MusicFor(string mode)
        {
            if (mode == "Hub") return "Hub";
            if (mode == "Camp") return "Camp";
            if (mode == "Fugue") return "Fugue";
            if (mode == "Finale") return "Court";
            if (mode == "Vault") return "Vault";
            if (mode == "Encounter" || mode == "Dungeon") return "Combat";
            return "Explore"; // eras, personal quests, etc.
        }

        // ---- encounter rosters ----

        private List<Combatant> BuildDungeonEncounter(string id)
        {
            var list = PartyCombatants(new[]
            {
                new Vector2Int(2, 5), new Vector2Int(2, 7), new Vector2Int(2, 9), new Vector2Int(2, 11)
            });

            var fighter = _content.Classes.Find(c => c.className == "Fighter");

            if (id == "boss")
            {
                var maw = Enemy("The Unbound Maw", fighter, 18, 18, 5, _content.Abilities["rotting_claw"], 400, _content.Items["chain_shirt"]);
                maw.maxHitPoints += 40; maw.currentHitPoints = maw.maxHitPoints; // boss bulk
                list.Add(new Combatant { sheet = maw, coord = new Vector2Int(16, 7), color = new Color(0.6f, 0.1f, 0.3f), faction = Faction.Enemy });
                list.Add(new Combatant { sheet = Enemy("Cinder-Hound", fighter, 14, 12, 2, _content.Abilities["rotting_claw"], 75), coord = new Vector2Int(15, 5), color = new Color(0.7f, 0.2f, 0.1f), faction = Faction.Enemy });
                list.Add(new Combatant { sheet = Enemy("Cinder-Hound", fighter, 14, 12, 2, _content.Abilities["rotting_claw"], 75), coord = new Vector2Int(15, 9), color = new Color(0.7f, 0.2f, 0.1f), faction = Faction.Enemy });
            }
            else // guards
            {
                list.Add(new Combatant { sheet = Enemy("Doomguide Knight", fighter, 15, 13, 2, _content.Abilities["longsword"], 100, _content.Items["healing_potion"]), coord = new Vector2Int(12, 6), color = Color.red, faction = Faction.Enemy });
                list.Add(new Combatant { sheet = Enemy("Doomguide Knight", fighter, 15, 13, 2, _content.Abilities["longsword"], 100, _content.Items["leather_armor"]), coord = new Vector2Int(13, 8), color = Color.red, faction = Faction.Enemy });
                list.Add(new Combatant { sheet = Enemy("The Unbound", fighter, 13, 14, 2, _content.Abilities["rotting_claw"], 50), coord = new Vector2Int(12, 10), color = new Color(0.5f, 0.2f, 0.4f), faction = Faction.Enemy });
            }
            return list;
        }

        /// <summary>Map the active party onto starting tiles (leader = Player, rest = Ally).</summary>
        private List<Combatant> PartyCombatants(Vector2Int[] coords)
        {
            var list = new List<Combatant>();
            var active = Party.Instance.active;
            for (int i = 0; i < active.Count && i < coords.Length; i++)
                list.Add(new Combatant
                {
                    sheet = active[i],
                    coord = coords[i],
                    color = i == 0 ? Color.blue : Color.cyan,
                    faction = i == 0 ? Faction.Player : Faction.Ally
                });
            return list;
        }

        // ---- companion / enemy sheets ----

        private CharacterSheet BuildGarrow()
        {
            var cleric = _content.Classes.Find(c => c.className == "Cleric");
            var s = new CharacterSheet { displayName = "Sister Garrow", classDef = cleric, raceDef = _content.Races[0], level = 2, baseArmorClass = 14 };
            s.abilities.Set(Ability.Strength, 12);
            s.abilities.Set(Ability.Dexterity, 12);
            s.abilities.Set(Ability.Constitution, 14);
            s.abilities.Set(Ability.Wisdom, 16);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(_content.Abilities["mace"]);
            s.knownAbilities.Add(_content.Abilities["cure_wounds"]);
            s.knownAbilities.Add(_content.Abilities["bless"]);
            s.equippedWeaponAbility = _content.Abilities["mace"];
            s.spellSlots.max[1] = 3;
            return s;
        }

        private CharacterSheet BuildRoen()
        {
            var rogue = _content.Classes.Find(c => c.className == "Rogue");
            var s = new CharacterSheet { displayName = "Roen Alleywind", classDef = rogue, raceDef = _content.Races[0], level = 2, baseArmorClass = 13 };
            s.abilities.Set(Ability.Strength, 10);
            s.abilities.Set(Ability.Dexterity, 16);
            s.abilities.Set(Ability.Constitution, 13);
            s.abilities.Set(Ability.Charisma, 15);
            s.RecalculateMaxHitPoints();
            s.knownAbilities.Add(_content.Abilities["shortbow"]);
            s.knownAbilities.Add(_content.Abilities["dagger"]);
            s.equippedWeaponAbility = _content.Abilities["shortbow"];
            return s;
        }

        private CharacterSheet Enemy(string name, ClassDefinition cls, int str, int con, int level, AbilityDefinition atk, int xp, ItemDefinition drop = null)
        {
            var s = new CharacterSheet { displayName = name, classDef = cls, raceDef = _content.Races[0], level = level, baseArmorClass = 13, experienceValue = xp };
            s.abilities.Set(Ability.Strength, str);
            s.abilities.Set(Ability.Dexterity, 13);
            s.abilities.Set(Ability.Constitution, con);
            s.RecalculateMaxHitPoints();
            float hpMult = GameSettings.EnemyHpMult; // difficulty: frailer (Story) / spongier (Hard)
            if (hpMult != 1f)
            {
                s.maxHitPoints = Mathf.Max(1, Mathf.RoundToInt(s.maxHitPoints * hpMult));
                s.currentHitPoints = s.maxHitPoints;
            }
            s.knownAbilities.Add(atk);
            s.equippedWeaponAbility = atk;
            s.lootGold = Mathf.Max(0, xp / 4);
            if (drop != null) s.loot.Add(drop);
            return s;
        }
    }
}
