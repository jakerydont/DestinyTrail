using System;
using Moq;
using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class TravelTests
    {
        private readonly Mock<IUtility> _mockUtility;
        private readonly MockGame _mockGame;

        private Landmark testLandmark = new Landmark { ID = "TESTLANDMARK", Name = "Test Landmark", Distance = 100, Lore = "Test Lore" };
        private Landmark secondTestLandmark = new Landmark { ID = "SECONDTESTLANDMARK", Name = "Second Test Landmark", Distance = 80, Lore = "Second Test Lore" };
        public TravelTests()
        {
            _mockUtility = new Mock<IUtility>();
            // Initialize a mock Game object with required properties
            _mockGame = new MockGame
            {
                Party = new WagonParty(new string[] { "Alice", "Bob" }),
                CurrentDate = DateTime.Now,
                MilesToNextLandmark = 100,
                NextLandmark = testLandmark,
                MilesTraveled = 0,
                _display = new MockDisplay(),
                _landmarksData = new LandmarksData { Landmarks = new List<Landmark> { testLandmark, secondTestLandmark } }
            };
        }

        [Fact]
        public void Constructor_ShouldLoadStatusesAndInitializeComponents()
        {
                        
            // Act
            var travel = new Travel(_mockGame, _mockUtility.Object);

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
            var travel = new Travel(_mockGame);
            _mockGame.MilesToNextLandmark = 50;

            // Act
            travel.TravelLoop();

            // Assert
            Assert.NotEqual(50, _mockGame.MilesTraveled);
            Assert.InRange(_mockGame.MilesToNextLandmark, 0, 50);
        }

        [Fact]
        public void TravelLoop_ShouldProcessOccurrences()
        {
            // Arrange
            var travel = new Travel(_mockGame);
            _mockGame.MilesToNextLandmark = 150; // Set a distance greater than the default pace

            // Act
            travel.TravelLoop();

            // Assert
            Assert.True(_mockGame.MilesTraveled > 0); // Miles should have been traveled
            Assert.True(_mockGame.MilesToNextLandmark < 150); // Check if it decreased correctly
        }

        [Fact]
        public void ContinueTravelling_ShouldUpdateGameState()
        {
            // Arrange
            var travel = new Travel(_mockGame);
            var previousLandmark = _mockGame.NextLandmark;
            _mockGame.GameMode = Modes.AtLandmark;

            // Act
            travel.ContinueTravelling();

            // Assert
            Assert.Equal("You decided to continue.", ((MockDisplay)_mockGame._display).Items[^1]);
            Assert.NotEqual(previousLandmark, _mockGame.NextLandmark);
            Assert.Equal(_mockGame.NextLandmark.Distance, _mockGame.MilesToNextLandmark);
            Assert.Equal(Modes.Travelling, _mockGame.GameMode);
        }
    }

    // Mock classes for testing purposes
    public class MockDisplay : IDisplay
    {
        public List<string> Items { get; set; } = new List<string>();
        public void Write(string message) => Items.Add(message);
        public void ScrollToBottom() { }
        public void Clear() { }
    }
    public class MockGame : IGame
    {
        public required WagonParty Party { get; set; }
        public DateTime CurrentDate { get; set; }
        public double MilesToNextLandmark { get; set; }
        public required Landmark NextLandmark { get; set; }
        public double MilesTraveled { get; set; }
        public required IDisplay _display { get; set; }
        public Modes GameMode { get; set; }
        public required LandmarksData _landmarksData { get; set; }

        public void ChangeMode(Modes gameMode)
        {
            GameMode = gameMode;
        }

        public void DrawStatusPanel()
        {

        }
    }

}
