namespace DestinyTrail.Engine
{
    public class Occurrence : GameComponent, IOccurrence
    {
        public required string DisplayText { get; set; }
        public double Probability { get; set; }
        public required string Effect {get;set;}
        
        public Occurrence(string name, string displayText, double probability, string effect)
        {
            Name = name;
            DisplayText = displayText;
            Probability = probability;
            Effect = effect;
        }
    }
}