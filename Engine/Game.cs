using System.Dynamic;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using YamlDotNet.Serialization;
using System.Configuration;
using YamlDotNet.Core.Tokens;
using System.Reflection.Metadata.Ecma335;
using DestinyTrail.Server;

namespace DestinyTrail.Engine
{
    public class Game : IGame
    {
        public ITwitchChatService TwitchChatService {get;set;}
        private bool _shouldInitializeGameOver;

        private CancellationTokenSource _defaultCancellationTokenSource { get; set; }

        public ITravel travel {get;set;}

        public IInventory Inventory { get; set; }
        public IInputHandler _inputHandler { get; set; }

        public IDisplay MainDisplay { get; set; }

        protected IDisplay StatusDisplay { get; set; }

        private IUtility _utility { get; set; }
        
        public IWorldStatus WorldStatus { get; set; }
        public IWagonParty _party { get; set; }

        private Modes _gameMode;
        public Modes GameMode 
        { 
            get => _gameMode; 
            set => ChangeMode(value); 
        }
        public IShoppingEngine ShoppingEngine { get; set; }
        private bool _shouldInitializeAtLandmark { get; set; }

        private Task _gameLoopTask;
        private bool _isRunning;

        public static async Task<Game> CreateAsync(IDisplay Output, 
            IDisplay Status, 
            IUtility Utility, 
            IWagonParty Party,
            ITravel Travel,
            IWorldStatus worldStatus,
            IInputHandler InputHandler,
            ITwitchChatService twitchChatService) 
        {
            var game = new Game(Output,    Status,   Utility,   Party,  Travel, worldStatus, InputHandler, twitchChatService);
            string inventoryFilePath = Utility.GetAppSetting("InventoryFilePath");
            string inventoryCustomItemsFilePath = Utility.GetAppSetting("InventoryCustomItemsFilePath");
            game.Inventory = await Utility.LoadYamlAsync<Inventory>(inventoryFilePath);
            game.Inventory.CustomItems = await Utility.LoadYamlAsync<Inventory>(inventoryCustomItemsFilePath);
            game.ShoppingEngine = new ShoppingEngine(game.MainDisplay, game.Inventory);

                        //var secrets = TwitchAuth.LoadSecrets();
            game.TwitchChatService.Initialize("jakerydont","rq9a4v2r0l2co77um7f3nm06xd2iaz","jakerydont");
            await game.TwitchChatService.ConnectAsync();

            return game;
        }

        private Game(
            IDisplay Output, 
            IDisplay Status, 
            IUtility Utility, 
            IWagonParty Party,
            ITravel Travel,
            IWorldStatus worldStatus,
            IInputHandler inputHandler,
            ITwitchChatService twitchChatService)   
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

            _defaultCancellationTokenSource = new CancellationTokenSource();

            MainDisplay.Write(_party.GetDisplayNames());

            travel.ModeChanged += OnModeChanged;


            // dummy loads
            Inventory = new Inventory();
            ShoppingEngine = new ShoppingEngine(new Display(), Inventory);

            TwitchChatService = twitchChatService;


        }

        private void OnModeChanged(Modes mode)
        {
            ChangeMode(mode);
        }

    public async Task StartGameLoop(CancellationToken? cts = null)
    {
        if (_isRunning) return; // Prevent multiple loops
        
        _isRunning = true;
        var token = cts ?? _defaultCancellationTokenSource.Token;

        _gameLoopTask = Task.Run(async () =>
        {
            try
            {
                while (_isRunning && !token.IsCancellationRequested)
                {
                    await ProcessGameTick();
                    await Task.Delay(1000, token); // 1 second delay between ticks
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully
                MainDisplay.Write("Game loop cancelled.");
            }
            catch (Exception ex)
            {
                MainDisplay.Write($"Error in game loop: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
            }
        }, token);

        await Task.CompletedTask; // Return immediately while loop runs in background
    }

    private async Task ProcessGameTick()
    {
        // Process chat messages
        foreach (var (username, message) in TwitchChatService.GetMessages())
        {
            ProcessChatMessage(username, message);
        }

        if (GameMode != Modes.GameOver)
        {
            _party.KillCheckParty();
            if (!_party.IsAnybodyAlive())
            {
                GameMode = Modes.GameOver;
            }
        }

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
            case Modes.GameOver:
                GameOverLoop();
                break;
        }
    }

    public async Task StopGameLoop()
    {
        _isRunning = false;
        if (_gameLoopTask != null)
        {
            await _gameLoopTask;
        }
    }
    private void ProcessQueuedMessages()
    {
        foreach (var (username, message) in TwitchChatService.GetMessages())
        {
            ProcessChatMessage(username, message);
        }
    }

    private void ProcessChatMessage(string username, string message)
    {
        // Handle game-specific commands
        if (message.StartsWith("!help", StringComparison.OrdinalIgnoreCase))
        {
            TwitchChatService.SendMessageAsync("Available commands: !status, !health, !travel, !shop").Wait();
            return;
        }

        if (message.StartsWith("!status", StringComparison.OrdinalIgnoreCase))
        {
            var status = $"Current mode: {GameMode}, Miles traveled: {_utility.Abbreviate(travel.MilesTraveled)}";
            TwitchChatService.SendMessageAsync(status).Wait();
            return;
        }

        if (message.StartsWith("!health", StringComparison.OrdinalIgnoreCase))
        {
            var health = _party.GetDisplayHealth();
            TwitchChatService.SendMessageAsync($"Party health: {health}").Wait();
            return;
        }

        // Add more game-specific commands as needed
        switch (GameMode)
        {
            case Modes.Shopping when message.StartsWith("!shop", StringComparison.OrdinalIgnoreCase):
                HandleShopCommand(username, message);
                break;
            case Modes.Travelling when message.StartsWith("!travel", StringComparison.OrdinalIgnoreCase):
                HandleTravelCommand(username, message);
                break;
        }
    }

    private void HandleShopCommand(string username, string message)
    {
        // Implement shop-specific commands
        MainDisplay.Write($"{username} is shopping");
    }

    private void HandleTravelCommand(string username, string message)
    {
        // Implement travel-specific commands
        MainDisplay.Write($"{username} is affecting travel");
    }

        private void AtLandmarkLoop()
        {
            if (!_shouldInitializeAtLandmark) return;
            _shouldInitializeAtLandmark = false;
            MainDisplay.WriteTitle(travel.NextLandmark.Name);

            //TODO: Adapt for GUI
            MainDisplay.Write("Press enter to continue.");

        }

        private void GameOverLoop()
        {
            if (!_shouldInitializeGameOver) return;
            _shouldInitializeGameOver = false;
            MainDisplay.WriteTitle("GAME OVER");

            //TODO: Adapt for GUI
            MainDisplay.Write("Everybody in your party died or went lost and is unaccounted for. Many wagons fail to fulfill their destiny.");
            DrawStatusPanel();
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
            _gameMode = mode;
            if (_gameMode == Modes.AtLandmark)
            {
                _shouldInitializeAtLandmark = true;
            }
            else if (_gameMode == Modes.GameOver) 
            {
                _shouldInitializeGameOver = true;
            }
            else if (_gameMode == Modes.Shopping)
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