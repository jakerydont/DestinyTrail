using System;
using System.Reflection.Metadata.Ecma335;
using Moq;
using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class TravelTests
    {
        private readonly Mock<IUtility> _mockUtility;
        private Mock<IDisplay> _mockDisplay;
        private readonly Mock<IGame> _mockGame;


        private  Travel _travel;
        private Landmark testLandmark = new Landmark { ID = "TESTLANDMARK", Name = "Test Landmark", Distance = 100, Lore = "Test Lore" };
        private Landmark secondTestLandmark = new Landmark { ID = "SECONDTESTLANDMARK", Name = "Second Test Landmark", Distance = 80, Lore = "Second Test Lore" };

        public TravelTests()
        {
            // Mocking IGame
            _mockGame = new Mock<IGame>();

            // Mocking IUtility
            _mockUtility = new Mock<IUtility>();

            // Setting up mock data to return from LoadYaml
            _mockUtility.Setup(u => u.LoadYaml<StatusData>("data/Statuses.yaml"))
                .Returns(new StatusData { Statuses = ["good", "less good", "bad", "dead"] });

            _mockUtility.Setup(u => u.LoadYaml<PaceData>("data/Paces.yaml"))
                .Returns(new PaceData { Paces = [
                    new (){Name = "slowest", Factor = 8},
                    new (){Name = "middle", Factor = 10},
                    new (){Name = "fastest", Factor = 12}
                ]});

            _mockUtility.Setup(u => u.LoadYaml<RationData>("data/Rations.yaml"))
                .Returns( new RationData { Rations = [
                    new() { Name= "best", Factor = 100 },
                    new () { Name= "middle", Factor = 70 },
                    new () { Name= "worst", Factor = 50 }
                ]});



            _mockUtility = new Mock<IUtility>();
            // Initialize a mock Game object with required properties
            _mockDisplay = new Mock<IDisplay>();
            _mockGame = new Mock<IGame>();


            _mockGame.Object.Party = new WagonParty(["Alice", "Bob"]);

            _mockGame.Object.CurrentDate = DateTime.Now;
            _mockGame.Object.MilesToNextLandmark = 100;
            _mockGame.Object.NextLandmark = testLandmark;
            _mockGame.Object.MilesTraveled = 0;
            _mockGame.Object._display = _mockDisplay.Object;
            _mockGame.Object._landmarksData = new LandmarksData { Landmarks = new List<Landmark> { testLandmark, secondTestLandmark } };
           
           
            _travel = new Travel(_mockGame.Object, _mockUtility.Object);

        }

        [Fact]
        public void Constructor_ShouldLoadStatusesAndInitializeComponents()
        {

            // Creating the Travel object with mocked dependencies
            _travel = new Travel(_mockGame.Object, _mockUtility.Object);

            // Assert
            Assert.NotNull(_travel.Statuses);
            Assert.NotEmpty(_travel.Statuses);
            Assert.NotNull(_travel._occurrenceEngine);
            Assert.NotNull(_travel._pace);
            Assert.NotNull(_travel._rations);
        }

        [Fact]
        public void TravelLoop_ShouldUpdateMilesTraveled()
        {
            // Creating the Travel object with mocked dependencies
            _travel = new Travel(_mockGame.Object, _mockUtility.Object);


            _mockGame.Object.MilesToNextLandmark = 50;

            // Act
            _travel.TravelLoop();

            // Assert
            Assert.NotEqual(50, _mockGame.Object.MilesTraveled);
            Assert.InRange(_mockGame.Object.MilesToNextLandmark, 0, 50);
        }

        [Fact]
        public void TravelLoop_ShouldProcessOccurrences()
        {
            // Arrange        
            _travel = new Travel(_mockGame.Object, _mockUtility.Object);
            _mockGame.Object.MilesToNextLandmark = 150; // Set a distance greater than the default pace

            // Act
            _travel.TravelLoop();

            // Assert
            Assert.True(_mockGame.Object.MilesTraveled > 0); // Miles should have been traveled
            Assert.True(_mockGame.Object.MilesToNextLandmark < 150); // Check if it decreased correctly
        }

        [Fact]
        public void ContinueTravelling_ShouldUpdateGameState()
        {
            // Arrange
            // Creating the Travel object with mocked dependencies
            _travel = new Travel(_mockGame.Object, _mockUtility.Object);

            var previousLandmark = _mockGame.Object.NextLandmark;
            _mockGame.Object.ChangeMode(Modes.AtLandmark);

            // Act
            _travel.ContinueTravelling();

            // Assert
            Assert.Equal("You decided to continue.", _mockGame.Object._display.Items[^1]);
            Assert.NotEqual(previousLandmark, _mockGame.Object.NextLandmark);
            Assert.Equal(_mockGame.Object.NextLandmark.Distance, _mockGame.Object.MilesToNextLandmark);
            Assert.Equal(Modes.Travelling, _mockGame.Object.GameMode);
        }
    }
}
