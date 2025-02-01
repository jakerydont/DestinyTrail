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
        public IInputHandler _inputHandler { get; set; }

        public IDisplay MainDisplay { get; set; }

        protected IDisplay StatusDisplay { get; set; }

        private IUtility _utility { get; set; }
        
        public IWorldStatus WorldStatus { get; set; }
        public IWagonParty _party { get; set; }
        public Modes GameMode { get; private set; }
        public IShoppingEngine ShoppingEngine { get; set; }
        private bool _shouldInitializeAtLandmark { get; set; }

        public static async Task<Game> CreateAsync(IDisplay Output, 
            IDisplay Status, 
            IUtility Utility, 
            IWagonParty Party,
            ITravel Travel,
            IWorldStatus worldStatus,
            IInputHandler InputHandler) 
        {
            var game = new Game(Output,    Status,   Utility,   Party,  Travel, worldStatus, InputHandler);
            string inventoryFilePath = Utility.GetAppSetting("InventoryFilePath");
            string inventoryCustomItemsFilePath = Utility.GetAppSetting("InventoryCustomItemsFilePath");
            game.Inventory = await Utility.LoadYamlAsync<Inventory>(inventoryFilePath);
            game.Inventory.CustomItems = await Utility.LoadYamlAsync<Inventory>(inventoryCustomItemsFilePath);
            game.ShoppingEngine = new ShoppingEngine(game.MainDisplay, game.Inventory);
            return game;
        }

        private Game(
            IDisplay Output, 
            IDisplay Status, 
            IUtility Utility, 
            IWagonParty Party,
            ITravel Travel,
            IWorldStatus worldStatus,
            IInputHandler inputHandler)   
        {
            _inputHandler = inputHandler;
            _inputHandler.Initialize(this);

            MainDisplay = Output;
            StatusDisplay = Status;
            _utility = Utility;
            travel = Travel;
            _party = Party;
            WorldStatus = worldStatus;
            GameMode = Modes.Travelling;
            _cancellationTokenSource = new CancellationTokenSource();

            MainDisplay.Write(_party.GetDisplayNames());

            travel.ModeChanged += OnModeChanged;


            // dummy loads
            Inventory = new Inventory();
            ShoppingEngine = new ShoppingEngine(new Display(), Inventory);

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
                            await travel.TravelLoop();
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
                    await Task.Delay(100, token);
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
        public string Weather { get; set; } = "Default";
    }
}