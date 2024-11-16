using System;
using Moq;
using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class TravelTests
    {
        private readonly Mock<IUtility> _mockUtility;
        private Mock<IDisplay> _mockDisplay;
        private readonly Mock<IGame> _mockGame;

        private Landmark testLandmark = new Landmark { ID = "TESTLANDMARK", Name = "Test Landmark", Distance = 100, Lore = "Test Lore" };
        private Landmark secondTestLandmark = new Landmark { ID = "SECONDTESTLANDMARK", Name = "Second Test Landmark", Distance = 80, Lore = "Second Test Lore" };
        public TravelTests()
        {
            _mockUtility = new Mock<IUtility>();
            // Initialize a mock Game object with required properties
            _mockDisplay = new Mock<IDisplay>();
            _mockGame = new Mock<IGame>();

          
            _mockGame.Object.Party = new WagonParty(new string[] { "Alice", "Bob" });

            _mockGame.Object.CurrentDate = DateTime.Now;
            _mockGame.Object.MilesToNextLandmark = 100;
            _mockGame.Object.NextLandmark = testLandmark;
            _mockGame.Object.MilesTraveled = 0;
            _mockGame.Object._display = _mockDisplay.Object;
            _mockGame.Object._landmarksData = new LandmarksData { Landmarks = new List<Landmark> { testLandmark, secondTestLandmark } };
          
        }

        [Fact]
        public void Constructor_ShouldLoadStatusesAndInitializeComponents()
        {
                        
            // Act
            var travel = new Travel(_mockGame.Object, _mockUtility.Object);

            // Assert
            Assert.NotNull(travel.Statuses);
            Assert.NotEmpty(travel.Statuses);
            Assert.NotNull(travel._occurrenceEngine);
            Assert.NotNull(travel._pace);
            Assert.NotNull(travel._rations);
        }

        [Fact]
        public void TravelLoop_ShouldUpdateMilesTraveled()
        {
            // Arrange
            Travel? travel = new Travel(_mockGame.Object);
            _mockGame.Object.MilesToNextLandmark = 50;

            // Act
            travel.TravelLoop();

            // Assert
            Assert.NotEqual(50, _mockGame.Object.MilesTraveled);
            Assert.InRange(_mockGame.Object.MilesToNextLandmark, 0, 50);
        }

        [Fact]
        public void TravelLoop_ShouldProcessOccurrences()
        {
            // Arrange
            var travel = new Travel(_mockGame.Object);
            _mockGame.Object.MilesToNextLandmark = 150; // Set a distance greater than the default pace

            // Act
            travel.TravelLoop();

            // Assert
            Assert.True(_mockGame.Object.MilesTraveled > 0); // Miles should have been traveled
            Assert.True(_mockGame.Object.MilesToNextLandmark < 150); // Check if it decreased correctly
        }

        [Fact]
        public void ContinueTravelling_ShouldUpdateGameState()
        {
            // Arrange
            var travel = new Travel(_mockGame.Object);
            var previousLandmark = _mockGame.Object.NextLandmark;
            _mockGame.Object.ChangeMode( Modes.AtLandmark);

            // Act
            travel.ContinueTravelling();

            // Assert
            Assert.Equal("You decided to continue.", _mockGame.Object._display.Items[^1]);
            Assert.NotEqual(previousLandmark, _mockGame.Object.NextLandmark);
            Assert.Equal(_mockGame.Object.NextLandmark.Distance, _mockGame.Object.MilesToNextLandmark);
            Assert.Equal(Modes.Travelling, _mockGame.Object.GameMode);
        }
    }
}
