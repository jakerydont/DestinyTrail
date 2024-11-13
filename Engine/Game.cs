
using System.Dynamic;
using System.Formats.Asn1;

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
        public ShoppingState ShoppingState { get; set; }
        public InventoryItem ShoppingSelection { get; set; }

        public IDisplay _display { get; set; }

        protected IDisplay _status { get; set; }

        public double MilesTraveled { get; set; }
        public double MilesToNextLandmark { get; set; }
        public string[] RandomNames { get; private set; }

        public WagonParty Party { get; set; }


        public Modes GameMode { get; private set; }

        private bool _shouldInitializeAtLandmark { get; set; }

        public Game()
            : this(new Display(), new Display()) { }

        public Game(IDisplay Output, IDisplay Status)
        {
            _display = Output;
            _status = Status;
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
            ShoppingState = ShoppingState.Init;
            ShoppingSelection = Inventory.Default;
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
                            ShoppingLoop();
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

        private void ShoppingLoop()
        {
            if (ShoppingState == ShoppingState.Init)
            {
                _display.Write("-----\n\nWelcome to the store. Type what you want to buy. Type \"exit\" to quit.");
                _display.Write("Oxen, Food, Baja Blast, etc");
                ShoppingState = ShoppingState.WaitSelection;
            }
            else if (ShoppingState == ShoppingState.WaitSelection)
            {
                // noop
            }
            else if (ShoppingState == ShoppingState.HowMany)
            {
                _display.Write($"How many ${ShoppingSelection.Unit}${ShoppingSelection.NameSingular} do you want?");
                ShoppingState = ShoppingState.WaitQuantity;
            }
            else if (ShoppingState == ShoppingState.WaitQuantity)
            {
                // noop
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
            _status.Write($"Date: {CurrentDate.GetFormatted()}");
            _status.Write($"Weather: {_weather}");
            _status.Write($"Health: {Party.GetDisplayHealth()}");
            _status.Write($"Distance to next landmark: {MilesToNextLandmark.Abbreviate()} miles ({MilesToNextLandmark.Abbreviate()} km)");
            _status.Write($"Distance traveled: {MilesTraveled.Abbreviate()} miles ({MilesTraveled.Abbreviate()} km)");

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
                ShoppingState = ShoppingState.Init;
            }
        }

        public void SelectShoppingItem(string input)
        {
            try
            { 
                var selectedItem = Inventory.GetByName(input); 
            }
            catch (NullReferenceException) {
                _display.Write($"Hey, you old poophead, I ain't got no ${input} for sale. Try again.");
            }
        }
    }
}