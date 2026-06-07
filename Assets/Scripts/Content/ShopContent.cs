using System.Collections.Generic;

namespace SunderedCrown.Content
{
    /// <summary>
    /// The Lower City fence's stock. A flat list of (itemId, price, reputation-required) — pricier/better
    /// goods unlock as your `reputation.lowcity` rises, so doing right by the quarter literally widens what
    /// the underworld will sell you. Pure data; ShopScreen renders it against the party gold.
    /// </summary>
    public static class ShopContent
    {
        public class Offer
        {
            public string itemId;
            public int price;
            public int repRequired; // reputation.lowcity needed for this to appear
        }

        public static readonly List<Offer> Stock = new List<Offer>
        {
            new Offer { itemId = "healing_potion", price = 25, repRequired = 0 },
            new Offer { itemId = "leather_armor",  price = 40, repRequired = 0 },
            new Offer { itemId = "longsword",      price = 55, repRequired = 0 },
            new Offer { itemId = "wooden_shield",  price = 20, repRequired = 0 },
            new Offer { itemId = "warhammer",      price = 30, repRequired = 1 },
            new Offer { itemId = "iron_helm",      price = 35, repRequired = 2 },
            new Offer { itemId = "chain_shirt",    price = 95, repRequired = 2 },
            new Offer { itemId = "greataxe",       price = 80, repRequired = 4 },
            new Offer { itemId = "half_plate",     price = 160, repRequired = 5 },
            new Offer { itemId = "ring_protection", price = 240, repRequired = 6 },
        };

        /// <summary>The Chionthar Docks smuggler's stock — cheaper smuggled goods, no reputation needed (the
        /// river doesn't care who you are, only your coin). A second, distinct vendor.</summary>
        public static readonly List<Offer> SmugglerStock = new List<Offer>
        {
            new Offer { itemId = "healing_potion",  price = 18, repRequired = 0 }, // smuggled, cheaper
            new Offer { itemId = "smuggled_dagger", price = 6,  repRequired = 0 },
            new Offer { itemId = "leather_armor",   price = 30, repRequired = 0 },
            new Offer { itemId = "rapier",          price = 22, repRequired = 0 }, // a "fell off a ship" blade
            new Offer { itemId = "chain_shirt",     price = 75, repRequired = 0 }, // no rep gate, just pricey
            new Offer { itemId = "greatsword",      price = 48, repRequired = 0 },
        };
    }
}
