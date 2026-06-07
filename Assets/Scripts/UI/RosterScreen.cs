using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Core;
using SunderedCrown.Stats;

namespace SunderedCrown.UI
{
    /// <summary>
    /// Party management (toggle with P). Lists every companion you've recruited and lets you
    /// bench or field them, up to the party's maxActive. The protagonist (roster[0]) is locked
    /// into the field — you can't leave yourself behind. Click a name to expand a stat block
    /// (abilities, AC/HP, and the companion's approval of you). A thin wrapper over Party.SetActive.
    /// </summary>
    public class RosterScreen : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.P;
        private bool _open;
        private int _expanded = -1;
        private Vector2 _scroll;

        void Update() { if (Input.GetKeyDown(toggleKey)) _open = !_open; }

        void OnGUI()
        {
            if (!_open) return;
            var party = Party.Instance;
            if (party == null) return;

            const float w = 480, h = 520;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);
            GUILayout.Label($"<size=18><b>🛡️ THE PARTY</b></size>   <size=11>(press {toggleKey} to close)</size>");
            GUILayout.Label($"<color=#aaa>Fielded {party.active.Count}/{party.maxActive}. The protagonist always walks. Click a name for details.</color>");
            GUILayout.Space(8);

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < party.roster.Count; i++)
            {
                var m = party.roster[i];
                bool isLeader = i == 0;
                bool inField = party.active.Contains(m);

                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.BeginHorizontal();
                string state = inField ? "<color=#8f8>● fielded</color>" : "<color=#888>○ benched</color>";
                string lvl = $"L{m.level} {(m.classDef != null ? m.classDef.className : "")}".TrimEnd();
                if (GUILayout.Button($"  {state}  <b>{m.displayName}</b>  <color=#9fd>{lvl}</color>", NameButton(), GUILayout.Width(310)))
                    _expanded = _expanded == i ? -1 : i;

                GUILayout.FlexibleSpace();
                if (isLeader)
                {
                    GUI.enabled = false;
                    GUILayout.Button("Leader", GUILayout.Width(96));
                    GUI.enabled = true;
                }
                else if (inField)
                {
                    if (GUILayout.Button("Bench", GUILayout.Width(96)))
                        party.SetActive(m, false);
                }
                else
                {
                    GUI.enabled = party.active.Count < party.maxActive;
                    if (GUILayout.Button("Field", GUILayout.Width(96)))
                        party.SetActive(m, true);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();

                if (_expanded == i) DrawSheet(m, isLeader);
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();

            if (party.roster.Count <= 1)
                GUILayout.Label("<color=#888><i>No companions recruited yet. Talk to Roen and Varra at the Gate.</i></color>");

            GUILayout.FlexibleSpace();
            GUILayout.Label("<size=11><color=#888>Benched companions keep their levels and gear; field them before a fight.</color></size>");
            GUILayout.EndArea();
        }

        private void DrawSheet(CharacterSheet m, bool isLeader)
        {
            GUILayout.Space(2);
            GUILayout.Label(
                $"  <color=#ccc>AC <b>{m.ArmorClass}</b>   HP <b>{m.currentHitPoints}/{m.maxHitPoints}</b>   " +
                $"Init <b>{m.InitiativeModifier:+0;-0}</b>   Prof <b>+{m.ProficiencyBonus}</b></color>");
            GUILayout.Label(
                $"  <color=#9fd>STR</color> {Score(m, Ability.Strength)}  " +
                $"<color=#9fd>DEX</color> {Score(m, Ability.Dexterity)}  " +
                $"<color=#9fd>CON</color> {Score(m, Ability.Constitution)}  " +
                $"<color=#9fd>INT</color> {Score(m, Ability.Intelligence)}  " +
                $"<color=#9fd>WIS</color> {Score(m, Ability.Wisdom)}  " +
                $"<color=#9fd>CHA</color> {Score(m, Ability.Charisma)}");

            // Known abilities — the character's combat kit at a glance.
            if (m.knownAbilities != null && m.knownAbilities.Count > 0)
            {
                var names = new System.Collections.Generic.List<string>();
                foreach (var ab in m.knownAbilities) if (ab != null) names.Add(ab.abilityName);
                if (names.Count > 0)
                    GUILayout.Label($"  <color=#cba>Abilities:</color> <color=#ddd>{string.Join(" · ", names)}</color>");
            }

            if (!isLeader)
            {
                int approval = GameFlags.Current.GetInt($"companion.{IdOf(m)}.approval");
                GUILayout.Label($"  <color=#aaa>Approval of you:</color> {ApprovalBar(approval)} <color=#ccc>{approval:+0;-0}</color>  <color=#888><i>{ApprovalWord(approval)}</i></color>");
            }
        }

        private static string Score(CharacterSheet m, Ability a)
        {
            int v = m.abilities.Get(a), mod = m.Modifier(a);
            return $"<b>{v}</b><color=#888>({mod:+0;-0})</color>";
        }

        // Companion approval is keyed by a lowercase first-name id (companion.<id>.approval).
        private static string IdOf(CharacterSheet m)
        {
            if (string.IsNullOrEmpty(m.displayName)) return "?";
            string first = m.displayName.Split(' ')[0];
            return first.ToLowerInvariant();
        }

        private static string ApprovalBar(int approval)
        {
            int filled = Mathf.Clamp((approval + 100) / 20, 0, 10); // -100..100 → 0..10 pips
            var s = "<color=#8f8>";
            for (int i = 0; i < filled; i++) s += "▰";
            s += "</color><color=#444>";
            for (int i = filled; i < 10; i++) s += "▱";
            return s + "</color>";
        }

        private static string ApprovalWord(int a) =>
            a >= 60 ? "devoted" : a >= 25 ? "warm" : a > -25 ? "neutral" : a > -60 ? "cold" : "hostile";

        private static GUIStyle _nameBtn;
        private GUIStyle NameButton()
        {
            if (_nameBtn == null)
                _nameBtn = new GUIStyle(GUI.skin.label) { richText = true, alignment = TextAnchor.MiddleLeft };
            return _nameBtn;
        }
    }
}
