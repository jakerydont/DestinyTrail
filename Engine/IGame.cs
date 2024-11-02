namespace DestinyTrail.Engine
{
    public interface IGame
    {
        public WagonParty Party { get; set; }
        public DateTime CurrentDate { get; set; }
        public double MilesToNextLandmark { get; set; }
        public Landmark NextLandmark { get; set; }
        public double MilesTraveled { get; set; }
        public Display _display { get; set; }
        public Modes GameMode { get; }

        public LandmarksData _landmarksData { get; set; }
        public void ChangeMode(Modes atLandmark);
        public void DrawStatusPanel();
    }
}