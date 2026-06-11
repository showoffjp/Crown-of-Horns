// ============================================================================
// Save round-trip — headless model of Assets/Scripts/Save/SaveSystem.cs's
// serialization CONTRACT: the flatten of GameFlags' dictionaries into the parallel
// key/value lists JsonUtility needs, a JSON hop, and the unflatten back. This is the
// bug-prone part (a key/value list desync corrupts every save); the Unity file I/O
// itself is covered by SaveSystemTests. Mirrors SaveData exactly.
// ============================================================================

// Flatten a GameFlags (.bools / .ints maps) into the SaveData DTO shape.
function flatten(flags, { sceneName = null } = {}) {
  const d = {
    version: "0.1.0", sceneName, savedAtUtc: new Date().toISOString(),
    boolKeys: [], boolValues: [], intKeys: [], intValues: [],
    questIds: [], questStatuses: [], partyGold: 0,
    heroName: null, heroClass: null, heroRace: null, heroLevel: 1, heroScores: []
  };
  for (const k of Object.keys(flags.bools)) { d.boolKeys.push(k); d.boolValues.push(flags.bools[k]); }
  for (const k of Object.keys(flags.ints)) { d.intKeys.push(k); d.intValues.push(flags.ints[k]); }
  return d;
}

const toJson = (d) => JSON.stringify(d, null, 2);
const fromJson = (s) => JSON.parse(s);

// Unflatten a DTO back into a fresh GameFlags (mirrors SaveSystem.Load).
function apply(d, GameFlags) {
  const f = new GameFlags();
  for (let i = 0; i < d.boolKeys.length; i++) f.bools[d.boolKeys[i]] = d.boolValues[i];
  for (let i = 0; i < d.intKeys.length; i++) f.ints[d.intKeys[i]] = d.intValues[i];
  return f;
}

module.exports = { flatten, toJson, fromJson, apply };
