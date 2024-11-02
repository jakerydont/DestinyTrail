using System;
using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class TravelTests
    {
        private readonly MockGame _mockGame;

        private Landmark testLandmark = new Landmark { ID = "TESTLANDMARK", Name = "Test Landmark", Distance = 100, Lore = "Test Lore" };

        public TravelTests()
        {
            // Initialize a mock Game object with required properties
            _mockGame = new MockGame
            {
                Party = new WagonParty(new string[] { "Alice", "Bob" }),
                CurrentDate = DateTime.Now,
                MilesToNextLandmark = 100,
                NextLandmark = testLandmark,
                MilesTraveled = 0,
                _display = new MockDisplay(),
                _landmarksData = new LandmarksData{ Landmarks = new List<Landmark>{testLandmark}}
            };
        }

        [Fact]
        public void Constructor_ShouldLoadStatusesAndInitializeComponents()
        {
            // Act
            var travel = new Travel(_mockGame);

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
            Assert.Equal("You decided to continue.", _mockGame._display.Items[^1]); // Check display message
            Assert.NotEqual(previousLandmark, _mockGame.NextLandmark); // Ensure the landmark changed
            Assert.Equal(_mockGame.NextLandmark.Distance, _mockGame.MilesToNextLandmark); // Check the distance to next landmark
            Assert.Equal(Modes.Travelling, _mockGame.GameMode); // Check if the game mode changed
        }


    }

    // Mock classes for testing purposes
    public class MockDisplay : Display
    {
        public new dynamic Items { get; set; } = new List<string>();

        public new void Write(string message) => Items.Add(message);
        public new void ScrollToBottom() { }
    }
    public class MockGame : IGame
    {
        public required WagonParty Party { get; set; }
        public DateTime CurrentDate { get; set; }
        public double MilesToNextLandmark { get; set; }
        public required Landmark NextLandmark { get; set; }
        public double MilesTraveled { get; set; }
        public required Display _display { get; set; }
        public Modes GameMode { get; set; }
        public required LandmarksData _landmarksData { get; set; }

        public void ChangeMode(Modes atLandmark)
        {
            
        }

        public void DrawStatusPanel()
        {
            
        }
    }

}