namespace DestinyTrail.Engine;

public interface IOccurrence
{
    string DisplayText { get; set; }
    double Probability { get; set; }
    string Effect { get; set; }
}
