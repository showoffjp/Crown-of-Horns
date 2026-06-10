// ============================================================================
// Leveling — headless port of Characters/Progression.cs. XP on the 5e table,
// pure operations on a sheet, so it runs without Unity.
// ============================================================================
const INT_MAX = 2147483647;
const MaxLevel = 20;
// Total XP required to BE a given level (index = level; index 0 unused).
const XpForLevel = [
  0,
  0, 300, 900, 2700, 6500, 14000, 23000, 34000, 48000, 64000,
  85000, 100000, 120000, 140000, 165000, 195000, 225000, 265000, 305000, 355000
];

const Progression = {
  MaxLevel,
  _listeners: [],
  onLevelUp(cb) { this._listeners.push(cb); },           // C# event OnLevelUp +=
  offLevelUp(cb) { this._listeners = this._listeners.filter(l => l !== cb); }, // -=
  _fire(s, lvl) { for (const cb of this._listeners) cb(s, lvl); },

  XpToReach(level) { return (level >= 1 && level <= MaxLevel) ? XpForLevel[level] : INT_MAX; },
  XpToNextLevel(s) { return s.level >= MaxLevel ? 0 : Math.max(0, XpForLevel[s.level + 1] - s.experience); },

  AwardExperience(sheet, amount) {
    if (!sheet || amount <= 0 || sheet.level >= MaxLevel) return 0;
    sheet.experience += amount;
    let gained = 0;
    while (sheet.level < MaxLevel && sheet.experience >= XpForLevel[sheet.level + 1]) { this.LevelUp(sheet); gained++; }
    return gained;
  },

  LevelUp(sheet) {
    sheet.level++;
    if (sheet.classDef != null) {
      const conMod = sheet.Modifier ? sheet.Modifier("constitution") : 0;
      const hpGain = Math.max(1, (sheet.classDef.AverageHitDieGain || 0) + conMod);
      sheet.maxHitPoints += hpGain;
      sheet.currentHitPoints += hpGain;
      const grants = sheet.classDef.startingAbilities;
      const idx = sheet.level - 1;
      if (grants && idx >= 0 && idx < grants.length && grants[idx]) {
        sheet.knownAbilities = sheet.knownAbilities || [];
        if (!sheet.knownAbilities.includes(grants[idx])) sheet.knownAbilities.push(grants[idx]);
      }
    }
    this._fire(sheet, sheet.level);
  }
};

module.exports = { Progression, XpForLevel, MaxLevel, INT_MAX };
