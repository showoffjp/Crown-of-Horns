using System;
using System.Collections.Generic;
using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Stats;

namespace SunderedCrown.UI
{
    /// <summary>
    /// A zero-setup character-creation flow (OnGUI) that drives CharacterBuilder:
    /// choose race / class / background, spend 27 point-buy points across the six
    /// abilities, name the hero, and Begin. Fires OnComplete(sheet). Replace with a
    /// styled uGUI screen later — the logic (CharacterBuilder) is UI-agnostic.
    ///
    /// Populate the option lists before enabling (e.g. from SwordCoastContent).
    /// </summary>
    public class CharacterCreationScreen : MonoBehaviour
    {
        public List<RaceDefinition> races = new List<RaceDefinition>();
        public List<ClassDefinition> classes = new List<ClassDefinition>();
        public List<BackgroundDefinition> backgrounds = new List<BackgroundDefinition>();

        public event Action<CharacterSheet> OnComplete;

        private int _race, _class, _bg;
        private string _name = "Adventurer";
        private readonly AbilityScores _scores = new AbilityScores();
        private bool _done;

        void Awake()
        {
            // All abilities start at the point-buy floor (8).
            foreach (Ability a in Enum.GetValues(typeof(Ability))) _scores.Set(a, 8);
        }

        void OnGUI()
        {
            if (_done || races.Count == 0 || classes.Count == 0) return;

            const float w = 460;
            float x = Screen.width / 2f - w / 2f;
            GUILayout.BeginArea(new Rect(x, 30, w, 560), GUI.skin.box);
            GUILayout.Label("<b>CREATE YOUR HERO — the Returned</b>");

            _name = NamedField("Name", _name);

            Selector("Race", races[_race].raceName, ref _race, races.Count);
            GUILayout.Label(RaceSummary(races[_race]));

            Selector("Class", classes[_class].className, ref _class, classes.Count);
            Selector("Background", backgrounds.Count > 0 ? backgrounds[_bg].backgroundName : "—", ref _bg, Math.Max(1, backgrounds.Count));

            GUILayout.Space(8);
            int spent = CharacterBuilder.PointsSpent(_scores);
            int remaining = CharacterBuilder.PointBuyBudget - Mathf.Max(0, spent);
            GUILayout.Label($"<b>Ability Scores</b>   (point-buy, {remaining} points left)");

            foreach (Ability a in Enum.GetValues(typeof(Ability)))
                AbilityRow(a, races[_race]);

            GUILayout.Space(10);
            bool legal = CharacterBuilder.IsLegalPointBuy(_scores);
            GUI.enabled = legal;
            if (GUILayout.Button(legal ? "BEGIN THE DESCENT" : "Spend points to continue"))
            {
                var bg = backgrounds.Count > 0 ? backgrounds[_bg] : null;
                var sheet = CharacterBuilder.Build(_name, races[_race], classes[_class], bg, _scores);
                _done = true;
                OnComplete?.Invoke(sheet);
                gameObject.SetActive(false);
            }
            GUI.enabled = true;

            GUILayout.EndArea();
        }

        private void AbilityRow(Ability a, RaceDefinition race)
        {
            GUILayout.BeginHorizontal();
            int baseV = _scores.Get(a);
            int racial = race != null ? race.BonusFor(a) : 0;
            int final = baseV + racial;
            int mod = (int)Math.Floor((final - 10) / 2.0);
            string racialTxt = racial != 0 ? $" (+{racial})" : "";
            GUILayout.Label($"{a,-13} {baseV}{racialTxt}  → {final} ({(mod >= 0 ? "+" : "")}{mod})", GUILayout.Width(300));

            GUI.enabled = baseV > CharacterBuilder.PointBuyMin;
            if (GUILayout.Button("-", GUILayout.Width(28))) _scores.Set(a, baseV - 1);
            GUI.enabled = baseV < CharacterBuilder.PointBuyMax &&
                          CharacterBuilder.PointsSpent(WithBump(a)) <= CharacterBuilder.PointBuyBudget;
            if (GUILayout.Button("+", GUILayout.Width(28))) _scores.Set(a, baseV + 1);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        // A copy of the scores with one ability bumped +1, for affordability checks.
        private AbilityScores WithBump(Ability a)
        {
            var copy = new AbilityScores();
            foreach (Ability x in Enum.GetValues(typeof(Ability))) copy.Set(x, _scores.Get(x));
            copy.Set(a, _scores.Get(a) + 1);
            return copy;
        }

        private void Selector(string label, string value, ref int index, int count)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(90));
            if (GUILayout.Button("◀", GUILayout.Width(28))) index = (index - 1 + count) % count;
            GUILayout.Label(value, GUILayout.Width(220));
            if (GUILayout.Button("▶", GUILayout.Width(28))) index = (index + 1) % count;
            GUILayout.EndHorizontal();
        }

        private string NamedField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(90));
            value = GUILayout.TextField(value, GUILayout.Width(220));
            GUILayout.EndHorizontal();
            return value;
        }

        private string RaceSummary(RaceDefinition r) =>
            r == null ? "" : $"   Speed {r.baseSpeedTiles} tiles. {r.description}";
    }
}
