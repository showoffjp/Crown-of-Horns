using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Combat;
using SunderedCrown.Dialogue;
using SunderedCrown.Quests;
using SunderedCrown.Stats;

namespace SunderedCrown.Content
{
    /// <summary>
    /// Builds a slice of Forgotten Realms content in CODE for the playable prologue:
    /// races, classes, backgrounds, a few abilities/spells, the opening dialogue with
    /// Aldric's herald, and the first quest. In a real project these are authored as
    /// ScriptableObject assets in Assets/Data (see docs/CONTENT_PIPELINE.md); building
    /// them here keeps the prologue self-contained and shows the data shapes.
    /// </summary>
    public class SwordCoastContent
    {
        public readonly List<RaceDefinition> Races = new();
        public readonly List<ClassDefinition> Classes = new();
        public readonly List<BackgroundDefinition> Backgrounds = new();
        public readonly Dictionary<string, AbilityDefinition> Abilities = new();
        public readonly Dictionary<string, StatusEffectDefinition> Effects = new();

        public DialogueGraph PrologueDialogue { get; private set; }
        public Quest FirstQuest { get; private set; }

        public SwordCoastContent()
        {
            BuildEffects();
            BuildAbilities();
            BuildRaces();
            BuildClasses();
            BuildBackgrounds();
            BuildPrologueDialogue();
            BuildFirstQuest();
        }

        // ---- effects ----
        private void BuildEffects()
        {
            Effects["poisoned"] = Effect("Poisoned", Condition.Poisoned, 3, bearerDisadv: true);
            var burning = Effect("Burning", Condition.Burning, 2);
            burning.damageOverTimeDice = "1d4"; burning.damageOverTimeType = DamageType.Fire;
            Effects["burning"] = burning;
            Effects["frightened"] = Effect("Frightened", Condition.Frightened, 2, bearerDisadv: true);
            Effects["blessed"] = Effect("Blessed", Condition.Blessed, 3, beneficial: true, atkMod: 2);
        }

        // ---- abilities (weapons + a few iconic spells) ----
        private void BuildAbilities()
        {
            Abilities["longsword"] = Weapon("Longsword", "1d8", DamageType.Slashing, 1);
            Abilities["mace"] = Weapon("Mace", "1d6", DamageType.Bludgeoning, 1);
            Abilities["shortbow"] = Weapon("Shortbow", "1d6", DamageType.Piercing, 8);
            Abilities["dagger"] = Weapon("Dagger", "1d4", DamageType.Piercing, 1);
            Abilities["greataxe"] = Weapon("Greataxe", "1d12", DamageType.Slashing, 1);

            Abilities["firebolt"] = Spell("Fire Bolt", "1d10", DamageType.Fire, range: 12,
                slot: 0, attack: true);
            var fireball = Spell("Fireball", "8d6", DamageType.Fire, range: 10, slot: 3, attack: false);
            fireball.saveAbility = Ability.Dexterity; fireball.saveForHalf = true;
            fireball.targeting = TargetingMode.AreaBurst; fireball.areaRadiusTiles = 3;
            fireball.addAbilityModToDamage = false; fireball.appliedEffect = Effects["burning"];
            Abilities["fireball"] = fireball;

            var heal = Spell("Cure Wounds", "1d8", DamageType.Radiant, range: 1, slot: 1, attack: false);
            heal.isHeal = true; heal.healDice = "1d8"; heal.targeting = TargetingMode.SingleAlly;
            Abilities["cure_wounds"] = heal;

            var bless = Spell("Bless", "", DamageType.Radiant, range: 6, slot: 1, attack: false);
            bless.targeting = TargetingMode.SingleAlly; bless.appliedEffect = Effects["blessed"];
            bless.damageDice = ""; Abilities["bless"] = bless;

            var claw = Weapon("Rotting Claw", "1d6", DamageType.Slashing, 1);
            claw.appliedEffect = Effects["poisoned"]; Abilities["rotting_claw"] = claw;
        }

