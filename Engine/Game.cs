
using Avalonia.Controls;
using Avalonia.Threading;
namespace DestinyTrail.Engine
{
    public partial class Game {

        private CancellationTokenSource _cancellationTokenSource {get;set;}
        private OccurrenceEngine occurrenceEngine;
        private PaceData paceData;
        private Pace pace;
        private DateTime currentDate;

        private Display _display {get;set;}
        public int milesTraveled { get; private set; }
        public string[] statuses { get; private set; }
        public string[] randomNames { get; private set; }

        private WagonParty party;

        public Game() {
            _display = new Display();
            Initialize();
        }

        public Game(ListBox Output)
        { 
            _display = new Display(Output);
            Initialize();
        }


        private void Initialize() {
                        _cancellationTokenSource = new CancellationTokenSource();

            milesTraveled = 0;

                    string occurrencesFilePath = "data/Occurrences.yaml"; // Update this path as needed
                    string statusesFilePath = "data/Statuses.yaml"; // Path to the new statuses file
                    string pacesFilePath = "data/Paces.yaml"; // Path to the new paces file
                    string randomNamesPath = "data/RandomNames.yaml";

                   statuses = Utility.LoadYaml<StatusData>(statusesFilePath).Statuses.ToArray();
                    randomNames = Utility.LoadYaml<RandomNamesData>(randomNamesPath).RandomNames.ToArray();

                     party = new WagonParty(randomNames);
                    _display.Write(party.GetNames());

                     occurrenceEngine = new OccurrenceEngine(occurrencesFilePath, party, statuses);

                    // Load paces from the YAML file
                     paceData = Utility.LoadYaml<PaceData>(pacesFilePath);
                    pace = paceData.Paces.First(pace => pace.Name == "grueling");


                    // Start with an initial date
                    currentDate = new DateTime(1860, 10, 1); // Start from October 1, 1850
        }
        public async void StartGameLoop()
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(5000, token); // Wait for 5 seconds
                    MainLoop(); // Update the UI
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, handle if needed
            }
        }
        public void MainLoop() {
                 
           

                _display.Clear();

                // Pick a random occurrence based on probability
                Occurrence randomOccurrence = occurrenceEngine.PickRandomOccurrence();
                var occurrence = occurrenceEngine.ProcessOccurrence(randomOccurrence);


                // Output the date and display text of the occurrence along with the person's name
                _display.Write($"\n{currentDate:MMMM d, yyyy}\n------\n{occurrence.DisplayText}");


                milesTraveled += pace.Factor;
                // Output the date and display text of the occurrence along with the person's name

                _display.Write($"Distance traveled: {milesTraveled} miles ({milesTraveled} km)");
            

                // Increment the date by one day
                currentDate = currentDate.AddDays(1);

                // Wait for 5 seconds before picking a new occurrence
                //await Task.Delay(5000); // 5000 milliseconds = 5 seconds
         
        }
    }
}