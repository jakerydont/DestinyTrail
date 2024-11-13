using System.Collections.Generic;
using System.IO;
using DestinyTrail.Engine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DestinyTrail.Engine
{
    public class Inventory : GameData<InventoryItem>
    {
        public List<InventoryItem> InventoryItems { get => _items; set => _items = value; }
        public InventoryItem Oxen => GetByName("Oxen");
        public InventoryItem Food => GetByName("Food");
        public InventoryItem Bullets => GetByName("Bullets");
        public InventoryItem Clothes => GetByName("Clothes");
        public InventoryItem Dollars => GetByName("Money");
        public InventoryItem WagonTongues => GetByName("Wagon Tongues");
        public InventoryItem WagonAxles => GetByName("Wagon Axles");
        public InventoryItem WagonWheels => GetByName("Wagon Wheels");
        public List<InventoryItem> CustomItems { get; set; }

    }
}


