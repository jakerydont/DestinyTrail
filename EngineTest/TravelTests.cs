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
            _mockGame.Setup(g => g.Party).Returns(new Mock<IWagonParty>().Object);

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
            Assert.NotNull(_travel._pace);
            Assert.NotNull(_travel._rations);
            Assert.NotNull(_travel._occurrenceEngine);
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
            _mockGame.VerifySet(g => g.MilesToNextLandmark = 50 - _travel._pace.Factor);
            _mockGame.VerifySet(g => g.MilesTraveled = _travel._pace.Factor);

        }

        [Fact]
        public void TravelLoop_ChangesModeToAtLandmark_WhenMilesToNextLandmarkIsZero()
        {
            // Arrange
            var mockDisplay = new Mock<IDisplay>();
            _mockGame.Setup(g => g._display).Returns(mockDisplay.Object);
            _mockGame.Setup(g => g.MilesToNextLandmark).Returns(_travel._pace.Factor); // Set exact distance to reach landmark
            _mockGame.Setup(g => g.NextLandmark.Name).Returns("Fort Kearney");

            // Act
            _travel.TravelLoop();

            // Assert
            _mockGame.Verify(g => g.ChangeMode(Modes.AtLandmark), Times.Once);
            mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Fort Kearney"))));
        }

        [Fact]
        public void TravelLoop_ProcessesOccurrence_WhenStillTraveling()
        {
            // Arrange
            var mockDisplay = new Mock<IDisplay>();
            _mockGame.Setup(g => g._display).Returns(mockDisplay.Object);
            _mockGame.Setup(g => g.MilesToNextLandmark).Returns(50);

            var mockOccurrenceEngine = new Mock<OccurrenceEngine>();
            mockOccurrenceEngine.Setup(o => o.PickRandomOccurrence()).Returns(new Occurrence { Name="WILDANIMAL", DisplayText = "Wild Animal Encounter", Effect="dead" });

            _travel._occurrenceEngine = mockOccurrenceEngine.Object;

            // Act
            _travel.TravelLoop();

            // Assert
            mockOccurrenceEngine.Verify(o => o.PickRandomOccurrence(), Times.Once);
            mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Wild Animal Encounter"))));
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
