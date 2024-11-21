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
            _mockGame.Setup(g => g.MilesToNextLandmark).Returns(100);
            _mockGame.Setup(g => g.MilesTraveled).Returns(0);
            _mockGame.Setup(g => g.CurrentDate).Returns(DateTime.Now);


            var wagonParty = new Mock<IWagonParty>();
            wagonParty.Setup(w => w.GetRandomMember()).Returns(new Person { ID = 0, Name = "Greg", Status = new Status { Name = "Healthy" } });

            _mockGame.Setup(g => g.Party).Returns(wagonParty.Object);

            // Mocking YAML loading
            _mockUtility.Setup(u => u.LoadYaml<StatusData>("data/Statuses.yaml"))
                .Returns(new StatusData { Statuses = new() { "Healthy", "Sick" } });
            _mockUtility.Setup(u => u.LoadYaml<PaceData>("data/Paces.yaml"))
                .Returns(new PaceData { Paces = [new Pace { Name = "Slow", Factor = 10 }, new Pace { Name = "Fast", Factor = 20 } ]});
            _mockUtility.Setup(u => u.LoadYaml<RationData>("data/Rations.yaml"))
                .Returns(new RationData { Rations = [new Rations { Name = "Meager", Factor = 1 }, new Rations { Name = "Full", Factor = 2 } ]});
            _mockUtility.Setup(u => u.LoadYaml<OccurrenceData>("data/Occurrences.yaml"))
                .Returns(new OccurrenceData { Occurrences = [
                    new Occurrence { Name = "ATE_IT", DisplayText = "{name} ate it", Effect = "none" }, 
                    new Occurrence { Name = "Occurrence 2", DisplayText = "{name} is an occurrence", Effect = "dead" } ]});

            // Creating the Travel object with mocked dependencies
            _travel = new Travel(_mockGame.Object, _mockUtility.Object);
        }

        [Fact]
        public void Constructor_InitializesDependencies()
        {
            // Assert
            _mockUtility.Verify(u => u.LoadYaml<StatusData>("data/Statuses.yaml"), Times.Once);
            _mockUtility.Verify(u => u.LoadYaml<PaceData>("data/Paces.yaml"), Times.Once);
            _mockUtility.Verify(u => u.LoadYaml<RationData>("data/Rations.yaml"), Times.Once);

            Assert.NotNull(_travel.Statuses);
            Assert.NotNull(_travel.Pace);
            Assert.NotNull(_travel.Rations);
            Assert.NotNull(_travel.OccurrenceEngine);
        }

        [Fact]
        public void TravelLoop_UpdatesMilesTraveledAndStatus_WhenTraveling()
         {
            // Arrange
            var mockDisplay = new Mock<IDisplay>();
            _mockGame.Setup(g => g._display).Returns(mockDisplay.Object);
            _mockGame.Setup(g => g.MilesToNextLandmark).Returns(50);

            // Act
            _travel.TravelLoop();

            // Assert
            _mockGame.VerifySet(g => g.MilesToNextLandmark = 50 - _travel.Pace.Factor);
            _mockGame.VerifySet(g => g.MilesTraveled = _travel.Pace.Factor);

        }

        [Fact]
        public void TravelLoop_ChangesModeToAtLandmark_WhenMilesToNextLandmarkIsZero()
        {
            // Arrange
            var mockDisplay = new Mock<IDisplay>();
            _mockGame.Setup(g => g._display).Returns(mockDisplay.Object);
            
            // Set up the game state
            _mockGame.Setup(g => g.MilesToNextLandmark).Returns(0); // Simulate reaching the landmark
            _mockGame.Setup(g => g.NextLandmark.Name).Returns("Fort Kearney");

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
            _mockGame.Setup(g => g.MilesToNextLandmark).Returns(50); // Still traveling

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
            _mockGame.Setup(g => g.NextLandmark).Returns(nextLandmark);

            _mockUtility.Setup(u => u.NextOrFirst(It.IsAny<IEnumerable<Landmark>>(), It.IsAny<Func<Landmark, bool>>()))
                        .Returns(nextLandmark);

            // Act
            _travel.ContinueTravelling();

            // Assert
            _mockGame.VerifySet(g => g.MilesToNextLandmark = nextLandmark.Distance);
            _mockGame.Verify(g => g.ChangeMode(Modes.Travelling), Times.Once);
            mockDisplay.Verify(d => d.Write("You decided to continue."));
        }
    }
}
