namespace DestinyTrail.Engine
{
    public interface IInventoryItem
    {
        public int Quantity { get; set; }
        public string GetLore();
        public bool Add(int Quantity);
        public bool Subtract(int Quantity);

    }
}
