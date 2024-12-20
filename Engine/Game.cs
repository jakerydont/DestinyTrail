using System.Dynamic;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using YamlDotNet.Serialization;
using System.Configuration;

namespace DestinyTrail.Engine
{
    public class Game : IGame
    {
        private CancellationTokenSource _cancellationTokenSource { get; set; }

        public ITravel travel {get;set;}

        public IInventory Inventory { get; set; }
        public InputHandler InputHandler { get; private set; }

        public IDisplay MainDisplay { get; set; }

        protected IDisplay StatusDisplay { get; set; }

        private IUtility _utility { get; set; }
        
        public IWorldStatus WorldStatus { get; set; }
        public IWagonParty _party { get; set; }
        public Modes GameMode { get; private set; }
        public IShoppingEngine ShoppingEngine { get; set; }
        private bool _shouldInitializeAtLandmark { get; set; }



        public Game(
            IDisplay Output, 
            IDisplay Status, 
            IUtility Utility, 
            IWagonParty Party,
            ITravel Travel,
            IWorldStatus worldStatus)   
        {
            InputHandler = new InputHandler(this);
            MainDisplay = Output;
            StatusDisplay = Status;
            _utility = Utility;
            travel = Travel;
            _party = Party;
            WorldStatus = worldStatus;

            _cancellationTokenSource = new CancellationTokenSource();


            MainDisplay.Write(_party.GetDisplayNames());

            string inventoryFilePath = Utility.GetAppSetting("InventoryFilePath");
            string inventoryCustomItemsFilePath = Utility.GetAppSetting("InventoryCustomItemsFilePath");
            Inventory = Utility.LoadYaml<Inventory>(inventoryFilePath);
            Inventory.CustomItems = Utility.LoadYaml<Inventory>(inventoryCustomItemsFilePath);

    

            GameMode = Modes.Travelling;
            ShoppingEngine = new ShoppingEngine(MainDisplay, Inventory);

            travel.ModeChanged += OnModeChanged;
        }

        private void OnModeChanged(Modes mode)
        {
            ChangeMode(mode);
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
                            travel.TravelLoop();
                            DrawStatusPanel();
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
            MainDisplay.WriteTitle(travel.NextLandmark.Name);
            MainDisplay.Write("Press enter to continue.");

        }


        public void DrawStatusPanel()
        {
            StatusDisplay.Clear();
            StatusDisplay.Write($"Date: {_utility.GetFormatted(WorldStatus.CurrentDate)}");
            StatusDisplay.Write($"Weather: {WorldStatus.Weather}");
            StatusDisplay.Write($"Health: {_party.GetDisplayHealth()}");
            StatusDisplay.Write($"Distance to next landmark: {_utility.Abbreviate(travel.MilesToNextLandmark)} miles ({_utility.Abbreviate(travel.MilesToNextLandmark)} km)");
            StatusDisplay.Write($"Distance traveled: {_utility.Abbreviate(travel.MilesTraveled)} miles ({_utility.Abbreviate(travel.MilesTraveled)} km)");

            StatusDisplay.Write($"-----------");
            foreach (var person in _party.Members)
            {
                StatusDisplay.Write($"{person.Name} ..... {person.Status}");
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

    public class GameStatus
    {
        public DateTime CurrentDate { get; set; }
        public string Weather { get; set; }
    }
}