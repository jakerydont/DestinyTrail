namespace DestinyTrail.Engine
{
    public interface IShoppingEngine
    {
        ShoppingState ShoppingState { get; set; }
        InventoryItem SelectedItem { get; set; }
        int Quantity { get; }
        void InitializeState();
        void ShoppingLoop();
        void ProcessInput(string input);
    }
}
