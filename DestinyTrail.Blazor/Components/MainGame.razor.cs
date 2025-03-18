using DestinyTrail.Engine;
using DestinyTrail.Engine.Abstractions;


namespace DestinyTrail.Blazor.Components;


public partial class MainGame
{    

    private string _connectionStatus;
    private string _lastChatMessage;
    private List<(string Username, string Message)> _chatMessages = new();
    private CancellationTokenSource _gameCts = new();

    protected override async Task OnInitializedAsync()
    {
        await Task.Run(() => 
        {
            if (game?.TwitchChatService != null)
            {
                game.TwitchChatService.OnConnectionStatusChanged += HandleConnectionStatusChanged;
                game.TwitchChatService.OnMessageReceived += HandleMessageReceived;
            }}
        );
    }

    private void HandleConnectionStatusChanged(string status)
    {
        _connectionStatus = status;
        InvokeAsync(StateHasChanged);
    }


    private Game? game { get; set; }
    private BlazorDisplay? outputDisplay;
    private BlazorDisplay? statusDisplay;
    private bool isGameInitialized = false;

    private string commandInput = "";

    private InputHandler _inputHandler;

    public MainGame() {
        _inputHandler = new InputHandler();
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !isGameInitialized)
        {
            if (outputDisplay == null || statusDisplay == null)
            {
                throw new InvalidOperationException("Blazor displays are not initialized.");
            }

            try 
            {
                var worldStatus = new WorldStatus();
                var configurationProvider = new DestinyTrail.Blazor.ConfigurationProvider();
                var fileReader = new DestinyTrail.Blazor.FileReader(HttpClient);
                var utility = new Utility(new YamlDeserializer(), fileReader, configurationProvider);
                var wagonParty = await WagonParty.CreateAsync(utility);
                var travel = await Travel.CreateAsync(wagonParty, utility, outputDisplay, worldStatus);
                var twitchChatService = new TwitchIntegration.TwitchChatService();
                
                game = await Game.CreateAsync(
                    outputDisplay, 
                    statusDisplay, 
                    utility, 
                    wagonParty, 
                    travel, 
                    worldStatus, 
                    _inputHandler, 
                    twitchChatService
                );

                isGameInitialized = true;

                // Start game loop in background
                await game.StartGameLoop(_gameCts.Token);
            }
            catch (Exception ex)
            {
                // Handle initialization error
                outputDisplay?.Write($"Error initializing game: {ex.Message}");
            }
        }
    }

    private void HandleMessageReceived(string username, string message)
    {
        _chatMessages.Add((username, message));
        _lastChatMessage = $"{username}: {message}";
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
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