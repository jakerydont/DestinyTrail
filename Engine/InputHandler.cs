namespace DestinyTrail.Engine;

public class InputHandler : IInputHandler
{
    private IGame? Game;

    public void Initialize(IGame game)
    {
        Game = game;
    }

    public virtual void ProcessCommand(string command)
    {
        _ = ProcessCommandAsync(command);
    }

    public virtual async Task ProcessCommandAsync(string command) 
    {
        if (Game == null)  throw new NullReferenceException("Game variable null. Initialize method must be called to set game variable.");
        if (Game.GameMode == Modes.AtLandmark)
        {
            if (command == "")
            {
                await Game.travel.ContinueTravelling();
            }

            if (command == "buy")
            {
                Game.ChangeMode(Modes.Shopping);
            }
        }

        if (Game.GameMode == Modes.Shopping)
        {
            Game.ShoppingEngine.ProcessInput(command);
        }
    }
}
