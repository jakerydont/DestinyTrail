using System;

namespace DestinyTrail.Engine

{    public class ShoppingEngine
    {
        public ShoppingState ShoppingState { get; set; }
        public InventoryItem Selection { get; set; }
        public int Quantity { get; private set; }
        private IDisplay _display { get; }
        private Inventory Inventory { get; }

        public ShoppingEngine(IDisplay display, Inventory inventory)
        {
            _display = display;
            Inventory = inventory;
            ShoppingState = ShoppingState.Init;
            Selection = Inventory.Default;
            Quantity = 0;
        }

        public void InitializeState()
        {
            ShoppingState = ShoppingState.Init;
            Selection = Inventory.Default;
            Quantity = 0;
        }


        public void ShoppingLoop()
        {
            switch (ShoppingState)
            {
                case ShoppingState.Init:
                    _display.Write("-----\n\nWelcome to the store. Type what you want to buy. Type \"exit\" to quit.");
                    _display.Write("Oxen, Food, Baja Blast, etc");
                    ShoppingState = ShoppingState.AskSelection;
                    break;
                case ShoppingState.AskSelection:
                    _display.Write("What'll it be?");
                    break;
                case ShoppingState.AwaitSelection:
                    break;
                case ShoppingState.AskQuantity:
                    _display.Write($"{Selection}? Yeah I got some.");
                    _display.Write($"How many {Selection.Unit}{Selection} do you want?");
                    ShoppingState = ShoppingState.AwaitQuantity;
                    break;
                case ShoppingState.AwaitQuantity:
                    break;
                case ShoppingState.ConfirmPurchase:
                    _display.Write($"{Quantity} {Selection.Unit}{Selection.SingularOrPluralName(Quantity)}? That'll be {CalculatePrice()}. Deal?");
                    break;
                case ShoppingState.AwaitConfirm:
                    break;
            }
        }

        private string CalculatePrice()
        {
            return "free because I haven't coded this yet.";
        }

        public void SelectShoppingItem(string input)
        {
            try
            {
                var selectedItem = Inventory.GetByName(input);
                if (selectedItem != null)
                {
                    Selection = selectedItem;
                    ShoppingState = ShoppingState.AskQuantity;
                }
            }
            catch (NullReferenceException)
            {
                _display.Write($"Hey, you old poophead, I ain't got no ${input} for sale. Try again.");
            }
        }

        public void SelectQuantity(string input)
        {
            try
            {
                int quantity = 0;
                var isNumber = Int32.TryParse(input, out quantity);

                if (isNumber)
                {
                    if (quantity > 0)
                    {
                        Quantity = quantity;
                        ShoppingState = ShoppingState.AwaitConfirm;
                    }
                    else
                    {
                        _display.Write($"Change yer mind?");
                        ShoppingState = ShoppingState.AskSelection;
                    }
                }
                else
                {
                    _display.Write($"That ain't no number I've ever heard of.");
                }

            }
            catch (NullReferenceException)
            {
                _display.Write($"Hey, you old poophead, I ain't got no ${input} for sale. Try again.");
            }
        }


    }

}
