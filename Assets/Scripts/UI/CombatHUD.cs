using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SunderedCrown.Combat;
using SunderedCrown.Grid;
using SunderedCrown.Characters;
using SunderedCrown.Items;

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
        private Button _dodgeButton;
        private Button _dashButton;
        private Button _helpButton;
        private Button _disengageButton;
        private Button _shoveButton;
        private Button _quaffButton;

        private readonly List<string> _logLines = new List<string>();
        private readonly List<(GridUnit unit, RectTransform fill, Text label)> _portraits = new();
        private readonly List<Button> _abilityButtons = new();

        private GridUnit _lastActive;
        private bool _portraitsBuilt;
        private bool _hintDismissed;

        private static readonly Color Panel = new Color(0.08f, 0.08f, 0.10f, 0.85f);
        private static readonly Color Accent = new Color(0.85f, 0.7f, 0.35f);

        void Start()
        {
            _turns = TurnManager.Instance;
            _input = FindFirstObjectByType<PlayerCombatInput>();
            BuildFrame();
            if (_turns != null) { _turns.OnCombatLog += AppendLog; _turns.OnTurnStarted += FocusCameraOn; }
        }

        void OnDestroy() { if (_turns != null) { _turns.OnCombatLog -= AppendLog; _turns.OnTurnStarted -= FocusCameraOn; } }

        // Glide the camera to center on whoever's turn it is.
        private void FocusCameraOn(GridUnit unit)
        {
            if (unit == null || !SunderedCrown.Core.GameSettings.CameraAutoFocus) return;
            var cam = Camera.main;
            var rig = cam != null ? cam.GetComponent<SunderedCrown.CameraRig.IsometricCameraController>() : null;
            if (rig != null) rig.FocusOn(unit.transform.position);
        }

        // First-ever-combat tutorial hint (IMGUI over the uGUI HUD), shown once per machine.
        void OnGUI()
        {
            if (_turns == null || !_turns.InCombat || _hintDismissed) return;
            if (PlayerPrefs.GetInt("tutorial.combat_seen", 0) == 1) return;

            var area = new Rect(Screen.width / 2f - 280, 12, 560, 70);
            GUILayout.BeginArea(area, GUI.skin.box);
            GUILayout.Label("<b>Your turn:</b> click a tile to move, click a foe to attack.  Actions: " +
                            "<b>G</b> Defend · <b>F</b> Dash · <b>T</b> Help · <b>X</b> Disengage · <b>V</b> Shove · <b>Q</b> Quaff · <b>Space</b> End turn.");
            if (GUILayout.Button("Got it", GUILayout.Width(90)))
            { _hintDismissed = true; PlayerPrefs.SetInt("tutorial.combat_seen", 1); PlayerPrefs.Save(); }
            GUILayout.EndArea();
        }

        // ---- construction ------------------------------------------------

        private void BuildFrame()
        {
            EnsureEventSystem();

            var canvasGO = new GameObject("CombatHUD_Canvas");
            canvasGO.transform.SetParent(transform, false); // tear down with this HUD's mode
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

            // Ability bar (bottom-center) — wide enough for a high-level caster's full kit.
            _abilityBar = MakePanel("AbilityBar", new Vector2(0.5f, 0), new Vector2(0.5f, 0),
                new Vector2(0, 20), new Vector2(1180, 90));
            _abilityBar.anchoredPosition = new Vector2(0, 60);
            var hl = _abilityBar.gameObject.AddComponent<HorizontalLayoutGroup>();
            hl.spacing = 8; hl.padding = new RectOffset(8, 8, 8, 8);
            hl.childControlWidth = true; hl.childForceExpandWidth = true; hl.childControlHeight = true;

            // End Turn button (bottom-right).
            _endTurnButton = MakeButton("EndTurn", "End Turn ▸", new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-180, 30), new Vector2(170, 60), () => _turns?.NextTurn());

            // The four secondary actions stack in a right-edge column ABOVE End Turn, clear of the
            // bottom-centered ability bar.
            _dodgeButton = MakeButton("Dodge", "🛡 Defend (G)", new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-180, 96), new Vector2(170, 52), () => _turns?.TryDodge());
            _dashButton = MakeButton("Dash", "💨 Dash (F)", new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-180, 152), new Vector2(170, 52), () => _turns?.TryDash());
            _helpButton = MakeButton("Help", "🤝 Help (T)", new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-180, 208), new Vector2(170, 52), () => { if (_input != null) _input.BeginHelp(); });
            _disengageButton = MakeButton("Disengage", "🥾 Disengage (X)", new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-180, 264), new Vector2(170, 52), () => _turns?.TryDisengage());
            _shoveButton = MakeButton("Shove", "🪨 Shove (V)", new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-180, 320), new Vector2(170, 52), () => { if (_input != null) _input.BeginShove(); });
            _quaffButton = MakeButton("Quaff", "🧪 Quaff (Q)", new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-180, 376), new Vector2(170, 52), () => { if (_input != null) _input.TryQuaff(); });
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

            if (!inCombat)
            {
                bool won = _turns.TurnOrder.Any(u => u != null && u.Sheet.IsAlive &&
                    (u.faction == Faction.Player || u.faction == Faction.Ally));
                _activeInfo.text = won ? "<b><color=#8d8>— Victory —</color></b>" : "<b><color=#c95>— Defeat —</color></b>";
                _dodgeButton.gameObject.SetActive(false);
                _dashButton.gameObject.SetActive(false);
                _helpButton.gameObject.SetActive(false);
                _disengageButton.gameObject.SetActive(false);
                _shoveButton.gameObject.SetActive(false);
                _quaffButton.gameObject.SetActive(false);
                return;
            }

            // The action buttons are offered only on a living player/ally turn that still has its action.
            var act = _turns.ActiveUnit;
            bool playerTurn = act != null && (act.faction == Faction.Player || act.faction == Faction.Ally);
            _dodgeButton.gameObject.SetActive(playerTurn);
            _dodgeButton.interactable = playerTurn && _turns.ActionAvailable && !(act != null && act.Sheet.IsDodging);
            _dashButton.gameObject.SetActive(playerTurn);
            _dashButton.interactable = playerTurn && _turns.ActionAvailable;
            _helpButton.gameObject.SetActive(playerTurn);
            _helpButton.interactable = playerTurn && _turns.ActionAvailable;
            _disengageButton.gameObject.SetActive(playerTurn);
            _disengageButton.interactable = playerTurn && _turns.ActionAvailable && !(act != null && act.Sheet.IsDisengaging);
            _shoveButton.gameObject.SetActive(playerTurn);
            _shoveButton.interactable = playerTurn && _turns.ActionAvailable;
            _quaffButton.gameObject.SetActive(playerTurn);
            _quaffButton.interactable = playerTurn && _turns.ActionAvailable && PotionCount() > 0;

            // Nudge: tint End Turn amber while the active hero still has an unspent action.
            var etImg = _endTurnButton.GetComponent<Image>();
            if (etImg != null)
                etImg.color = (playerTurn && _turns.ActionAvailable)
                    ? new Color(0.5f, 0.42f, 0.2f, 0.95f)
                    : new Color(0.2f, 0.2f, 0.25f, 0.95f);

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
                (s.IsDodging ? "<color=#8cf>🛡 Dodging</color> " : "") +
                (s.IsDisengaging ? "<color=#9c9>🥾 Disengaged</color> " : "") +
                (cond.Length > 0 ? $"<color=#d88>{cond}</color>" : "") +
                SlotsLine(s) +
                (PotionCount() > 0 ? $"\n<color=#9d9>🧪 ×{PotionCount()} healing</color>" : "");
        }

        /// <summary>A compact spell-slot readout (filled/empty dots per level) for the active caster, or "".</summary>
        private static string SlotsLine(CharacterSheet s)
        {
            if (s == null || s.spellSlots == null) return "";
            var sb = new System.Text.StringBuilder();
            for (int lvl = 1; lvl < 10; lvl++)
            {
                int mx = s.spellSlots.max[lvl];
                if (mx <= 0) continue;
                int cur = s.spellSlots.current[lvl];
                sb.Append($" <color=#cba>L{lvl}</color> <color=#9bd>{new string('●', cur)}</color><color=#555>{new string('○', Mathf.Max(0, mx - cur))}</color>");
            }
            return sb.Length > 0 ? "\n<size=15>Slots" + sb + "</size>" : "";
        }

        /// <summary>How many healing consumables sit in the shared party stash (for the Quaff readout).</summary>
        private static int PotionCount()
        {
            var inv = Party.Instance != null ? Party.Instance.inventory : null;
            if (inv == null) return 0;
            int n = 0;
            foreach (var st in inv.stacks)
            {
                var def = ItemDatabase.Get(st.itemId);
                if (def != null && def.kind == ItemKind.Consumable && def.useEffect != null && def.useEffect.isHeal)
                    n += st.count;
            }
            return n;
        }

        private void RefreshInitiative(GridUnit active)
        {
            int foes = _turns.TurnOrder.Count(u => u != null && u.Sheet.IsAlive && u.faction == Faction.Enemy);
            var sb = new System.Text.StringBuilder(
                $"<b>Initiative</b>   <color=#cc9>Round {_turns.RoundNumber}</color>   <color=#e99>Foes {foes}</color>\n");
            foreach (var u in _turns.TurnOrder)
            {
                string mark = u == active ? "▸ " : "   ";
                bool friendly = u.faction == Faction.Player || u.faction == Faction.Ally;
                string status = u.Sheet.IsAlive ? ""
                    : friendly ? " <color=#d95>(downed)</color>" : " <color=#a55>(slain)</color>";
                sb.AppendLine($"{mark}{u.Sheet.displayName} [{u.Sheet.currentHitPoints}]{status}");
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
                var btn = MakeLayoutButton(_abilityBar, AbilityLabel(i + 1, ab), () => _input.Arm(idx));
                _abilityButtons.Add(btn);
            }
        }

        /// <summary>A two-line ability-bar label: "N. Name" over a compact stat line (heal/damage dice,
        /// range, AoE radius, slot, bonus-action) so players can read their kit at a glance.</summary>
        private static string AbilityLabel(int number, SunderedCrown.Characters.AbilityDefinition ab)
        {
            var bits = new List<string>();
            if (ab.isHeal)
                bits.Add("heal " + (!string.IsNullOrWhiteSpace(ab.healDice) ? ab.healDice : ab.damageDice));
            else if (!string.IsNullOrWhiteSpace(ab.damageDice))
                bits.Add(ab.damageDice + " " + ab.damageType);
            bits.Add(ab.rangeTiles <= 1 ? "melee" : "rng " + ab.rangeTiles);
            if (ab.areaRadiusTiles > 0) bits.Add("AoE " + ab.areaRadiusTiles);
            if (ab.spellSlotLevel > 0) bits.Add("L" + ab.spellSlotLevel);
            if (ab.cost == SunderedCrown.Characters.ActionCost.BonusAction) bits.Add("bonus");
            return $"{number}. {ab.abilityName}\n<size=13><color=#bbb>{string.Join(" · ", bits)}</color></size>";
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
            if (_logLines.Count > 11) _logLines.RemoveAt(0);
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
            go.GetComponent<LayoutElement>().minWidth = 108;
            go.GetComponent<Button>().onClick.AddListener(onClick);
            var t = MakeText(go.GetComponent<RectTransform>(), label, 17, TextAnchor.MiddleCenter);
            t.alignment = TextAnchor.MiddleCenter;
            return go.GetComponent<Button>();
        }
    }
}
