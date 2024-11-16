using System;

namespace DestinyTrail.Engine;

public class InputHandler
{
    private IGame Game;

    public InputHandler(IGame game)
    {
        Game = game;
    }
public void ProcessCommand(string input)
{
    if (Game.GameMode == Modes.AtLandmark)
    {
        if (input == "")
        {
            Game.Travel.ContinueTravelling();
        }

        if (input == "buy")
        {
            Game.ChangeMode(Modes.Shopping);
        }
    }

    if (Game.GameMode == Modes.Shopping)
    {
        Game.ShoppingEngine.ProcessInput(input);
    }
}
}