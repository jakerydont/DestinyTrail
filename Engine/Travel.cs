using System;

namespace DestinyTrail.Engine;

public class Travel : ITravel
{
    public event Action<Modes> ModeChanged;
    
    public IWagonParty Party { get;  set; }

    public IUtility Utility {get;set;}

    public IDisplay Display { get; set; }

    public IOccurrenceEngine OccurrenceEngine { get; set; }

    private PaceData _paceData;
    public Pace Pace { get; set; }

    public Rations Rations { get; set; }
    private RationData _rationData { get; set; }

    private bool _advanceDay = true;
    public LandmarksData _landmarksData;

    public Landmark NextLandmark { get; set; }
    public double MilesToNextLandmark { get; set; }
    public double MilesTraveled { get; set; }

    public IWorldStatus WorldStatus { get; set; }

    public Travel(IWagonParty party, IUtility utility, IDisplay display, IWorldStatus worldStatus)
    {
        Party = party;
        Utility = utility;
        Display = display;
        WorldStatus = worldStatus;


        OccurrenceEngine = new OccurrenceEngine(Party, Utility);

        string pacesFilePath = Utility.GetAppSetting("PacesFilePath");
        _paceData = Utility.LoadYaml<PaceData>(pacesFilePath);
        Pace = _paceData.MinBy(pace => pace.Factor);

        string rationsFilePath = Utility.GetAppSetting("RationsFilePath");
        _rationData = Utility.LoadYaml<RationData>(rationsFilePath);
        Rations = _rationData.MaxBy(rations => rations.Factor);

        string landmarksFilePath = Utility.GetAppSetting("LandmarksFilePath");
        _landmarksData = Utility.LoadYaml<LandmarksData>(landmarksFilePath);
        NextLandmark = _landmarksData.First();

        MilesTraveled = 0;
        MilesToNextLandmark = (double)NextLandmark.Distance;


    }
    public void TravelLoop()
    {
        var todaysMiles = CalculateMilesTraveled();
        if (todaysMiles > MilesToNextLandmark)
        {
            todaysMiles = MilesToNextLandmark;
        }
        MilesTraveled += todaysMiles;
        MilesToNextLandmark -= todaysMiles;

        string occurrenceMessage = "";
        if (MilesToNextLandmark <= 0)
        {
            occurrenceMessage = $"You have reached {NextLandmark.Name}.";
            ModeChanged.Invoke(Modes.AtLandmark);

        }

        else
        {
            Occurrence randomOccurrence = OccurrenceEngine.PickRandomOccurrence();
            var occurrence = OccurrenceEngine.ProcessOccurrence(randomOccurrence);
            occurrenceMessage = occurrence.DisplayText;
        }

        Party.SpendDailyHealth(Pace, Rations);



        Display.Write($"{Utility.GetFormatted(WorldStatus.CurrentDate)}: {occurrenceMessage}");
        Display.ScrollToBottom();

        if (_advanceDay)
        {
            WorldStatus.CurrentDate = WorldStatus.CurrentDate.AddDays(1);
        }
    }
    private double CalculateMilesTraveled()
    {
        // TODO: factor in oxen like ( _pace.Factor / (Inventory.currentOxen / Inventory.maximumOxen ))
        return Pace.Factor;
    }

    public void ContinueTravelling()
    {
        Display.Write($"You decided to continue.");
        NextLandmark = Utility.NextOrFirst(
            _landmarksData.Landmarks,
            landmark => landmark.ID == NextLandmark.ID
            );
        MilesToNextLandmark = NextLandmark.Distance;
        ModeChanged.Invoke(Modes.Travelling);

    }
}
