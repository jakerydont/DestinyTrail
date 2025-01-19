namespace DestinyTrail.Engine
{
    public class Inventory : GameData<InventoryItem>, IInventory
    {
        public static new IInventoryItem Default = new InventoryItem{ Name = "none" };
        IInventoryItem IInventory.Default => Default;
        public List<InventoryItem> InventoryItems { get => _items; set => _items = value; }

        IInventoryItem IGameData<IInventoryItem>.this[int index] {
            get => _items[index];
            set => _items[index] = (InventoryItem)value;
        }

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


        public Inventory() : base()
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

        public bool TryGetByName(string name, out IInventoryItem item)
        {
            item = GetByName(name);
            return item != null;
        }



        void IGameData<IInventoryItem>.Remove(IInventoryItem item) => Remove((InventoryItem)item);

        public void Add(IInventoryItem item) => Add((InventoryItem)item);

        IInventoryItem[] IGameData<IInventoryItem>.ToArray() => _items.ToArray();

        IInventoryItem IGameData<IInventoryItem>.First() => _items[0];

        public IInventoryItem? MinBy<TKey>(Func<IInventoryItem, TKey> keySelector) => _items.MinBy(keySelector);
    }
}


