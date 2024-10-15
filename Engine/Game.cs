
using System.Formats.Asn1;
using Avalonia.Controls;
using Avalonia.Threading;
namespace DestinyTrail.Engine
{
    public class Game {

        private CancellationTokenSource _cancellationTokenSource {get;set;}
        private OccurrenceEngine _occurrenceEngine;
        private PaceData _paceData;
        private Pace _pace;
        private DateTime _currentDate;

        private RationData _rationData {get;set;}

        private Rations _rations {get;set;}

        private LandmarksData _landmarksData {get;set;}

        private string _weather = "not implemented";

        protected Display _display {get;set;}

        protected Display _status {get;set;}

        public int MilesTraveled { get; private set; }
        public string[] Statuses { get; private set; }
        public string[] RandomNames { get; private set; }

        private WagonParty _party {get;set;}
        private string _milesToNextLandmark = "not implemented";

        public Game() 
            : this(new Display(), new Display()) {}

        public Game(ListBox Output, ListBox Status)  
            : this(new Display(Output), new Display(Status)) {}
  
        public Game(Display Output, Display Status) {
            _display = Output;
            _status = Status;
            _cancellationTokenSource = new CancellationTokenSource();

            MilesTraveled = 0;

            string occurrencesFilePath = "data/Occurrences.yaml";
            string statusesFilePath = "data/Statuses.yaml"; 
            string pacesFilePath = "data/Paces.yaml"; 
            string rationsFilePath = "data/Rations.yaml";
            string randomNamesPath = "data/RandomNames.yaml";
            string landmarksFilePath = "data/Landmarks.yaml";
            

            Statuses = [.. Utility.LoadYaml<StatusData>(statusesFilePath).Statuses];
            RandomNames = [.. Utility.LoadYaml<RandomNamesData>(randomNamesPath).RandomNames];

            _party = new WagonParty(RandomNames);
            _display.Write(_party.GetDisplayNames());

            _occurrenceEngine = new OccurrenceEngine(occurrencesFilePath, _party, Statuses);


            _paceData = Utility.LoadYaml<PaceData>(pacesFilePath);
            _pace = _paceData.Paces.MinBy(pace => pace.Factor)!;

            _landmarksData = Utility.LoadYaml<LandmarksData>(landmarksFilePath);
            

            _rationData = Utility.LoadYaml<RationData>(rationsFilePath);
            _rations = _rationData.Rations.MaxBy(rations => rations.Factor)!;

            _currentDate = new DateTime(1860, 10, 1);
        }
        public async void StartGameLoop()
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    MainLoop(); 
                    await Task.Delay(5000, token); 
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, handle if needed
            }
        }
        public void MainLoop()
        {
            _currentDate = _currentDate.AddDays(1);
            MilesTraveled += CalculateMilesTraveled();

            Occurrence randomOccurrence = _occurrenceEngine.PickRandomOccurrence();
            var occurrence = _occurrenceEngine.ProcessOccurrence(randomOccurrence);

            _party.SpendDailyHealth(_pace, _rations);

            _status.Clear();
            _status.Write($"Date: {_currentDate:MMMM d, yyyy}");
            _status.Write($"Weather: {_weather}");
            _status.Write($"Health: {_party.GetDisplayHealth()}");
            _status.Write($"Distance to next landmark: {_milesToNextLandmark}");
            _status.Write($"Distance traveled: {MilesTraveled} miles ({MilesTraveled} km)");

            _display.Write($"\n{_currentDate:MMMM d, yyyy}\n------\n{occurrence.DisplayText}");
            _display.ScrollToBottom(); 
        }

        private int CalculateMilesTraveled()
        {
            // TODO: factor in oxen like ( _pace.Factor / (Inventory.currentOxen / Inventory.maximumOxen ))
            return _pace.Factor ;
        }
    }
}