        // ---- races ----
        private void BuildRaces()
        {
            Races.Add(Race("Human", 6, "Versatile and ambitious folk of the Sword Coast.",
                (Ability.Strength, 1), (Ability.Constitution, 1)));
            Races.Add(Race("High Elf", 6, "Graceful, long-lived, keen of mind.",
                (Ability.Dexterity, 2), (Ability.Intelligence, 1)));
            Races.Add(Race("Shield Dwarf", 5, "Stout warriors of the mountain holds.",
                (Ability.Constitution, 2), (Ability.Strength, 2)));
            Races.Add(Race("Lightfoot Halfling", 5, "Small, lucky, and unflappable.",
                (Ability.Dexterity, 2), (Ability.Charisma, 1)));
            Races.Add(Race("Tiefling", 6, "Infernal-blooded, mistrusted, resilient.",
                (Ability.Charisma, 2), (Ability.Intelligence, 1)));
            Races.Add(Race("Half-Orc", 6, "Fierce, enduring children of two worlds.",
                (Ability.Strength, 2), (Ability.Constitution, 1)));
        }

        // ---- classes ----
        private void BuildClasses()
        {
            Classes.Add(Class("Fighter", 10, Ability.Strength, Abilities["longsword"]));
            Classes.Add(Class("Wizard", 6, Ability.Intelligence, Abilities["firebolt"], caster: true, cast: Ability.Intelligence));
            Classes.Add(Class("Cleric", 8, Ability.Wisdom, Abilities["mace"], caster: true, cast: Ability.Wisdom));
            Classes.Add(Class("Rogue", 8, Ability.Dexterity, Abilities["dagger"]));
            Classes.Add(Class("Ranger", 10, Ability.Dexterity, Abilities["shortbow"]));
            Classes.Add(Class("Barbarian", 12, Ability.Strength, Abilities["greataxe"]));
        }

        // ---- backgrounds ----
        private void BuildBackgrounds()
        {
            Backgrounds.Add(Background("Candlekeep Acolyte",
                "Raised among the monks of the great library. You know things others have forgotten.",
                "bg.candlekeep_acolyte", new[] { "Arcana", "History" }));
            Backgrounds.Add(Background("Flaming Fist Deserter",
                "You once wore the Fist's colors in Baldur's Gate. You left. They remember.",
                "bg.flaming_fist", new[] { "Athletics", "Intimidation" }));
            Backgrounds.Add(Background("Doomguide's Ward",
                "You served Kelemvor's church under Aldric Morn — before his fall.",
                "bg.doomguide_ward", new[] { "Religion", "Medicine" }));
            Backgrounds.Add(Background("The Nameless",
                "You woke after your death with no past. The Wall remembers you even if you do not.",
                "bg.nameless", new[] { "Insight", "Survival" }));
        }

        // ---- prologue dialogue: Aldric's herald, just after your death ----
        private void BuildPrologueDialogue()
        {
            var g = ScriptableObject.CreateInstance<DialogueGraph>();
            g.conversationId = "prologue.herald";
            g.startNodeId = "0";

            g.nodes.Add(Node("0", "The Herald",
                "You should be dead. I watched the harvest take you. And yet — here you stand, " +
                "with the Wall's chill still on your soul. Master Aldric will want to meet what he made.",
                autoNext: "1", onEnter: SetTrue("prologue.herald_met")));

            g.nodes.Add(Node("1", "The Herald",
                "He is not the monster the Doomguides paint. He would tear down the Wall of the " +
                "Faithless — the obscenity that swallows every soul who knelt to no god. Will you hear him?",
                choices: new[]
                {
                    Choice("I saw the Wall. He's right that it's monstrous — but I don't trust him.",
                        "2a", effects: AddInt("companion.garrow.approval", 5)),
                    Choice("[INSIGHT DC 12] You're terrified of him.", "2b",
                        check: (Ability.Wisdom, 12), failNode: "2c"),
                    Choice("Thousands will die for his crusade. Not while I breathe.", "2d",
                        effects: AddInt("faction.kelemvor.reputation", 5)),
                }));

            g.nodes.Add(Node("2a", "The Herald",
                "Honest. He'll like that. But the Doomguides are already here for you — they think " +
                "the Returned are a key to the Crown. Cut through them, and the road to Aldric opens.",
                autoNext: "fight"));
            g.nodes.Add(Node("2b", "The Herald",
                "...You see clearly. Yes. I fear what he is becoming. But the Wall frightens me more. " +
                "Survive the Doomguides at the door and decide for yourself.",
                autoNext: "fight", onEnter: SetTrue("prologue.herald_afraid")));
            g.nodes.Add(Node("2c", "The Herald",
                "Don't presume to read me, Returned. The Doomguides are at the door — survive them first.",
                autoNext: "fight"));
            g.nodes.Add(Node("2d", "The Herald",
                "Then you'll have to get through his enemies AND his friends. The Doomguides come for " +
                "you regardless — to them you're a relic to be locked away. Defend yourself.",
                autoNext: "fight"));

            g.nodes.Add(Node("fight", "The Herald",
                "They're here. Steel yourself, Returned — this is where your second life begins.",
                onEnter: SetTrue("prologue.fight_started")));

            PrologueDialogue = g;
        }

