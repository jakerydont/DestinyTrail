using System;
using Xunit;
using Moq;
using DestinyTrail.Engine;
using System.Threading.Tasks;
using DestinyTrail.Engine.Abstractions;

namespace DestinyTrail.Engine.Tests
{
    public class GameTests
    {

        private readonly Mock<IDisplay> _mockDisplay;
        private readonly Mock<IDisplay> _mockStatus;
        private readonly Mock<IUtility> _mockUtility;
        private readonly Mock<IWorldStatus> _mockWorldStatus;

        private readonly Mock<IInputHandler> _mockInputHandler;
        private Mock<ITravel> _mockTravel;
        private Mock<IWagonParty> _mockWagonParty;
        private readonly Game _game;

        private readonly List<Occurrence> occurrences = new List<Occurrence>
        {
            new() { Name = "Occurrence 1", DisplayText = "Occurrence 1", Probability = 0.0, Effect = "no effect" },
            new() { Name = "Occurrence 2", DisplayText = "Occurrence 2", Probability = 0.0, Effect = "no effect" },
            new() { Name = "Occurrence 3", DisplayText = "Occurrence 3", Probability = 1.0, Effect = "no effect" }
        };
        public GameTests()
        {
            _mockDisplay = new Mock<IDisplay>();
            _mockStatus = new Mock<IDisplay>();
            _mockUtility = new Mock<IUtility>();

            _mockTravel = new Mock<ITravel>();
            _mockTravel.Setup(t => t.TravelLoop()).Returns(Task.CompletedTask);

            _mockWorldStatus = new Mock<IWorldStatus>();
            _mockInputHandler = new Mock<IInputHandler>();

            _mockWagonParty = new Mock<IWagonParty>();

            _mockWagonParty.Setup(p => p.GetRandomMember()).Returns(new Person { ID = 0, Name = "Greg", Status = new Status { Name = "Healthy" } });
            _mockWagonParty.Setup(p => p.Members).Returns(new List<IPerson> { new Person { ID = 0, Name = "Greg", Status = new Status { Name = "Healthy" } } });

            // Mocking Utility.LoadYaml method to return mock data
            _mockUtility.Setup(u => u.LoadYamlAsync<OccurrenceData>(It.IsAny<string>())).ReturnsAsync(new OccurrenceData { Occurrences = occurrences });
            _mockUtility.Setup(u => u.LoadYamlAsync<RandomNamesData>(It.IsAny<string>())).ReturnsAsync(new RandomNamesData { RandomNames = new List<PersonName> { new() { Name = "Name One" }, new() { Name = "Name Two" }, new() { Name = "Name Three" } } });
            _mockUtility.Setup(u => u.LoadYamlAsync<LandmarksData>(It.IsAny<string>())).ReturnsAsync(new LandmarksData { Landmarks = new List<Landmark> { new Landmark { Name = "Landmark 1", ID = "FIRST", Lore = "The first landmark" } } });
            _mockUtility.Setup(u => u.LoadYamlAsync<Inventory>(It.IsAny<string>())).ReturnsAsync(new Inventory());
            _mockUtility.Setup(u => u.LoadYamlAsync<StatusData>(It.IsAny<string>())).ReturnsAsync(new StatusData {
                Statuses=[
                    new() { Name = "Healthy" }, 
                    new() { Name = "Sick" }, 
                    new() { Name = "Injured" }
                ]});


            var mockOccurrenceEngine = new Mock<IOccurrenceEngine>();
            // Initialize the Game with the mocked dependencies
            _game = Task.Run(() =>Game.CreateAsync(
                _mockDisplay.Object,
                _mockStatus.Object,
                _mockUtility.Object, 
                _mockWagonParty.Object,
                _mockTravel.Object,
                _mockWorldStatus.Object,
                _mockInputHandler.Object

            )).Result;

        }

        [Fact]
        public void Game_InitializesCorrectly()
        {


            Assert.Equal(Modes.Travelling, _game.GameMode);
        }

        [Fact]
        public void ChangeMode_ChangesGameMode()
        {
            // Arrange: Game starts in Travelling mode
            var initialMode = _game.GameMode;

            // Act: Change mode to AtLandmark
            _game.ChangeMode(Modes.AtLandmark);

            // Assert: The mode should be changed to AtLandmark
            Assert.Equal(Modes.AtLandmark, _game.GameMode);

            // Act: Change mode to Shopping
            _game.ChangeMode(Modes.Shopping);

            // Assert: The mode should be changed to Shopping
            Assert.Equal(Modes.Shopping, _game.GameMode);

            _game.ChangeMode(Modes.GameOver); 
            Assert.Equal(Modes.GameOver, _game.GameMode);
        }

        [Fact]
        public async Task StartGameLoop_ShouldExitWhenCancelled()
        {
            _mockWagonParty.Setup(p => p.IsAnybodyAlive()).Returns(true);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000); 


            // Act
            var task = _game.StartGameLoop(cts.Token);
            await task;

            // Assert
            _mockWagonParty.Verify(p => p.IsAnybodyAlive(), Times.AtLeastOnce);

        }



        [Fact]
        public async Task  StartGameLoop_ShouldSwitchModeToGameOver_WhenAllMembersDead() {

            _mockWagonParty.Setup(p => p.IsAnybodyAlive()).Returns(false);
            _game._party = _mockWagonParty.Object;


            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000); 

            // Act
            await _game.StartGameLoop(cts.Token);

            Assert.Equal(Modes.GameOver, _game.GameMode);
        }

        [Fact]
        public void DrawStatusPanel_UpdatesStatusDisplay()
        {
            // Act: Call DrawStatusPanel
            _game.DrawStatusPanel();

            // Assert: Verify that the status display writes the correct information
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Date:"))), Times.Once);
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Weather:"))), Times.Once);
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Health:"))), Times.Once);
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Distance to next landmark:"))), Times.Once);
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Distance traveled:"))), Times.Once);
        }



    }
}
