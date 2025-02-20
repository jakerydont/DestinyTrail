// See https://aka.ms/new-console-template for more information
using DestinyTrail.Engine;
using DestinyTrail.TwitchIntegration;
using System.Threading;

var inputHandler = new InputHandler();

var display = new Display();
var utility = new Utility();

var wagonParty = await WagonParty.CreateAsync(utility);
var occurrenceEngine = OccurrenceEngine.CreateAsync(wagonParty, utility);

var worldStatus = new WorldStatus();

var travelEngine = await Travel.CreateAsync(wagonParty, utility, display, worldStatus);

var twitchChatService = new TwitchChatService();
var game = await Game.CreateAsync(display, display, utility, wagonParty, travelEngine, worldStatus, inputHandler,twitchChatService);
var gameLoopTask = game.StartGameLoop();
var inputTask = Task.Run(() => ProcessUserInput(game));

await Task.WhenAny(gameLoopTask, inputTask);

// Method to process user input
static void ProcessUserInput(Game game)
{
    while (true)
    {
        if (Console.IsInputRedirected)
        {

            string input = Console.ReadLine() ?? "";

            game._inputHandler.ProcessCommand(input);
        }
        else
        {
            // If input is not redirected, we read a single key press
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.Q)
                {

                    break;
                }
            }
        }
    }
}

