using DestinyTrail.Engine;
using DestinyTrail.Engine.Abstractions;

namespace DestinyTrail.Blazor.Components;


public partial class MainGame
{    
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

            var worldStatus = new WorldStatus();
            var configurationProvider = new DestinyTrail.Blazor.ConfigurationProvider();
            var fileReader = new DestinyTrail.Blazor.FileReader(HttpClient);
            var utility = new Utility(new YamlDeserializer(), fileReader, configurationProvider);
            var wagonParty = await WagonParty.CreateAsync(utility);
            var travel = await Travel.CreateAsync(wagonParty, utility, outputDisplay, worldStatus);
            game = await Game.CreateAsync(outputDisplay, statusDisplay, utility, wagonParty, travel, worldStatus, _inputHandler);
            

            await game.StartGameLoop();
        }
    }
}