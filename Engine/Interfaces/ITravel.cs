namespace DestinyTrail.Engine;

public interface ITravel
{
    event Action<Modes> ModeChanged;
    public IWagonParty Party { get; set; }
    public IUtility Utility { get; set; }
    IOccurrenceEngine occurrenceEngine { get; set; }
    Pace Pace { get; }
    Rations Rations { get; set; }

    Task TravelLoop();
    Task ContinueTravelling();

    double MilesToNextLandmark { get; set; }
    double MilesTraveled { get; set; }
    public Landmark NextLandmark { get; set; }


}

