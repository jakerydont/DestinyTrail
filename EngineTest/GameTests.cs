using System;
using Xunit;
using Moq;
using DestinyTrail.Engine;
using System.Threading.Tasks;

namespace DestinyTrail.Engine.Tests
{
    public class GameTests
    {

        private readonly Mock<IDisplay> _mockDisplay;
        private readonly Mock<IDisplay> _mockStatus;
        private readonly Mock<IUtility> _mockUtility;
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

            // Mocking Utility.LoadYaml method to return mock data
            _mockUtility.Setup(u => u.LoadYaml<OccurrenceData>("data/Occurrences.yaml")).Returns(new OccurrenceData { Occurrences = occurrences });
            _mockUtility.Setup(u => u.LoadYaml<RandomNamesData>("data/RandomNames.yaml")).Returns(new RandomNamesData { RandomNames = new List<PersonName> { new() { Name = "Name One" }, new() { Name = "Name Two" }, new() { Name = "Name Three" } } });
            _mockUtility.Setup(u => u.LoadYaml<LandmarksData>("data/Landmarks.yaml")).Returns(new LandmarksData { Landmarks = new List<Landmark> { new Landmark { Name = "Landmark 1", ID = "FIRST", Lore = "The first landmark" } } });
            _mockUtility.Setup(u => u.LoadYaml<Inventory>("data/Inventory.yaml")).Returns(new Inventory());
            _mockUtility.Setup(u => u.LoadYaml<StatusData>("data/Statuses.yaml")).Returns(new StatusData {
                Statuses=[
                    new() { Name = "Healthy" }, 
                    new() { Name = "Sick" }, 
                    new() { Name = "Injured" }
                ]});
            // Initialize the Game with the mocked dependencies
            _game = new Game(_mockDisplay.Object, _mockStatus.Object, _mockUtility.Object);
        }

        [Fact]
        public void Game_InitializesCorrectly()
        {
            // Verify that key properties are initialized correctly
            Assert.NotNull(_game.RandomNames);
            Assert.Equal(0, _game.MilesTraveled);
            Assert.Equal(0, _game.MilesToNextLandmark);
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
