using Xunit.Sdk;

namespace DestinyTrail.Engine.Tests
{
    public class TravelTests
    {
        private readonly Mock<IUtility> _mockUtility;
        private readonly Mock<IGame> _mockGame;
        private readonly Travel _travel;

        public TravelTests()
        {
            _mockUtility = new Mock<IUtility>();
            _mockGame = new Mock<IGame>();

            // Mocking game properties


            _mockGame.Setup(g => g.CurrentDate).Returns(DateTime.Now);


            var wagonParty = new Mock<IWagonParty>();
            wagonParty.Setup(w => w.GetRandomMember()).Returns(new Person { ID = 0, Name = "Greg", Status = new Status { Name = "Healthy" } });

            _mockGame.Setup(g => g._party).Returns(wagonParty.Object);

            // Mocking YAML loading
            _mockUtility.Setup(u => u.GetAppSetting(It.IsAny<string>())).Returns(string.Empty);
            _mockUtility.Setup(u => u.LoadYaml<StatusData>(It.IsAny<string>()))
                .Returns(new StatusData { Statuses = new() {new() { Name ="Healthy"},new(){  Name ="Sick"} } });

            _mockUtility.Setup(u => u.LoadYaml<PaceData>(It.IsAny<string>()))
                .Returns(new PaceData { Paces = [new Pace { Name = "Slow", Factor = 10 }, new Pace { Name = "Fast", Factor = 20 } ]});

            _mockUtility.Setup(u => u.LoadYaml<RationData>(It.IsAny<string>()))
                .Returns(new RationData { Rations = [new Rations { Name = "Meager", Factor = 1 }, new Rations { Name = "Full", Factor = 2 } ]});

            _mockUtility.Setup(u => u.LoadYaml<OccurrenceData>(It.IsAny<string>()))
                .Returns(new OccurrenceData { Occurrences = [
                    new Occurrence { Name = "ATE_IT", DisplayText = "{name} ate it", Effect = "none" }, 
                    new Occurrence { Name = "Occurrence 2", DisplayText = "{name} is an occurrence", Effect = "dead" } ]});

            _mockUtility.Setup(u => u.LoadYaml<LandmarksData>(It.IsAny<string>()))
                .Returns(new LandmarksData { Landmarks = [new Landmark { ID = "FORT_LARAMIE", Name = "Fort Laramie", Distance = 150, Lore = "Fun place" } ]});

            // Creating the Travel object with mocked dependencies
            _travel = new Travel(_mockUtility.Object);


            _travel.MilesTraveled =0;
        }

        [Fact]
        public void Constructor_InitializesDependencies()
        {
            // Assert
            _mockUtility.Verify(u => u.LoadYaml<PaceData>(It.IsAny<string>()), Times.Once);
            _mockUtility.Verify(u => u.LoadYaml<RationData>(It.IsAny<string>()), Times.Once);


            Assert.NotNull(_travel.Pace);
            Assert.NotNull(_travel.Rations);
            Assert.NotNull(_travel.OccurrenceEngine);

            Assert.Equal(0, _travel.MilesTraveled);
            Assert.Equal(150, _travel.MilesToNextLandmark);
        }

        [Fact]
        public void TravelLoop_UpdatesMilesTraveledAndStatus_WhenTraveling()
         {
            // Arrange
            var mockDisplay = new Mock<IDisplay>();
            _mockGame.Setup(g => g._display).Returns(mockDisplay.Object);
            _travel.MilesToNextLandmark = 50;

            // Act
            _travel.TravelLoop();

            // Assert
            
            Assert.Equal(50 - _travel.Pace.Factor, _travel.MilesToNextLandmark);

            Assert.Equal(_travel.MilesTraveled, _travel.Pace.Factor);

        }

        [Fact]
        public void TravelLoop_ChangesModeToAtLandmark_WhenMilesToNextLandmarkIsZero()
        {
            // Arrange
            var mockDisplay = new Mock<IDisplay>();
            _mockGame.Setup(g => g._display).Returns(mockDisplay.Object);
            
            // Set up the game state
                        _travel.MilesToNextLandmark = 0;
            _travel.NextLandmark.Name = "Fort Kearney";

            // Act
            _travel.TravelLoop();

            // Assert
            // Verify that the game mode changes to AtLandmark
            _mockGame.Verify(g => g.ChangeMode(Modes.AtLandmark), Times.Once);

            // Verify that the display includes the correct landmark message
            mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Fort Kearney"))), Times.Once);
        }


        [Fact]
        public void TravelLoop_ProcessesOccurrence_WhenStillTraveling()
        {
            // Arrange
            var mockDisplay = new Mock<IDisplay>();
            _mockGame.Setup(g => g._display).Returns(mockDisplay.Object);
           _travel.MilesToNextLandmark = 50; // Still traveling

            // Mocking a simplified OccurrenceEngine interaction
            var mockOccurrenceEngine = new Mock<IOccurrenceEngine>();
            var mockOccurrence = new Occurrence 
            { 
                Name = "WILDANIMAL", 
                DisplayText = "Wild Animal Encounter", 
                Effect = "dead" 
            };

            mockOccurrenceEngine
                .Setup(o => o.PickRandomOccurrence())
                .Returns(mockOccurrence);
            mockOccurrenceEngine
                .Setup(o => o.ProcessOccurrence(mockOccurrence))
                .Returns(mockOccurrence);

            _travel.OccurrenceEngine = mockOccurrenceEngine.Object;

            // Act
            _travel.TravelLoop();

            // Assert
            mockOccurrenceEngine.Verify(o => o.PickRandomOccurrence(), Times.Once);
            mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Wild Animal Encounter"))), Times.Once);
        }


        [Fact]
        public void ContinueTravelling_ResetsMilesAndChangesMode()
        {
            // Arrange
            var mockDisplay = new Mock<IDisplay>();
            _mockGame.Setup(g => g._display).Returns(mockDisplay.Object);

            var nextLandmark = new Landmark { ID = "FORT_LARAMIE" , Name = "Fort Laramie", Distance = 150, Lore="Fun place" };
           _travel.NextLandmark = nextLandmark;

            _travel._landmarksData = new LandmarksData{Landmarks = new List<Landmark> { nextLandmark }};

            _mockUtility.Setup(u => u.NextOrFirst(It.IsAny<IEnumerable<Landmark>>(), It.IsAny<Func<Landmark, bool>>()))
                        .Returns(nextLandmark);

            // Act
            _travel.ContinueTravelling();

            // Assert
            Assert.Equal(_travel.MilesToNextLandmark, nextLandmark.Distance);
            _mockGame.Verify(g => g.ChangeMode(Modes.Travelling), Times.Once);
            mockDisplay.Verify(d => d.Write("You decided to continue."));
        }
    }
}
