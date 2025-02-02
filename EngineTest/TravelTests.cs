using Xunit;
using Moq;
using System.Threading.Tasks;

namespace DestinyTrail.Engine.Tests
{
    public class TravelTests
    {
        private readonly Mock<IWagonParty> _mockWagonParty;
        private readonly Mock<IUtility> _mockUtility;
        private readonly Mock<IDisplay> _mockDisplay;
        private readonly Mock<IWorldStatus> _mockWorldStatus;

        private readonly Travel _travel;


        public TravelTests()
        {
            _mockUtility = new Mock<IUtility>();

            _mockWorldStatus = new Mock<IWorldStatus>();
            _mockWorldStatus.Setup(ws => ws.CurrentDate).Returns(DateTime.Now);

            _mockDisplay = new Mock<IDisplay>();

            // Mocking game properties
            _mockWagonParty = new Mock<IWagonParty>();

            _mockWagonParty.Setup(w => w.GetRandomMember()).Returns(new Person { ID = 0, Name = "Greg", Status = new Status { Name = "Healthy" } });

        

            // Mocking YAML loading
            _mockUtility.Setup(u => u.GetAppSetting(It.IsAny<string>())).Returns(string.Empty);
            _mockUtility.Setup(u => u.LoadYamlAsync<StatusData>(It.IsAny<string>()))
                .ReturnsAsync(new StatusData { Statuses = new() {new() { Name ="Healthy"},new(){  Name ="Sick"} } });

            _mockUtility.Setup(u => u.LoadYamlAsync<PaceData>(It.IsAny<string>()))
                .ReturnsAsync(new PaceData { Paces = [new Pace { Name = "Slow", Factor = 10 }, new Pace { Name = "Fast", Factor = 20 } ]});

            _mockUtility.Setup(u => u.LoadYamlAsync<RationData>(It.IsAny<string>()))
                .ReturnsAsync(new RationData { Rations = [new Rations { Name = "Meager", Factor = 1 }, new Rations { Name = "Full", Factor = 2 } ]});

            _mockUtility.Setup(u => u.LoadYamlAsync<OccurrenceData>(It.IsAny<string>()))
                .ReturnsAsync(new OccurrenceData { Occurrences = [
                    new Occurrence { Name = "ATE_IT", DisplayText = "{name} ate it", Effect = "none" }, 
                    new Occurrence { Name = "Occurrence 2", DisplayText = "{name} is an occurrence", Effect = "dead" } ]});

            _mockUtility.Setup(u => u.LoadYamlAsync<LandmarksData>(It.IsAny<string>()))
                .ReturnsAsync(new LandmarksData { Landmarks = [
                    new Landmark { ID = "FORT_LARAMIE", Name = "Fort Laramie", Distance = 150, Lore = "Fun place" },
                    new Landmark { ID = "second_landmark", Name = "Second Landmark", Distance = 100, Lore = "other place" } 
                    ]});

            // Creating the Travel object with mocked dependencies
            _travel = Travel.CreateAsync(_mockWagonParty.Object,
            _mockUtility.Object,
             _mockDisplay.Object, 
             _mockWorldStatus.Object).GetAwaiter().GetResult();

            Assert.NotNull(_travel);

             _travel.MilesTraveled =0;
        }

        [Fact]
        public void Constructor_InitializesDependencies()
        {
            // Assert
            _mockUtility.Verify(u => u.LoadYamlAsync<PaceData>(It.IsAny<string>()), Times.Once);
            _mockUtility.Verify(u => u.LoadYamlAsync<RationData>(It.IsAny<string>()), Times.Once);


            Assert.NotNull(_travel.Pace);
            Assert.NotNull(_travel.Rations);
            Assert.NotNull(_travel.occurrenceEngine);

            Assert.Equal(0, _travel.MilesTraveled);
            Assert.Equal(150, _travel.MilesToNextLandmark);
        }

        [Fact]
        public async Task TravelLoop_UpdatesMilesTraveledAndStatus_WhenTraveling()
         {
            // Arrange
             _travel.MilesToNextLandmark = 50;

            // Act
            await _travel.TravelLoop();

            // Assert           
            Assert.Equal(50 - _travel.Pace.Factor, _travel.MilesToNextLandmark);
            Assert.Equal(_travel.MilesTraveled, _travel.Pace.Factor);

        }

        [Fact]
        public void TravelLoop_ChangesModeToAtLandmark_WhenMilesToNextLandmarkIsZero()
        {
            // Arrange
            _travel.MilesToNextLandmark = 0;
            _travel.NextLandmark.Name = "Fort Kearney";
            bool eventTriggered = false;
            _travel.ModeChanged += (mode) => { if (mode == Modes.AtLandmark) eventTriggered = true; };

            // Act
            _travel.TravelLoop();

            // Assert
            Assert.True(eventTriggered);

            // Verify that the display includes the correct landmark message
            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Fort Kearney"))), Times.Once);
        }


        [Fact]
        public void TravelLoop_ProcessesOccurrence_WhenStillTraveling()
        {
            // Arrange


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

            _travel.occurrenceEngine = mockOccurrenceEngine.Object;

            // Act
            _travel.TravelLoop();

            // Assert
            mockOccurrenceEngine.Verify(o => o.PickRandomOccurrence(), Times.Once);
            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Wild Animal Encounter"))), Times.Once);
        }


        [Fact]
        public async Task ContinueTravelling_ResetsMilesAndChangesMode()
        {
            // Arrange
            var nextLandmark = new Landmark { ID = "FORT_LARAMIE" , Name = "Fort Laramie", Distance = 150, Lore="Fun place" };
            _travel.NextLandmark = nextLandmark;
            _travel._landmarksData = new LandmarksData{Landmarks = new List<Landmark> { nextLandmark }};

            bool eventTriggered = false;
            _travel.ModeChanged += (mode) => { if (mode == Modes.Travelling) eventTriggered = true; };

            _mockUtility.Setup(u => u.NextOrFirst(It.IsAny<IEnumerable<Landmark>>(), It.IsAny<Func<Landmark, bool>>()))
                        .Returns(nextLandmark);

            // Act
            await _travel.ContinueTravelling();

            // Assert
            Assert.Equal(_travel.MilesToNextLandmark, nextLandmark.Distance);
            Assert.True(eventTriggered);
            _mockDisplay.Verify(d => d.Write("You decided to continue."));
        }
    }


}
