namespace DestinyTrail.Engine.Interfaces;

public interface IGame
{

    public IInputHandler _inputHandler { get; set; }
    public IWagonParty _party { get; set; }



    public IDisplay MainDisplay { get; set; }
    public Modes GameMode { get; }

    public ITravel travel {get;set;}

    public IShoppingEngine ShoppingEngine { get; set; }
    public void ChangeMode(Modes mode);
    public void DrawStatusPanel();
}