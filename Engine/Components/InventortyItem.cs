using System.ComponentModel.DataAnnotations;

namespace DestinyTrail.Engine
{
    public class InventoryItem : GameComponent, IInventoryItem
    {

        public static implicit operator int(InventoryItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return item.Quantity;
        }
        
        public string SingularOrPluralName(int Quantity) {
            return (Quantity == 1) ? NameSingular : NamePlural;
        }

        public override required string Name { get=> NamePlural; set => NamePlural=value; }
        public string NameSingular { get; set; } = "";
        public string NamePlural { get; set; } = "";

        private int _quantity;

        [Range(0, int.MaxValue, ErrorMessage = "No negative quantities allowed.")]
        public int Quantity
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

        private string _unit = "";
        public string Unit
        {
            get
            {
                if (_unit == "") return "";
                else return _unit + " of ";
            }
            set => _unit = value;
        }

        private string _unitSingular = "";
        public string UnitSingular
        {
            get
            {
                if (_unitSingular == "") return "";
                else return _unit + " of ";
            }
            set => _unitSingular = value;
        }

        private string Lore { get; set; } = "";


        public virtual bool Add(int amount)
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
        public virtual bool Subtract(int amount)
        {
            if (Quantity < amount) { return false; }
            Quantity -= amount;
            return true;
        }

        public void SetQuantity(int v)
        {
            Quantity = v;
        }

        public void SetBoolean(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
