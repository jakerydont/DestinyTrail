using System;

namespace DestinyTrail.Engine;

public class Travel
{
    IGame _game { get; set; }
    public string[] Statuses { get; private set; }

    public OccurrenceEngine _occurrenceEngine { get; set; }

    private PaceData _paceData;
    public Pace _pace;

    public Rations _rations {get;set;}
    private RationData _rationData {get;set;}
    
    private bool _advanceDay = true;

    public Travel(IGame game)
    {
        _game = game;

        string statusesFilePath = "data/Statuses.yaml"; 
        Statuses = [.. Utility.LoadYaml<StatusData>(statusesFilePath)];        
        
        string occurrencesFilePath = "data/Occurrences.yaml";
        _occurrenceEngine = new OccurrenceEngine(occurrencesFilePath, _game.Party, Statuses);

        string pacesFilePath = "data/Paces.yaml"; 
        _paceData = Utility.LoadYaml<PaceData>(pacesFilePath);
        _pace = _paceData.MinBy(pace => pace.Factor);

        string rationsFilePath = "data/Rations.yaml";
        _rationData = Utility.LoadYaml<RationData>(rationsFilePath);
        _rations = _rationData.MaxBy(rations => rations.Factor);

    }
        public void TravelLoop()
        {
            var todaysMiles = CalculateMilesTraveled();
            if (todaysMiles > _game.MilesToNextLandmark)
            {
                todaysMiles = _game.MilesToNextLandmark;
            }
            _game.MilesTraveled += todaysMiles;
            _game.MilesToNextLandmark -= todaysMiles;

            string occurrenceMessage = "";
            if (_game.MilesToNextLandmark <= 0)
            {
                occurrenceMessage = $"You have reached {_game.NextLandmark.Name}.";
                _game.ChangeMode(Modes.AtLandmark);

            }
            else
            {
                Occurrence randomOccurrence = _occurrenceEngine.PickRandomOccurrence();
                var occurrence = _occurrenceEngine.ProcessOccurrence(randomOccurrence);
                occurrenceMessage = occurrence.DisplayText;
            }

            _game.Party.SpendDailyHealth(_pace, _rations);

            _game.DrawStatusPanel();

            _game._display.Write($"{_game.CurrentDate.GetFormatted()}: {occurrenceMessage}");
            _game._display.ScrollToBottom();

            if (_advanceDay)
            {
                _game.CurrentDate = _game.CurrentDate.AddDays(1);
            }
        }
        private double CalculateMilesTraveled()
        {
            // TODO: factor in oxen like ( _pace.Factor / (Inventory.currentOxen / Inventory.maximumOxen ))
            return _pace.Factor;
        }

        public void ContinueTravelling()
        {
            _game._display.Write($"You decided to continue.");
            _game.NextLandmark = _game._landmarksData.Landmarks.NextOrFirst(landmark => landmark.ID == _game.NextLandmark.ID);
            _game.MilesToNextLandmark = _game.NextLandmark.Distance;
            _game.ChangeMode(Modes.Travelling);
        }
}
