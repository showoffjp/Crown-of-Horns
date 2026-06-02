using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SunderedCrown.Combat;
using SunderedCrown.Grid;

namespace SunderedCrown.UI
{
    /// <summary>
    /// A real uGUI combat HUD, constructed entirely at runtime so it works with ZERO
    /// scene/canvas setup (drop the component in and play). Shows party portraits with
    /// live HP bars, an initiative list, the active unit's vitals + action economy, a
    /// clickable ability bar that arms abilities (then click the world to target), a
    /// combat log, and an End Turn button.
    ///
    /// This replaces the IMGUI DebugCombatHUD for a shippable look. Swap the generated
    /// primitives for prefabs/sprites/TextMeshPro when you build art.
    /// </summary>
    public class CombatHUD : MonoBehaviour
    {
        private TurnManager _turns;
        private PlayerCombatInput _input;

        private Canvas _canvas;
        private Text _log, _initiative, _activeInfo;
        private RectTransform _abilityBar, _portraitRow;
        private Button _endTurnButton;

        private readonly List<string> _logLines = new List<string>();
        private readonly List<(GridUnit unit, RectTransform fill, Text label)> _portraits = new();
        private readonly List<Button> _abilityButtons = new();

        private GridUnit _lastActive;
        private bool _portraitsBuilt;

        private static readonly Color Panel = new Color(0.08f, 0.08f, 0.10f, 0.85f);
        private static readonly Color Accent = new Color(0.85f, 0.7f, 0.35f);

        void Start()
        {
            _turns = TurnManager.Instance;
            _input = FindFirstObjectByType<PlayerCombatInput>();
            BuildFrame();
            if (_turns != null) _turns.OnCombatLog += AppendLog;
        }

        void OnDestroy() { if (_turns != null) _turns.OnCombatLog -= AppendLog; }

        // ---- construction ------------------------------------------------

