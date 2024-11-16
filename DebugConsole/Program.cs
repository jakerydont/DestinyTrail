// See https://aka.ms/new-console-template for more information
using DestinyTrail.Engine;
using System.Threading;

var game = new Game();
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

            game.InputHandler.ProcessCommand(input);
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

