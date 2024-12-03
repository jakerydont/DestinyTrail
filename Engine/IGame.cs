namespace DestinyTrail.Engine
{
    public interface IGame
    {
        public IWagonParty _party { get; set; }
        public DateTime CurrentDate { get; set; }


        public IDisplay _display { get; set; }
        public Modes GameMode { get; }

        public ITravel _travel {get;set;}

        public IShoppingEngine ShoppingEngine { get; set; }
        public void ChangeMode(Modes mode);
        public void DrawStatusPanel();
    }
}