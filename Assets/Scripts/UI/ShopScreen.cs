using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Core;
using SunderedCrown.Items;

namespace SunderedCrown.UI
{
    /// <summary>
    /// The Lower City fence's shop (OnGUI overlay). Lists `ShopContent.Stock` filtered by your
    /// `reputation.lowcity`, buys against the shared party gold (`Inventory`). Drop it on the hub mode and
    /// set onClose; it draws over the hub. Art-free, zero setup.
    /// </summary>
    public class ShopScreen : MonoBehaviour
    {
        public System.Action onClose;
        // Optional vendor overrides (default to the Lower City fence). Set these to reuse the screen for
        // other merchants (e.g. the Docks smuggler).
        public System.Collections.Generic.List<ShopContent.Offer> stock;
        public string vendorName = "Sczerla's Sundries";
        public string vendorTagline = "a Lower City fence";
        public string vendorQuote = "\"Coin's coin, friend. Though I keep my better shelf for folk the quarter speaks well of.\"";

        private string _msg;
        private Vector2 _scroll;

        void OnGUI()
        {
            var offers = stock ?? ShopContent.Stock;
            const float w = 500, h = 460;
            float x = Screen.width / 2f - w / 2f, y = Screen.height / 2f - h / 2f;
            GUILayout.BeginArea(new Rect(x, y, w, h), GUI.skin.box);

            var inv = Party.Instance != null ? Party.Instance.inventory : null;
            int gold = inv != null ? inv.gold : 0;
            int rep = GameFlags.Current.GetInt("reputation.lowcity");

            GUILayout.Label($"<size=20><b>🪙 {vendorName}</b></size>   <size=11><color=#888>{vendorTagline}</color></size>");
            GUILayout.Label($"<color=#c9a0ff><i>{vendorQuote}</i></color>");
            GUILayout.Label($"Your gold: <b><color=#ffd866>{gold}</color></b>     <size=11><color=#9fd>(Lower City standing {rep})</color></size>");
            GUILayout.Space(8);
            _scroll = GUILayout.BeginScrollView(_scroll);

            string sellId = null; int sellPrice = 0;

            GUILayout.Label("<b>Buy</b>");
            bool anything = false;
            foreach (var o in offers)
            {
                if (rep < o.repRequired) continue;
                anything = true;
                var def = ItemDatabase.Get(o.itemId);
                string name = def != null ? def.displayName : o.itemId;

                string stat = def == null ? "" :
                    def.kind == ItemKind.Weapon ? $"  <color=#d9b08c>{def.weaponDamage} {def.weaponDamageType}</color>" :
                    def.kind == ItemKind.Armor  ? $"  <color=#9cd1e8>+{def.armorClassBonus} AC</color>" : "";
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label($"<b>{name}</b>   <color=#ffd866>{o.price}g</color>{stat}", GUILayout.Width(330));
                GUI.enabled = inv != null && gold >= o.price && def != null;
                if (GUILayout.Button("Buy", GUILayout.Width(110), GUILayout.Height(24)))
                {
                    inv.AddGold(-o.price);
                    inv.Add(def, 1);
                    GameFlags.Current.SetBool("shop.bought", true);
                    _msg = $"Bought {name} for {o.price}g.";
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            if (!anything) GUILayout.Label("<color=#888><i>The shelves are bare today.</i></color>");

            // Tease the rep-locked goods.
            foreach (var o in offers)
            {
                if (rep >= o.repRequired) continue;
                var def = ItemDatabase.Get(o.itemId);
                GUILayout.Label($"<color=#666>🔒 {(def != null ? def.displayName : o.itemId)} — needs Lower City standing {o.repRequired}</color>");
            }

            // --- Sell: the fence buys your loot at a fraction of worth (better as your standing rises). ---
            GUILayout.Space(10);
            GUILayout.Label("<b>Sell</b>   <size=11><color=#888>(the fence pays a fraction of an item's worth)</color></size>");
            float rate = Mathf.Clamp(0.40f + rep * 0.02f, 0.30f, 0.60f);
            bool anyToSell = false;
            if (inv != null)
            {
                foreach (var st in inv.stacks)
                {
                    var def = ItemDatabase.Get(st.itemId);
                    if (def == null) continue;
                    anyToSell = true;
                    int price = Mathf.Max(1, Mathf.RoundToInt(def.valueGold * rate));
                    GUILayout.BeginHorizontal(GUI.skin.box);
                    GUILayout.Label($"<b>{def.displayName}</b> ×{st.count}   <color=#ffd866>{price}g</color>", GUILayout.Width(330));
                    if (GUILayout.Button("Sell", GUILayout.Width(110), GUILayout.Height(24))) { sellId = st.itemId; sellPrice = price; }
                    GUILayout.EndHorizontal();
                }
            }
            if (!anyToSell) GUILayout.Label("<color=#888><i>You've nothing the fence wants.</i></color>");

            GUILayout.EndScrollView();

            // Apply a sell after the layout loop, so we don't mutate the list mid-iteration.
            if (sellId != null && inv != null && inv.Remove(sellId, 1))
            {
                inv.AddGold(sellPrice);
                GameFlags.Current.SetBool("shop.sold", true);
                var sd = ItemDatabase.Get(sellId);
                _msg = $"Sold {(sd != null ? sd.displayName : sellId)} for {sellPrice}g.";
            }

            if (!string.IsNullOrEmpty(_msg)) GUILayout.Label($"<color=#8f8>{_msg}</color>");
            if (GUILayout.Button("Leave", GUILayout.Height(30))) onClose?.Invoke();
            GUILayout.EndArea();
        }
    }
}
