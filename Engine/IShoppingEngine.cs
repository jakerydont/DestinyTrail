namespace DestinyTrail.Engine
{
    public interface IShoppingEngine
    {
        ShoppingState ShoppingState { get; set; }
        InventoryItem Selection { get; set; }
        int Quantity { get; }
        void InitializeState();
        void ShoppingLoop();
        void SelectShoppingItem(string input);
        void SelectQuantity(string input);
    }
}
