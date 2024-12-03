
using System.Dynamic;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using YamlDotNet.Serialization;
using System.Configuration;

namespace DestinyTrail.Engine
{
    public class Game : IGame
    {
        private CancellationTokenSource _cancellationTokenSource { get; set; }

        public DateTime CurrentDate { get; set; }
        public ITravel travel {get;set;}


        private string _weather = "not implemented";

        public IInventory Inventory { get; set; }
        public InputHandler InputHandler { get; private set; }

        public IDisplay _display { get; set; }

        protected IDisplay _status { get; set; }

        private IUtility _utility { get; set; }

        public IWagonParty _party { get; set; }
        public Modes GameMode { get; private set; }
        public IShoppingEngine ShoppingEngine { get; set; }
        private bool _shouldInitializeAtLandmark { get; set; }

        public Game(
            IDisplay Output, 
            IDisplay Status, 
            IUtility Utility, 
            IWagonParty Party,
            ITravel Travel)
        {
            InputHandler = new InputHandler(this);
            _display = Output;
            _status = Status;
            _utility = Utility;
            travel = Travel;
            _party = Party;

            _cancellationTokenSource = new CancellationTokenSource();


            _display.Write(_party.GetDisplayNames());

            string inventoryFilePath = Utility.GetAppSetting("InventoryFilePath");
            string inventoryCustomItemsFilePath = Utility.GetAppSetting("InventoryCustomItemsFilePath");
            Inventory = Utility.LoadYaml<Inventory>(inventoryFilePath);
            Inventory.CustomItems = Utility.LoadYaml<Inventory>(inventoryCustomItemsFilePath);
            CurrentDate = new DateTime(1860, 10, 1);



            GameMode = Modes.Travelling;
            ShoppingEngine = new ShoppingEngine(_display, Inventory);

        }
        public async Task StartGameLoop()
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    switch (GameMode)
                    {
                        case Modes.Travelling:
                            travel.TravelLoop();
                            break;
                        case Modes.AtLandmark:
                            AtLandmarkLoop();
                            break;
                        case Modes.Shopping:
                            ShoppingEngine.ShoppingLoop();
                            break;
                        default:
                            break;
                    }
                    await Task.Delay(1000, token);
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
            _display.WriteTitle(travel.NextLandmark.Name);
            _display.Write("Press enter to continue.");

        }


        public void DrawStatusPanel()
        {
            _status.Clear();
            _status.Write($"Date: {_utility.GetFormatted(CurrentDate)}");
            _status.Write($"Weather: {_weather}");
            _status.Write($"Health: {_party.GetDisplayHealth()}");
            _status.Write($"Distance to next landmark: {_utility.Abbreviate(travel.MilesToNextLandmark)} miles ({_utility.Abbreviate(travel.MilesToNextLandmark)} km)");
            _status.Write($"Distance traveled: {_utility.Abbreviate(travel.MilesTraveled)} miles ({_utility.Abbreviate(travel.MilesTraveled)} km)");

            _status.Write($"-----------");
            foreach (var person in _party.Members)
            {
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
            else if (GameMode == Modes.Shopping)
            {
                ShoppingEngine.InitializeState();
            }
        }

    }
}