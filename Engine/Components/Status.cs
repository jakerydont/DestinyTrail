namespace DestinyTrail.Engine
{
    public class Status : GameComponent
    {
        public static Status Dead = new() { Name = "dead", UpperThreshold = 0};

        public int UpperThreshold { get; set; }
    }
}
