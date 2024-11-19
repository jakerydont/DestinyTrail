namespace DestinyTrail.Engine
{
    public interface IInventory
    {
        /// <summary>
        /// Empty inventory item, used when no specific item is selected.
        /// </summary>
        InventoryItem Default { get; }

        /// <summary>
        /// List of inventory items.
        /// </summary>
        List<InventoryItem> InventoryItems { get; set; }

        /// <summary>
        /// Specific inventory items for various goods.
        /// </summary>
        InventoryItem Oxen { get; }
        InventoryItem Food { get; }
        InventoryItem Bullets { get; }
        InventoryItem Clothes { get; }
        InventoryItem Dollars { get; }
        InventoryItem WagonTongues { get; }
        InventoryItem WagonAxles { get; }
        InventoryItem WagonWheels { get; }

        /// <summary>
        /// List of custom inventory items defined by the user.
        /// </summary>
        List<InventoryItem> CustomItems { get; set; }

        /// <summary>
        /// Retrieves an inventory item by its name.
        /// </summary>
        /// <param name="name">The name of the inventory item.</param>
        /// <returns>The inventory item if found, otherwise Default.</returns>
        InventoryItem GetByName(string name);


        /// <summary>
        /// Lists all inventory items by name.
        /// </summary>
        /// <returns>Comma-separated list of inventory items.</returns>
        string ListInventoryItems();
    }
}