        private void BuildFirstQuest()
        {
            var q = ScriptableObject.CreateInstance<Quest>();
            q.questId = "q.prologue.returned";
            q.title = "What the Dead Owe";
            q.summary = "You died beneath Baldur's Gate and came back Returned. Survive the Doomguides " +
                        "at the harvest-site and decide what Aldric Morn is to you.";
            q.completionFlag = "prologue.cleared";
            q.experienceReward = 150;
            q.objectives = new List<QuestObjective>
            {
                new QuestObjective { objectiveId = "hear_herald", description = "Hear out Aldric's herald", completionFlag = "prologue.herald_met" },
                new QuestObjective { objectiveId = "survive", description = "Defeat the Doomguides at the door", completionFlag = "prologue.cleared" },
            };
            FirstQuest = q;
        }

        // ===== factory helpers =====

        private StatusEffectDefinition Effect(string name, Condition c, int rounds,
            bool bearerDisadv = false, bool beneficial = false, int atkMod = 0)
        {
            var e = ScriptableObject.CreateInstance<StatusEffectDefinition>();
            e.effectName = name; e.condition = c; e.durationRounds = rounds;
            e.bearerAttacksDisadvantage = bearerDisadv; e.isBeneficial = beneficial; e.attackRollModifier = atkMod;
            return e;
        }

        private AbilityDefinition Weapon(string name, string dice, DamageType type, int range)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = name; a.damageDice = dice; a.damageType = type; a.rangeTiles = range;
            a.isAttackRoll = true; return a;
        }

        private AbilityDefinition Spell(string name, string dice, DamageType type, int range, int slot, bool attack)
        {
            var a = ScriptableObject.CreateInstance<AbilityDefinition>();
            a.abilityName = name; a.damageDice = dice; a.damageType = type; a.rangeTiles = range;
            a.spellSlotLevel = slot; a.isAttackRoll = attack; return a;
        }

        private RaceDefinition Race(string name, int speed, string desc, params (Ability, int)[] bonuses)
        {
            var r = ScriptableObject.CreateInstance<RaceDefinition>();
            r.raceName = name; r.baseSpeedTiles = speed; r.description = desc;
            var list = new List<RaceDefinition.AbilityBonus>();
            foreach (var (ab, v) in bonuses) list.Add(new RaceDefinition.AbilityBonus { ability = ab, bonus = v });
            r.abilityBonuses = list.ToArray();
            return r;
        }

        private ClassDefinition Class(string name, int hitDie, Ability primary, AbilityDefinition starting,
            bool caster = false, Ability cast = Ability.Intelligence)
        {
            var c = ScriptableObject.CreateInstance<ClassDefinition>();
            c.className = name; c.hitDie = hitDie; c.primaryAbility = primary;
            c.isSpellcaster = caster; c.spellcastingAbility = cast;
            c.startingAbilities = new[] { starting };
            return c;
        }

        private BackgroundDefinition Background(string name, string desc, string flag, string[] skills)
        {
            var b = ScriptableObject.CreateInstance<BackgroundDefinition>();
            b.backgroundName = name; b.description = desc; b.grantsFlag = flag;
            b.skillProficiencies = skills; b.startingGold = 25;
            return b;
        }

        // ---- dialogue node/choice/clause sugar ----
        private DialogueNode Node(string id, string speaker, string text,
            DialogueChoice[] choices = null, string autoNext = null, FlagClause[] onEnter = null)
        {
            return new DialogueNode { id = id, speaker = speaker, text = text, choices = choices, autoNextNodeId = autoNext, onEnter = onEnter };
        }

        private DialogueChoice Choice(string text, string next, FlagClause[] effects = null,
            (Ability, int)? check = null, string failNode = null)
        {
            var c = new DialogueChoice { text = text, nextNodeId = next, effects = effects };
            if (check.HasValue) { c.checkAbility = check.Value.Item1; c.checkDC = check.Value.Item2; c.failNodeId = failNode; }
            return c;
        }

        private FlagClause[] SetTrue(string key) => new[] { new FlagClause { key = key, op = FlagOp.SetTrue } };
        private FlagClause[] AddInt(string key, int amount) => new[] { new FlagClause { key = key, op = FlagOp.AddInt, amount = amount } };
    }
}
