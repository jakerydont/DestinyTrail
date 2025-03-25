namespace DestinyTrail.Engine;

public class Occurrence : GameComponent, IOccurrence
{
    public string DisplayText { get; set; }
    public double Probability { get; set; }
    public string Effect {get;set;}
    
    public Occurrence() : this("Default Occurrence", "This occurrence does not exist", 0, "")
    {
        
    }
    public Occurrence(string name, string displayText, double probability, string effect)
    {
        Name = name;
        DisplayText = displayText;
        Probability = probability;
        Effect = effect;
    }
}