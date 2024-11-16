
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
}