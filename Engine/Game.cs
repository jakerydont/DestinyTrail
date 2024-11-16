
using System.Dynamic;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;

namespace DestinyTrail.Engine
{
    public class Game : IGame
    {

        private CancellationTokenSource _cancellationTokenSource { get; set; }

        public DateTime CurrentDate { get; set; }
        public Travel _travel;


        public LandmarksData _landmarksData { get; set; }
        public Landmark NextLandmark { get; set; }

        private string _weather = "not implemented";

        public Inventory Inventory { get; set; }


        public IDisplay _display { get; set; }

        protected IDisplay _status { get; set; }

        private IUtility _utility { get; set; }

        public double MilesTraveled { get; set; }
        public double MilesToNextLandmark { get; set; }
        public string[] RandomNames { get; private set; }

        public WagonParty Party { get; set; }


        public Modes GameMode { get; private set; }
        public ShoppingEngine ShoppingEngine { get; private set; }
        private bool _shouldInitializeAtLandmark { get; set; }


        public Game()
            : this(new Display(), new Display(), new Utility()) { }
        public Game(IDisplay Output, IDisplay Status)
            : this(Output, Status, new Utility()) { }

        public Game(IDisplay Output, IDisplay Status, IUtility Utility)
        {
            _display = Output;
            _status = Status;
            _utility = Utility;
            _cancellationTokenSource = new CancellationTokenSource();

            string randomNamesPath = "data/RandomNames.yaml";
            string landmarksFilePath = "data/Landmarks.yaml";
            string inventoryFilePath = "data/Inventory.yaml";

            RandomNames = [.. Utility.LoadYaml<RandomNamesData>(randomNamesPath)];

            Random.Shared.Shuffle(RandomNames);
            var partyNames = RandomNames.Take(26).ToArray();

            Party = new WagonParty(RandomNames);
            _display.Write(Party.GetDisplayNames());

            _landmarksData = Utility.LoadYaml<LandmarksData>(landmarksFilePath);
            NextLandmark = _landmarksData.First();

            MilesTraveled = 0;
            MilesToNextLandmark = (double)NextLandmark.Distance;

            Inventory = Utility.LoadYaml<Inventory>(inventoryFilePath);

            CurrentDate = new DateTime(1860, 10, 1);

            _travel = new Travel(this);

            GameMode = Modes.Travelling;
            ShoppingEngine = new ShoppingEngine(_display, Inventory);

        }
        public async Task StartGameLoop()
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    switch (GameMode)
                    {
                        case Modes.Travelling:
                            _travel.TravelLoop();
                            break;
                        case Modes.AtLandmark:
                            AtLandmarkLoop();
                            break;
                        case Modes.Shopping:
                            ShoppingEngine.ShoppingLoop();
                            break;
                        default:
                            break;
                    }
                    await Task.Delay(1000, token);
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, handle if needed
            }
        }

        private void AtLandmarkLoop()
        {
            if (!_shouldInitializeAtLandmark) return;
            _shouldInitializeAtLandmark = false;
            _display.Write($"{NextLandmark.Name}");
            _display.Write("Press enter to continue. Type \"buy\" to buy something.");

        }


        public void DrawStatusPanel()
        {
            _status.Clear();
            _status.Write($"Date: {_utility.GetFormatted(CurrentDate)}");
            _status.Write($"Weather: {_weather}");
            _status.Write($"Health: {Party.GetDisplayHealth()}");
            _status.Write($"Distance to next landmark: {_utility.Abbreviate(MilesToNextLandmark)} miles ({_utility.Abbreviate(MilesToNextLandmark)} km)");
            _status.Write($"Distance traveled: {_utility.Abbreviate(MilesTraveled)} miles ({_utility.Abbreviate(MilesTraveled)} km)");

            _status.Write($"-----------");
            foreach (var person in Party.Members)
            {
                _status.Write($"{person.Name} ..... {person.Status}");
            }
        }

        public void ChangeMode(Modes mode)
        {
            GameMode = mode;
            if (GameMode == Modes.AtLandmark)
            {
                _shouldInitializeAtLandmark = true;
            }
            else if (GameMode == Modes.Shopping)
            {
                ShoppingEngine.InitializeState();
            }
        }

    }
    public class ShoppingEngine
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