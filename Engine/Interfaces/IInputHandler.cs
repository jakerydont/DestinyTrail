namespace DestinyTrail.Engine;

public interface IInputHandler
{
    public void Initialize(IGame game);
    public void ProcessCommand(string command);
    public Task ProcessCommandAsync(string command);
}
