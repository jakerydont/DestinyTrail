
namespace DestinyTrail.Engine;

public interface IInventory : IGameData<IInventoryItem>
{
    /// <summary>
    /// Empty inventory item, used when no specific item is selected.
    /// </summary>
    IInventoryItem Default { get; }

    /// <summary>
    /// List of inventory items.
    /// </summary>
    List<InventoryItem> InventoryItems { get; set; }

    /// <summary>
    /// Specific inventory items for various goods.
    /// </summary>
    IInventoryItem Oxen { get; }
    IInventoryItem Food { get; }
    IInventoryItem Bullets { get; }
    IInventoryItem Clothes { get; }
    IInventoryItem Dollars { get; }
    IInventoryItem WagonTongues { get; }
    IInventoryItem WagonAxles { get; }
    IInventoryItem WagonWheels { get; }

    /// <summary>
    /// List of custom inventory items defined by the user.
    /// </summary>
    List<InventoryItem> CustomItems { get; set; }

    bool TryGetByName(string name, out IInventoryItem item);

    /// <summary>
    /// Retrieves an inventory item by its name.
    /// </summary>
    /// <param name="name">The name of the inventory item.</param>
    /// <returns>The inventory item if found, otherwise Default.</returns>
    IInventoryItem GetByName(string name);


    /// <summary>
    /// Lists all inventory items by name.
    /// </summary>
    /// <returns>Comma-separated list of inventory items.</returns>
    string ListInventoryItems();
}
