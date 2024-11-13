using System.Collections.Generic;
using System.IO;
using DestinyTrail.Engine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DestinyTrail.Engine
{
    public class Inventory
    {
        public InventoryItem Oxen { get; private set; }
        public InventoryItem Food { get; private set; }
        public InventoryItem Bullets { get; private set; }
        public InventoryItem Clothes { get; private set; }
        public InventoryItem Money { get; private set; }
        public InventoryItem WagonTongues { get; private set; }
        public InventoryItem WagonAxles { get; private set; }
        public InventoryItem WagonWheels { get; private set; }
        public List<InventoryItem> CustomItems { get; private set; }

        public Inventory()
        {
            Oxen = new InventoryItem { Name = "Oxen", Quantity = 0 };
            Food = new InventoryItem { Name = "Food", Quantity = 0 };
            Bullets = new InventoryItem { Name = "Bullets", Quantity = 0 };
            Clothes = new InventoryItem { Name = "Clothes", Quantity = 0 };
            Money = new InventoryItem { Name = "Money", Quantity = 0 };
            WagonTongues = new InventoryItem { Name = "Wagon Tongues", Quantity = 0 };
            WagonAxles = new InventoryItem { Name = "Wagon Axles", Quantity = 0 };
            WagonWheels = new InventoryItem { Name = "Wagon Wheels", Quantity = 0 };
            CustomItems = new List<InventoryItem>();
        }
    }
}


