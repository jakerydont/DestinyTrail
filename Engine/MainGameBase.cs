using DestinyTrail.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DestinyTrail.Engine
{
    public abstract class MainGameBase : IDisposable
    {
        // Common properties
        protected Game? game;
        protected InputHandler _inputHandler;
        protected WorldStatus _worldStatus;
        protected Utility _utility;
        protected WagonParty _wagonParty;
        protected Travel _travel;
        protected ITwitchChatService _twitchChatService;
        protected CancellationTokenSource _gameCts;
        protected bool isGameInitialized;
        
        // Twitch chat related properties
        protected string _connectionStatus;
        protected List<(string Username, string Message)> _chatMessages = new();

        // Constructor
        protected MainGameBase()
        {
            _inputHandler = new InputHandler();
            _worldStatus = new WorldStatus();
            _gameCts = new CancellationTokenSource();
            isGameInitialized = false;
            _connectionStatus = "Disconnected";
        }

        // Abstract properties for platform-specific implementations
        protected abstract IDisplay OutputDisplay { get; }
        protected abstract IDisplay StatusDisplay { get; }

        // Common initialization method
        protected virtual async Task InitializeGameAsync()
        {
            try 
            {
                _utility = CreateUtility();
                _wagonParty = await WagonParty.CreateAsync(_utility);
                _travel = await Travel.CreateAsync(_wagonParty, _utility, OutputDisplay, _worldStatus);
                _twitchChatService = CreateTwitchChatService();
                
                // Set up Twitch chat event handlers
                _twitchChatService.OnConnectionStatusChanged += HandleConnectionStatusChanged;
                _twitchChatService.OnMessageReceived += HandleMessageReceived;
                
                game = await Game.CreateAsync(
                    OutputDisplay, 
                    StatusDisplay, 
                    _utility, 
                    _wagonParty, 
                    _travel, 
                    _worldStatus, 
                    _inputHandler, 
                    _twitchChatService
                );

                isGameInitialized = true;
            }
            catch (Exception ex)
            {
                // Handle initialization error
                await OutputDisplay.Write($"Error initializing game: {ex.Message}");
            }
        }

        // Abstract methods for platform-specific implementations
        protected abstract Utility CreateUtility();
        protected abstract ITwitchChatService CreateTwitchChatService();
        protected abstract void OnStateChanged();
        
        // Virtual method for starting the game loop
        protected virtual async Task StartGameLoopAsync()
        {
            if (game != null)
            {
                await game.StartGameLoop(_gameCts.Token);
            }
        }

        // Virtual method for processing input
        protected virtual void ProcessInput(string input)
        {
            _inputHandler?.ProcessCommand(input);
        }

        // Twitch chat event handlers
        protected virtual void HandleConnectionStatusChanged(string status)
        {
            _connectionStatus = status;
            OnStateChanged();
        }

        protected virtual void HandleMessageReceived(string username, string message)
        {
            _chatMessages.Add((username, message));
            OnStateChanged();
        }

        // Common cleanup method
        public virtual void Dispose()
        {
            _gameCts.Cancel();
            _gameCts.Dispose();
            
            if (game?.TwitchChatService != null)
            {
                game.TwitchChatService.OnConnectionStatusChanged -= HandleConnectionStatusChanged;
                game.TwitchChatService.OnMessageReceived -= HandleMessageReceived;
            }

            game?.StopGameLoop().Wait();
        }
    }
}