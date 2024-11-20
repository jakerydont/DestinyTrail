namespace DestinyTrail.Engine
{
    public interface IGame
    {
        public IWagonParty Party { get; set; }
        public DateTime CurrentDate { get; set; }
        public double MilesToNextLandmark { get; set; }
        public Landmark NextLandmark { get; set; }
        public double MilesTraveled { get; set; }
        public IDisplay _display { get; set; }
        public Modes GameMode { get; }

        public Travel Travel {get;set;}
        public LandmarksData _landmarksData { get; set; }
        public ShoppingEngine ShoppingEngine { get; set; }
        public void ChangeMode(Modes mode);
        public void DrawStatusPanel();
    }
}