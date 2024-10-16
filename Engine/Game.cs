
using System.Dynamic;
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
        private Landmark _nextLandmark {get;set;}

        private string _weather = "not implemented";

        protected Display _display {get;set;}

        protected Display _status {get;set;}

        public double MilesTraveled { get; private set; }
        public double MilesToNextLandmark {get; private set; }
        public string[] Statuses { get; private set; }
        public string[] RandomNames { get; private set; }

        private WagonParty _party {get;set;}


        private bool _advanceDay = true;

        public Modes GameMode {get;set;}

        private bool _shouldInitializeAtLandmark {get;set;}

        public Game() 
            : this(new Display(), new Display()) {}

        public Game(ListBox Output, ListBox Status)  
            : this(new Display(Output), new Display(Status)) {}
  
        public Game(Display Output, Display Status) {
            _display = Output;
            _status = Status;
            _cancellationTokenSource = new CancellationTokenSource();



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
            _nextLandmark = _landmarksData.Landmarks!.First();

            MilesTraveled = 0;
            MilesToNextLandmark = (double)_nextLandmark.Distance!;

            _rationData = Utility.LoadYaml<RationData>(rationsFilePath);
            _rations = _rationData.Rations.MaxBy(rations => rations.Factor)!;

            _currentDate = new DateTime(1860, 10, 1);

            GameMode = Modes.Travelling;
        }
        public async void StartGameLoop()
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    switch (GameMode)
                    {
                        case Modes.Travelling:
                            TravelLoop(); 
                            break;
                        case Modes.AtLandmark:
                            AtLandmarkLoop();
                            break;
                        default:
                            break;
                    }
                   await Task.Delay(5000, token); 
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, handle if needed
            }
        }

        private void AtLandmarkLoop()
        {
            if (!_shouldInitializeAtLandmark) return;
            _shouldInitializeAtLandmark = false;
            _display.Clear();
            _display.Write($"{_nextLandmark.Name}");
            _display.Write("Press enter to continue");
            
        }

        public void TravelLoop()
        {
            _display.Write($"\n{_currentDate:MMMM d, yyyy}\n------");

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
                occurrenceMessage = $"You have reached {_nextLandmark.Name}.";
                GameMode = Modes.AtLandmark;
                _shouldInitializeAtLandmark = true;
            }
            else 
            {
                Occurrence randomOccurrence = _occurrenceEngine.PickRandomOccurrence();
                var occurrence = _occurrenceEngine.ProcessOccurrence(randomOccurrence);
                occurrenceMessage = occurrence.DisplayText;
            }

            _party.SpendDailyHealth(_pace, _rations);

            _status.Clear();
            _status.Write($"Date: {_currentDate:MMMM d, yyyy}");
            _status.Write($"Weather: {_weather}");
            _status.Write($"Health: {_party.GetDisplayHealth()}");
            _status.Write($"Distance to next landmark: {MilesToNextLandmark.Abbreviate()} miles ({MilesToNextLandmark.Abbreviate()} km)");
            _status.Write($"Distance traveled: {MilesTraveled.Abbreviate()} miles ({MilesTraveled.Abbreviate()} km)");

            _display.Write($"{occurrenceMessage}");
            _display.ScrollToBottom(); 

            if (_advanceDay) {
                _currentDate = _currentDate.AddDays(1);
            }
        }

        private double CalculateMilesTraveled()
        {
            // TODO: factor in oxen like ( _pace.Factor / (Inventory.currentOxen / Inventory.maximumOxen ))
            return _pace.Factor ;
        }

        public void ContinueTravelling()
        {

            _display.Items.Add($"You decided to continue.");
            _nextLandmark = _landmarksData.Landmarks.NextOrFirst(landmark => landmark.ID == _nextLandmark.ID);
            MilesToNextLandmark = _nextLandmark.Distance;
            GameMode = Modes.Travelling;
        }
    }
}