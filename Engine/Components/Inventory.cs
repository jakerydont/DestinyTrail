namespace DestinyTrail.Engine
{
    public class Inventory : GameData<InventoryItem>, IInventory
    {
        public static IInventoryItem Default = new InventoryItem{ Name = "none" };
        IInventoryItem IInventory.Default => Default;
        public List<InventoryItem> InventoryItems { get => _items; set => _items = value; }
        public IInventoryItem Oxen => GetByName("Oxen");
        public IInventoryItem Food => GetByName("Food");
        public IInventoryItem Bullets => GetByName("Bullets");
        public IInventoryItem Clothes => GetByName("Clothes");
        public IInventoryItem Dollars => GetByName("Dollars");
        public IInventoryItem WagonTongues => GetByName("Wagon Tongues");
        public IInventoryItem WagonAxles => GetByName("Wagon Axles");
        public IInventoryItem WagonWheels => GetByName("Wagon Wheels");
        
        private List<InventoryItem> _customItems = new List<InventoryItem>();
        public List<InventoryItem> CustomItems { get => _customItems; set => _customItems = value; }

        public Inventory()
        {
            _items = new List<InventoryItem>
            {
                new InventoryItem { Name = "Oxen" },
                new InventoryItem { Name = "Food" },
                new InventoryItem { Name = "Bullets" },
                new InventoryItem { Name = "Clothes" },
                new InventoryItem { Name = "Dollars" },
                new InventoryItem { Name = "Wagon Tongues" },
                new InventoryItem { Name = "Wagon Axles" },
                new InventoryItem { Name = "Wagon Wheels" }
            };
        }
        public string ListInventoryItems()
        {
            var itemNames = new List<string>(InventoryItems.Select(item=>item.Name));
            itemNames.AddRange(CustomItems.Select(item => item.Name));
            return string.Join(", ", itemNames);
        }

        IInventoryItem IInventory.GetByName(string name)
        {
            return GetByName(name);
        }



        public InventoryItem First()
        {
            throw new NotImplementedException();
        }

        void IGameData<InventoryItem>.Remove(InventoryItem item)
        {
            Remove(item);
        }
    }
}


