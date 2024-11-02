
using System.Dynamic;
using System.Formats.Asn1;
using Avalonia.Controls;
using Avalonia.Threading;
namespace DestinyTrail.Engine
{
    public class Game : IGame {

        private CancellationTokenSource _cancellationTokenSource {get;set;}

        public DateTime CurrentDate {get;set;}
        public Travel _travel;


        public LandmarksData _landmarksData {get;set;}
        public Landmark NextLandmark {get;set;}

        private string _weather = "not implemented";

        public Display _display {get;set;}

        protected Display _status {get;set;}

        public double MilesTraveled { get; set; }
        public double MilesToNextLandmark {get; set; }
        public string[] RandomNames { get; private set; }

        public WagonParty Party {get;set;}


        public Modes GameMode {get;private set;}

        private bool _shouldInitializeAtLandmark {get;set;}

        public Game() 
            : this(new Display(), new Display()) {}

        public Game(ListBox Output, ListBox Status)  
            : this(new Display(Output), new Display(Status)) {}
  
        public Game(Display Output, Display Status) {
            _display = Output;
            _status = Status;
            _cancellationTokenSource = new CancellationTokenSource();

            string randomNamesPath = "data/RandomNames.yaml";
            string landmarksFilePath = "data/Landmarks.yaml";
            
            RandomNames = [.. Utility.LoadYaml<RandomNamesData>(randomNamesPath)];

            Random.Shared.Shuffle(RandomNames);
            var partyNames = RandomNames.Take(26).ToArray();

            Party = new WagonParty(RandomNames);
            _display.Write(Party.GetDisplayNames());

            _landmarksData = Utility.LoadYaml<LandmarksData>(landmarksFilePath);
            NextLandmark = _landmarksData.First();

            MilesTraveled = 0;
            MilesToNextLandmark = (double)NextLandmark.Distance!;


            CurrentDate = new DateTime(1860, 10, 1);

            _travel = new Travel(this);

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
                            _travel.TravelLoop(); 
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
            _display.Write($"{NextLandmark.Name}");
            _display.Write("Press enter to continue");
            
        }


        public void DrawStatusPanel()
        {
            _status.Clear();
            _status.Write($"Date: {CurrentDate.GetFormatted()}");
            _status.Write($"Weather: {_weather}");
            _status.Write($"Health: {Party.GetDisplayHealth()}");
            _status.Write($"Distance to next landmark: {MilesToNextLandmark.Abbreviate()} miles ({MilesToNextLandmark.Abbreviate()} km)");
            _status.Write($"Distance traveled: {MilesTraveled.Abbreviate()} miles ({MilesTraveled.Abbreviate()} km)");

            _status.Write($"-----------");
            foreach(var person in Party.Members) {
                _status.Write($"{person.Name} ..... {person.Status}");
            }
        }

        public void ChangeMode(Modes mode)
        {
            GameMode = mode;
            if (GameMode == Modes.AtLandmark)
            { 
                _shouldInitializeAtLandmark = true;
            }
        }
    }
}