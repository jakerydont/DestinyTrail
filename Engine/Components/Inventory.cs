namespace DestinyTrail.Engine
{
    public class Inventory : GameData<InventoryItem>, IInventory
    {
        public static InventoryItem Default = new InventoryItem{ Name = "none" };
        InventoryItem IInventory.Default => Default;
        public List<InventoryItem> InventoryItems { get => _items; set => _items = value; }
        public InventoryItem Oxen => GetByName("Oxen");
        public InventoryItem Food => GetByName("Food");
        public InventoryItem Bullets => GetByName("Bullets");
        public InventoryItem Clothes => GetByName("Clothes");
        public InventoryItem Dollars => GetByName("Dollars");
        public InventoryItem WagonTongues => GetByName("Wagon Tongues");
        public InventoryItem WagonAxles => GetByName("Wagon Axles");
        public InventoryItem WagonWheels => GetByName("Wagon Wheels");
        
        private List<InventoryItem> _customItems = new List<InventoryItem>();
        public List<InventoryItem> CustomItems { get => _customItems; set => _customItems = value; }

        public string ListInventoryItems()
        {
            var itemNames = new List<string>(InventoryItems.Select(item=>item.Name));
            itemNames.AddRange(CustomItems.Select(item => item.Name));
            return string.Join(", ", itemNames);
        }

    }
}


