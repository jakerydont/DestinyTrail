
using Avalonia.Controls;
using Avalonia.Threading;
namespace DestinyTrail.Engine
{
    public partial class Game {

        private CancellationTokenSource _cancellationTokenSource {get;set;}
        private OccurrenceEngine _occurrenceEngine;
        private PaceData _paceData;
        private Pace _pace;
        private DateTime _currentDate;

        private string _weather = "not implemented";

        private Display _display {get;set;}

        private Display _status;

        public int milesTraveled { get; private set; }
        public string[] statuses { get; private set; }
        public string[] randomNames { get; private set; }

        private WagonParty _party {get;set;}
        private string _milesToNextLandmark = "not implemented";

        public Game() {
            _display = new Display();
            _status = new Display();
            Initialize();
        }

        public Game(ListBox Output, ListBox Status)
        { 
            _display = new Display(Output);
            _status = new Display(Status);
            Initialize();
        }


        private void Initialize() {
            _cancellationTokenSource = new CancellationTokenSource();

            milesTraveled = 0;

            string occurrencesFilePath = "data/Occurrences.yaml";
            string statusesFilePath = "data/Statuses.yaml"; 
            string pacesFilePath = "data/Paces.yaml"; 
            string randomNamesPath = "data/RandomNames.yaml";

            statuses = Utility.LoadYaml<StatusData>(statusesFilePath).Statuses.ToArray();
            randomNames = Utility.LoadYaml<RandomNamesData>(randomNamesPath).RandomNames.ToArray();

            _party = new WagonParty(randomNames);
            _display.Write(_party.GetNames());

            _occurrenceEngine = new OccurrenceEngine(occurrencesFilePath, _party, statuses);


            _paceData = Utility.LoadYaml<PaceData>(pacesFilePath);
            _pace = _paceData.Paces.First(pace => pace.Name == "grueling");

            _currentDate = new DateTime(1860, 10, 1);
        }
        public async void StartGameLoop()
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    MainLoop(); // Update the UI
                    await Task.Delay(5000, token); // Wait for 5 seconds
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
            milesTraveled += CalculateMilesTraveled();

            Occurrence randomOccurrence = _occurrenceEngine.PickRandomOccurrence();
            var occurrence = _occurrenceEngine.ProcessOccurrence(randomOccurrence);

            _status.Clear();
            _status.Write($"Date: {_currentDate:MMMM d, yyyy}");
            _status.Write($"Weather: {_weather}");
            _status.Write($"Health: {_party.GetHealth()}");
            _status.Write($"Distance to next landmark: {_milesToNextLandmark}");
            _status.Write($"Distance traveled: {milesTraveled} miles ({milesTraveled} km)");

            _display.Write($"\n{_currentDate:MMMM d, yyyy}\n------\n{occurrence.DisplayText}");
            _display.ScrollToBottom(); 
        }

        private int CalculateMilesTraveled()
        {
            return _pace.Factor;
        }
    }
}