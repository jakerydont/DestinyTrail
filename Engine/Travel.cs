using System;

namespace DestinyTrail.Engine;

public class Travel : ITravel
{
    IGame _game { get; set; }

    private IUtility Utility;

    public IOccurrenceEngine OccurrenceEngine { get; set; }

    private PaceData _paceData;
    public Pace Pace { get; set; }

    public Rations Rations {get;set;}
    private RationData _rationData {get;set;}
    
    private bool _advanceDay = true;

    public Travel(IGame game) : this(game, new Utility()){}

    public Travel(IGame game, IUtility utility)
    {
        _game = game;
        Utility = utility;


        string occurrencesFilePath = "data/Occurrences.yaml";
        OccurrenceEngine = new OccurrenceEngine(occurrencesFilePath, _game.Party, Utility);

        string pacesFilePath = "data/Paces.yaml"; 
        _paceData = Utility.LoadYaml<PaceData>(pacesFilePath);
        Pace = _paceData.MinBy(pace => pace.Factor);

        string rationsFilePath = "data/Rations.yaml";
        _rationData = Utility.LoadYaml<RationData>(rationsFilePath);
        Rations = _rationData.MaxBy(rations => rations.Factor);

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
                Occurrence randomOccurrence = OccurrenceEngine.PickRandomOccurrence();
                var occurrence = OccurrenceEngine.ProcessOccurrence(randomOccurrence);
                occurrenceMessage = occurrence.DisplayText;
            }

            _game.Party.SpendDailyHealth(Pace, Rations);

            _game.DrawStatusPanel();

            _game._display.Write($"{Utility.GetFormatted(_game.CurrentDate)}: {occurrenceMessage}");
            _game._display.ScrollToBottom();

            if (_advanceDay)
            {
                _game.CurrentDate = _game.CurrentDate.AddDays(1);
            }
        }
        private double CalculateMilesTraveled()
        {
            // TODO: factor in oxen like ( _pace.Factor / (Inventory.currentOxen / Inventory.maximumOxen ))
            return Pace.Factor;
        }

        public void ContinueTravelling()
        {
            _game._display.Write($"You decided to continue.");
            _game.NextLandmark = Utility.NextOrFirst(_game._landmarksData.Landmarks, landmark => landmark.ID == _game.NextLandmark.ID);
            _game.MilesToNextLandmark = _game.NextLandmark.Distance;
            _game.ChangeMode(Modes.Travelling);
        }
}
