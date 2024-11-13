namespace DestinyTrail.Engine
{
    public class Occurrence : GameComponent
    {
        public required string DisplayText { get; set; }
        public double Probability { get; set; }
        public required string Effect {get;set;}
    }
}
