using DestinyTrail.Engine;
using DestinyTrail.Server;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DestinyTrail.DebugConsole
{
    // Example of how a console program would use MainGameBase
    public class ProgramExample : MainGameBase
    {
        private Display _consoleDisplay;
        
        public ProgramExample()
        {
            _consoleDisplay = new Display();
            
            // Initialize and start the game
            Task.Run(async () => 
            {
                await InitializeGameAsync();
                await StartGameLoopAsync();
                await ProcessUserInputAsync();
            });
        }

        protected override IDisplay OutputDisplay => _consoleDisplay;
        protected override IDisplay StatusDisplay => _consoleDisplay;

        protected override Utility CreateUtility()
        {
            return new Utility();
        }

        protected override ITwitchChatService CreateTwitchChatService()
        {
            return new TwitchChatService();
        }

        protected override void OnStateChanged()
        {
            // Console doesn't need special UI updates
        }

        private async Task ProcessUserInputAsync()
        {
            while (!_gameCts.IsCancellationRequested)
            {
                if (Console.IsInputRedirected)
                {
                    string input = Console.ReadLine() ?? "";
                    ProcessInput(input);
                }
                else
                {
                    // If input is not redirected, we read a single key press
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true).Key;
                        if (key == ConsoleKey.Q)
                        {
                            _gameCts.Cancel();
                            break;
                        }
                    }
                }
                
                await Task.Delay(100); // Small delay to prevent CPU hogging
            }
        }
    }

    // Example of how to use the ProgramExample class
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var gameController = new ProgramExample();
            
            // Wait for the game to complete
            await Task.Delay(Timeout.Infinite);
        }
    }
}