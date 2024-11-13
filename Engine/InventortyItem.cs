using System.ComponentModel.DataAnnotations;

namespace DestinyTrail.Engine
{
    public class InventoryItem : GameComponent, IInventoryItem
    {   
        private int _quantity;

        [Range(0, int.MaxValue, ErrorMessage = "No negative quantities allowed.")]
        public required int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 0)
                    _quantity = 0;
                else
                    _quantity = value;
            }
        }

        private string Lore { get; set; } = "";


        public bool Add(int amount)
        {
            _quantity += amount;
            return true;
        }

        public string GetLore() => Lore;

        /// <summary>
        /// Try removing a set amount of the item. Only completes if enough of the item exists to remove.
        /// </summary>
        /// <param name="amount">The amount to remove.</param>
        /// <returns>Returns true if there was enough of the item to remove. Otherwise returns false.</returns>
        public bool Subtract(int amount)
        {
            if (Quantity < amount) { return false; }
            Quantity -= amount;
            return true;
        }
    }
}
