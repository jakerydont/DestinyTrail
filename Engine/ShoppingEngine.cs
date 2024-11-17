namespace DestinyTrail.Engine
{
    public class ShoppingEngine : IShoppingEngine
    {
        public ShoppingState ShoppingState { get; set; }
        public InventoryItem SelectedItem { get; set; }
        public int Quantity { get; private set; }
        private IDisplay _display { get; }
        private Inventory Inventory { get; }
        private int _price;

        private bool BoughtAnything { get; set; }
        public ShoppingEngine(IDisplay display, Inventory inventory)
        {
            _display = display;
            Inventory = inventory;
            InitializeState();
        }

        public void InitializeState()
        {
            ShoppingState = ShoppingState.Init;
            SelectedItem = Inventory.Default;
            Quantity = 0;
            BoughtAnything = false;
        }

        public void ShoppingLoop()
        {
            switch (ShoppingState)
            {
                case ShoppingState.Init:
                    _display.Write("-----\n\nWelcome to the store.");
                    ShoppingState = ShoppingState.AskSelection;
                    break;
                case ShoppingState.AskSelection:
                    _display.Write("We have Oxen, Food, Baja Blast, etc");
                    _display.Write("What'll it be?");
                    ShoppingState = ShoppingState.AwaitSelection;
                    break;
                case ShoppingState.AwaitSelection:
                    break;
                case ShoppingState.AskQuantity:
                    _display.Write($"{SelectedItem}? Yeah I got some.");
                    _display.Write($"How many {SelectedItem.Unit}{SelectedItem} do you want?");
                    ShoppingState = ShoppingState.AwaitQuantity;
                    break;
                case ShoppingState.AwaitQuantity:
                    break;
                case ShoppingState.ConfirmPurchase:
                    _display.Write($"{Quantity} {SelectedItem.Unit}{SelectedItem.SingularOrPluralName(Quantity)}? That'll be {CalculatePrice()}. Deal?");
                    ShoppingState = ShoppingState.AwaitConfirm;
                    break;
                case ShoppingState.AwaitConfirm:
                    break;
                case ShoppingState.Complete:
                    _display.Write("Deal. Pleasure doin' business with ya.");
                    _display.Write("We have plenty else in stock.");
                    ShoppingState = ShoppingState.AskSelection;
                    break;
                case ShoppingState.Leave:
                    if (BoughtAnything) 
                    {
                        _display.Write("Thanks for coming in. Good luck out there!");
                    }
                    else 
                    {
                        _display.Write("Wasting my time with your browsing. Hmph!");
                    }
                    _display.Write("Exiting store...");
                    break;
            }
        }

        internal void ProcessInput(string input)
        {
            if (input.ToLower().StartsWith("help"))
            {
                _display.Write("Type what you want to buy, then type how many you want to buy. Then say \"yes\" or \"no\" to confirm. To leave the store at any time, type \"exit\" or \"quit\" or \"leave\".");
            }
            if (input.ToLower().StartsWith("exit") || input.ToLower().StartsWith("quit") || input.ToLower().StartsWith("leave"))
            {
                ShoppingState = ShoppingState.Leave;
            }
            switch (ShoppingState)
            {
                case ShoppingState.AwaitSelection:
                    SelectShoppingItem(input);
                    break;
                case ShoppingState.AwaitQuantity:
                    SelectQuantity(input);
                    break;
                case ShoppingState.AwaitConfirm:
                    GetConfirmation(input);
                    break;
            }


        }


        /// <summary>
        /// TODO: Put in a price table for goods
        /// </summary>
        /// <returns></returns>
        private string CalculatePrice()
        {
            _price = 0;
            return "free because I haven't coded this yet.";
        }

        public void SelectShoppingItem(string input)
        {
            try
            {
                var selectedItem = Inventory.GetByName(input);
                if (selectedItem != null)
                {
                    SelectedItem = selectedItem;
                    ShoppingState = ShoppingState.AskQuantity;
                }
            }
            catch (NullReferenceException)
            {
                _display.Write($"Hey, you old poophead, I ain't got no \"${input}\" for sale. Try again.");
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
                        ShoppingState = ShoppingState.ConfirmPurchase;
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


        private void GetConfirmation(string input)
        {
            try
            {
                if (input == string.Empty)
                {
                    _display.Write("A deal ain't a deal unless you answer.");
                }
                else if (input.ToLower().StartsWith("y"))
                {
                    var wasAbleToPay = Inventory.Dollars.Subtract(_price);
                    if (wasAbleToPay)
                    {
                        Inventory.GetByName(SelectedItem).Add(Quantity);
                        BoughtAnything = true;
                        ShoppingState = ShoppingState.Complete;
                    }
                    else
                    {
                        _display.Write("You ain't got the money and this ain't no commune.");
                        if (Inventory.Dollars == 0)
                        {
                            _display.Write("In fact, you're flat broke. Get outta here, you hobo.");
                            ShoppingState = ShoppingState.Leave;
                        }
                        else
                        {
                            ShoppingState = ShoppingState.AskSelection;
                        }

                    }

                }
                else if (input.ToLower().StartsWith("n"))
                {
                    _display.Write("Have it your way.");
                }
            }
            catch (NullReferenceException)
            {
                _display.Write($"Hey, you old poophead, {input} ain't no kind of answer. A yes or no will do.");
            }
        }
    }

}
