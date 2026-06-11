// ============================================================================
// Quests — headless model of Assets/Scripts/Quests/QuestManager.cs + Quest.cs.
// The GameFlags-reactive quest state machine: explicit start, objective/completion/
// failure advancement, and the export/import the save system round-trips. Pins the
// LIVE reactive logic that the Unity side only exercises in PlayMode.
// ============================================================================

// GameFlags with the OnFlagChanged event the manager subscribes to (mirrors C#).
class GameFlags {
  constructor() { this.bools = {}; this.ints = {}; this._listeners = []; }
  GetBool(k) { return this.bools[k] === true; }
  SetBool(k, v) { this.bools[k] = v; this._fire(k); }
  GetInt(k) { return this.ints[k] || 0; }
  SetInt(k, v) { this.ints[k] = v; this._fire(k); }
  onFlagChanged(cb) { this._listeners.push(cb); }
  _fire(k) { for (const l of this._listeners) l(k); }
}

const QuestStatus = { Unstarted: "Unstarted", Active: "Active", Completed: "Completed", Failed: "Failed" };

const Quest = (o) => Object.assign({ questId: "", title: "", completionFlag: "", failureFlag: "", objectives: [] }, o);
const Objective = (o) => Object.assign({ objectiveId: "", completionFlag: "", optional: false, hidden: false }, o);

class QuestManager {
  constructor(flags, quests) {
    this.flags = flags;
    this.allQuests = quests || [];
    this._status = {};
    this.onQuestStarted = []; this.onQuestCompleted = []; this.onObjectiveCompleted = [];
    flags.onFlagChanged((k) => this._onFlagChanged(k)); // subscribe (== QuestManager.Start)
  }
  StatusOf(id) { return this._status[id] || QuestStatus.Unstarted; }
  StartQuest(id) {
    const q = this._find(id);
    if (!q || this.StatusOf(id) !== QuestStatus.Unstarted) return; // explicit, once
    this._status[id] = QuestStatus.Active;
    this.onQuestStarted.forEach((f) => f(q));
  }
  _onFlagChanged(key) {
    const flags = this.flags;
    for (const quest of this.allQuests) {
      if (!quest) continue;
      if (this.StatusOf(quest.questId) !== QuestStatus.Active) continue;

      for (const obj of quest.objectives)
        if (obj.completionFlag === key && flags.GetBool(key))
          this.onObjectiveCompleted.forEach((f) => f(quest, obj));

      if (quest.failureFlag && flags.GetBool(quest.failureFlag)) {   // failure checked BEFORE completion...
        this._status[quest.questId] = QuestStatus.Failed;
        continue;                                                     // ...and wins (the C# `continue`)
      }
      if (quest.completionFlag && flags.GetBool(quest.completionFlag)) {
        this._status[quest.questId] = QuestStatus.Completed;
        this.onQuestCompleted.forEach((f) => f(quest));
      }
    }
  }
  _find(id) { return this.allQuests.find((q) => q && q.questId === id); }

  ExportState() { return Object.assign({}, this._status); }          // detached copy
  ImportState(state) { this._status = {}; if (state) for (const k of Object.keys(state)) this._status[k] = state[k]; }
}

module.exports = { GameFlags, QuestStatus, Quest, Objective, QuestManager };
