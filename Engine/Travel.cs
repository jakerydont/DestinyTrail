using System;

namespace DestinyTrail.Engine;

public class Travel : ITravel
{
    public event Action<Modes> ModeChanged;

    public IWagonParty Party { get; set; }

    public IUtility Utility { get; set; }

    public IDisplay Display { get; set; }

    public IOccurrenceEngine occurrenceEngine { get; set; }

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

    private Travel(IWagonParty party, IUtility utility, IDisplay display, IWorldStatus worldStatus)
    {
        ModeChanged += (mode) => { };

        Party = party;
        Utility = utility;
        Display = display;
        WorldStatus = worldStatus;

        MilesTraveled = 0;

        occurrenceEngine = OccurrenceEngine.Default;
        _landmarksData = new LandmarksData{Landmarks = []};
        NextLandmark = LandmarksData.Default;
        _paceData = new PaceData{Paces = []};
        Pace = PaceData.Default;

        _rationData = new RationData{Rations = []};
        Rations = RationData.Default;
        


    }

    public static async Task<Travel> CreateAsync(IWagonParty party, IUtility utility, IDisplay display, IWorldStatus worldStatus)
    {
        var travel = new Travel(party, utility, display, worldStatus);

        var occurrenceEngineTask = OccurrenceEngine.CreateAsync(travel.Party, travel.Utility);

        string pacesFilePath = travel.Utility.GetAppSetting("PacesFilePath");
        var paceDataTask = travel.Utility.LoadYamlAsync<PaceData>(pacesFilePath);

        string rationsFilePath = travel.Utility.GetAppSetting("RationsFilePath");
        var rationDataTask = travel.Utility.LoadYamlAsync<RationData>(rationsFilePath);

        string landmarksFilePath = travel.Utility.GetAppSetting("LandmarksFilePath");
        var landmarksDataTask = travel.Utility.LoadYamlAsync<LandmarksData>(landmarksFilePath);

        // Keep track of remaining tasks
        var tasks = new List<Task> { occurrenceEngineTask, paceDataTask, rationDataTask, landmarksDataTask };

        while (tasks.Any())
        {
            // Wait for any task to complete
            var completedTask = await Task.WhenAny(tasks);
            tasks.Remove(completedTask);


            if (completedTask == occurrenceEngineTask)
            {
                travel.occurrenceEngine = await occurrenceEngineTask;
            }
            else if (completedTask == paceDataTask)
            {
                travel._paceData = await paceDataTask;
                travel.Pace = travel._paceData.MinBy(pace => pace.Factor);
            }
            else if (completedTask == rationDataTask)
            {
                travel._rationData = await rationDataTask;
                travel.Rations = travel._rationData.MaxBy(rations => rations.Factor);
            }
            else if (completedTask == landmarksDataTask)
            {
                travel._landmarksData = await landmarksDataTask;
                travel.NextLandmark = travel._landmarksData.First();
                travel.MilesToNextLandmark = (double)travel.NextLandmark.Distance;
            }
        }
        return travel;
    }


    public async Task TravelLoop()
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
            var rawOccurrence = occurrenceEngine.PickRandomOccurrence();
            var processedOccurrence = occurrenceEngine.ProcessOccurrence(rawOccurrence);
            occurrenceMessage = processedOccurrence.DisplayText;
        }

        Party.SpendDailyHealth(Pace, Rations);



        await Display.Write($"{Utility.GetFormatted(WorldStatus.CurrentDate)}: {occurrenceMessage}");
        await Display.ScrollToBottom();

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

    public async Task ContinueTravelling()
    {
        // Simulating asynchronous work, such as logging or waiting for a task to complete
        await Task.Run(() => Display.Write($"You decided to continue."));

        // Assuming there's no async operation here; if so, keep as is.
        NextLandmark = Utility.NextOrFirst(
            _landmarksData.Landmarks,
            landmark => landmark.ID == NextLandmark.ID
        );
        MilesToNextLandmark = NextLandmark.Distance;

        // Invoke the mode change
        ModeChanged.Invoke(Modes.Travelling);
    }

}
