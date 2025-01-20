using System;
using DestinyTrail.Engine;


namespace DestinyTrail.Blazor.Components;

public partial class MainGame 
{
    Game game { get; set; }
    public MainGame()
    {
        var OutputDisplay = new BlazorDisplay();
        var StatusDisplay = new BlazorDisplay();
        var WorldStatus = new WorldStatus();
        var configurationProvider = new ConfigurationProvider();
        var Utility = new Utility(configurationProvider);
        var WagonParty = new WagonParty(Utility);
        var Travel = new Travel(WagonParty, Utility, StatusDisplay, WorldStatus);
        game = new Game(OutputDisplay, StatusDisplay, Utility, WagonParty, Travel, WorldStatus);
        game.StartGameLoop();
    }
}