        private void BuildFrame()
        {
            EnsureEventSystem();

            var canvasGO = new GameObject("CombatHUD_Canvas");
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();

            // Active-unit info panel (top-left).
            var infoPanel = MakePanel("InfoPanel", new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(20, -20), new Vector2(380, 150));
            _activeInfo = MakeText(infoPanel, "", 22, TextAnchor.UpperLeft);

            // Portrait row (just under the info panel).
            _portraitRow = MakePanel("PortraitRow", new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(20, -185), new Vector2(380, 220));

            // Initiative list (top-right).
            var initPanel = MakePanel("InitPanel", new Vector2(1, 1), new Vector2(1, 1),
                new Vector2(-260, -20), new Vector2(240, 320));
            _initiative = MakeText(initPanel, "", 20, TextAnchor.UpperLeft);

            // Combat log (bottom-left).
            var logPanel = MakePanel("LogPanel", new Vector2(0, 0), new Vector2(0, 0),
                new Vector2(20, 20), new Vector2(620, 230));
            _log = MakeText(logPanel, "", 19, TextAnchor.LowerLeft);

            // Ability bar (bottom-center).
            _abilityBar = MakePanel("AbilityBar", new Vector2(0.5f, 0), new Vector2(0.5f, 0),
                new Vector2(0, 20), new Vector2(900, 90));
            _abilityBar.anchoredPosition = new Vector2(0, 60);
            var hl = _abilityBar.gameObject.AddComponent<HorizontalLayoutGroup>();
            hl.spacing = 8; hl.padding = new RectOffset(8, 8, 8, 8);
            hl.childControlWidth = true; hl.childForceExpandWidth = true; hl.childControlHeight = true;

            // End Turn button (bottom-right).
            _endTurnButton = MakeButton("EndTurn", "End Turn ▸", new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-180, 30), new Vector2(170, 60), () => _turns?.NextTurn());
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }
        }

        // ---- per-frame ---------------------------------------------------

        void Update()
        {
            if (_turns == null) return;
            bool inCombat = _turns.InCombat;
            _abilityBar.gameObject.SetActive(inCombat);
            _endTurnButton.gameObject.SetActive(inCombat);

            if (!inCombat) { _activeInfo.text = "<b>— Victory —</b>"; return; }

            if (!_portraitsBuilt) BuildPortraits();

            var active = _turns.ActiveUnit;
            RefreshActiveInfo(active);
            RefreshInitiative(active);
            RefreshPortraits();

            if (active != _lastActive) { _lastActive = active; RebuildAbilityBar(active); }
            RefreshAbilityBar(active);
        }

        private void RefreshActiveInfo(GridUnit active)
        {
            if (active == null) { _activeInfo.text = ""; return; }
            var s = active.Sheet;
            string cond = "";
            foreach (var e in s.activeEffects) if (e.def != null) cond += $"{e.def.effectName}({e.remainingRounds}) ";
            _activeInfo.text =
                $"<b>{s.displayName}</b>  [{active.faction}]\n" +
                $"HP {s.currentHitPoints}/{s.maxHitPoints}   AC {s.ArmorClass}\n" +
                $"Move {_turns.MovementRemaining}   " +
                $"Action {(_turns.ActionAvailable ? "●" : "—")}  Bonus {(_turns.BonusActionAvailable ? "●" : "—")}\n" +
                (cond.Length > 0 ? $"<color=#d88>{cond}</color>" : "");
        }

        private void RefreshInitiative(GridUnit active)
        {
            var sb = new System.Text.StringBuilder("<b>Initiative</b>\n");
            foreach (var u in _turns.TurnOrder)
            {
                string mark = u == active ? "▸ " : "   ";
                string dead = u.Sheet.IsAlive ? "" : " <color=#a55>(down)</color>";
                sb.AppendLine($"{mark}{u.Sheet.displayName} [{u.Sheet.currentHitPoints}]{dead}");
            }
            _initiative.text = sb.ToString();
        }

        // ---- portraits ---------------------------------------------------

        private void BuildPortraits()
        {
            float y = -6;
            foreach (var u in _turns.TurnOrder)
            {
                if (u.faction != Faction.Player && u.faction != Faction.Ally) continue;

                var card = MakeChildPanel(_portraitRow, new Vector2(0, 1), new Vector2(1, 1),
                    new Vector2(6, y), new Vector2(-12, 46));
                card.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.18f, 0.9f);

                var label = MakeText(card, u.Sheet.displayName, 18, TextAnchor.MiddleLeft);
                label.rectTransform.offsetMin = new Vector2(8, 0);

                // HP bar background + fill.
                var barBg = MakeChildPanel(card, new Vector2(0, 0), new Vector2(1, 0),
                    new Vector2(8, 4), new Vector2(-16, 10));
                barBg.GetComponent<Image>().color = new Color(0.3f, 0.05f, 0.05f, 1f);
                var fill = MakeChildPanel(barBg, new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
                fill.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.25f, 1f);

                _portraits.Add((u, fill, label));
                y -= 52;
            }
            _portraitsBuilt = true;
        }

        private void RefreshPortraits()
        {
            foreach (var (unit, fill, label) in _portraits)
            {
                float pct = unit.Sheet.maxHitPoints > 0
                    ? Mathf.Clamp01((float)unit.Sheet.currentHitPoints / unit.Sheet.maxHitPoints) : 0;
                fill.anchorMax = new Vector2(pct, 1);
                fill.offsetMin = Vector2.zero; fill.offsetMax = Vector2.zero;
                fill.GetComponent<Image>().color = unit.Sheet.IsAlive
                    ? new Color(0.2f, 0.7f, 0.25f, 1f) : new Color(0.4f, 0.4f, 0.4f, 1f);
                label.text = $"{unit.Sheet.displayName}  {unit.Sheet.currentHitPoints}/{unit.Sheet.maxHitPoints}";
            }
        }

        // ---- ability bar -------------------------------------------------

        private void RebuildAbilityBar(GridUnit active)
        {
            foreach (var b in _abilityButtons) if (b) Destroy(b.gameObject);
            _abilityButtons.Clear();
            if (active == null) return;

            bool player = active.faction == Faction.Player || active.faction == Faction.Ally;
            if (!player || _input == null) return;

            var known = active.Sheet.knownAbilities;
            for (int i = 0; i < known.Count; i++)
            {
                int idx = i;
                var ab = known[i];
                string slot = ab.spellSlotLevel > 0 ? $"\n<size=14>(L{ab.spellSlotLevel})</size>" : "";
                var btn = MakeLayoutButton(_abilityBar, $"{i + 1}. {ab.abilityName}{slot}", () => _input.Arm(idx));
                _abilityButtons.Add(btn);
            }
        }

        private void RefreshAbilityBar(GridUnit active)
        {
            if (active == null || _input == null) return;
            var armed = _input.SelectedAbility(active);
            var known = active.Sheet.knownAbilities;
            for (int i = 0; i < _abilityButtons.Count && i < known.Count; i++)
            {
                bool isArmed = known[i] == armed;
                bool affordable = AbilityRunner.CanAfford(_turns, active, known[i]);
                var img = _abilityButtons[i].GetComponent<Image>();
                img.color = isArmed ? Accent
                          : affordable ? new Color(0.2f, 0.2f, 0.25f, 0.95f)
                          : new Color(0.15f, 0.1f, 0.1f, 0.8f);
            }
        }

        // ---- log ---------------------------------------------------------

        private void AppendLog(string msg)
        {
            _logLines.Add(msg);
            if (_logLines.Count > 9) _logLines.RemoveAt(0);
            if (_log != null) _log.text = string.Join("\n", _logLines);
        }

        // ---- uGUI factory helpers ---------------------------------------

        private RectTransform MakePanel(string name, Vector2 aMin, Vector2 aMax, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(Image));
            go.transform.SetParent(_canvas.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = aMin; rt.anchorMax = aMax; rt.pivot = new Vector2(aMin.x, aMax.y);
            rt.sizeDelta = size; rt.anchoredPosition = pos;
            go.GetComponent<Image>().color = Panel;
            return rt;
        }

        private RectTransform MakeChildPanel(RectTransform parent, Vector2 aMin, Vector2 aMax, Vector2 offMin, Vector2 offMax)
        {
            var go = new GameObject("Child", typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = aMin; rt.anchorMax = aMax;
            rt.offsetMin = offMin; rt.offsetMax = offMax;
            go.GetComponent<Image>().color = Panel;
            return rt;
        }

        private Text MakeText(RectTransform parent, string content, int size, TextAnchor anchor)
        {
            var go = new GameObject("Text", typeof(Text));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(8, 8); rt.offsetMax = new Vector2(-8, -8);
            var t = go.GetComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = size; t.color = Color.white; t.alignment = anchor;
            t.supportRichText = true; t.text = content;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }

        private Button MakeButton(string name, string label, Vector2 aMin, Vector2 aMax,
            Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction onClick)
        {
            var rt = MakePanel(name, aMin, aMax, pos, size);
            rt.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 0.95f);
            var btn = rt.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(onClick);
            var t = MakeText(rt, label, 20, TextAnchor.MiddleCenter);
            t.alignment = TextAnchor.MiddleCenter;
            return btn;
        }

        private Button MakeLayoutButton(RectTransform parent, string label, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject("AbilityBtn", typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 0.95f);
            go.GetComponent<LayoutElement>().minWidth = 120;
            go.GetComponent<Button>().onClick.AddListener(onClick);
            var t = MakeText(go.GetComponent<RectTransform>(), label, 17, TextAnchor.MiddleCenter);
            t.alignment = TextAnchor.MiddleCenter;
            return go.GetComponent<Button>();
        }
    }
}
