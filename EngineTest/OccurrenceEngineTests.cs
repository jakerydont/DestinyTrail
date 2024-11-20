using Xunit;
using Moq;
using System;
using System.Linq;

namespace DestinyTrail.Engine.Tests
{
    public class OccurrenceEngineTests
    {
        private readonly Mock<IUtility> _mockUtility;
        private readonly Mock<IWagonParty> _mockParty;

        private List<Occurrence> TestOccurrences { get; set; }

        private readonly string[] _statuses = { "Healthy", "Sick", "Injured" };
        private OccurrenceEngine _occurrenceEngine;

        public OccurrenceEngineTests()
        {
            _mockUtility = new Mock<IUtility>();
            _mockParty = new Mock<IWagonParty>();
                        
                        
             TestOccurrences = new List<Occurrence>
            {
                new Occurrence { Name = "Occurence1", DisplayText = "Occurrence 1", Probability = 0.5, Effect = "no effect" },
                new Occurrence { Name = "Occurence1", DisplayText = "Occurrence 2", Probability = 0.5, Effect = "no effect" }
            };

            _mockUtility.Setup(u => u.LoadYaml<OccurrenceData>("data/Occurrences.yaml"))
                        .Returns(new OccurrenceData { Occurrences = TestOccurrences });

            // Act
            _occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", _mockParty.Object, _statuses, _mockUtility.Object);
        }

        [Fact]
        public void Constructor_LoadsOccurrencesFromYaml()
        {
            // Assert
            _mockUtility.Verify(u => u.LoadYaml<OccurrenceData>("data/Occurrences.yaml"), Times.Once);
        }

        [Fact]
        public void PickRandomOccurrence_ReturnsOccurrenceBasedOnProbability()
        {
            // Arrange
            var totalProbability = TestOccurrences.Sum(o => o.Probability);
            var random = new Random(0);

            // Act
            var selectedOccurrence = _occurrenceEngine.PickRandomOccurrence();

            // Assert
            Assert.NotNull(selectedOccurrence);
            Assert.Contains(selectedOccurrence, TestOccurrences);
        }

        [Fact]
        public void ProcessOccurrence_ReplacesNameAndAppliesEffect()
        {
            // Arrange
            var occurrence = new Occurrence
            {
                Name = "WILD_ANIMAL",
                DisplayText = "{name} encountered a wild animal.",
                Probability = 1.0,
                Effect = "Injured"
            };

            var mockPerson = new Mock<IPerson>();
            mockPerson.Setup(p => p.Name).Returns("John");
            _mockParty.Setup(p => p.GetRandomMember()).Returns(mockPerson.Object);

            _occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", _mockParty.Object, _statuses);

            // Act
            var processedOccurrence = _occurrenceEngine.ProcessOccurrence(occurrence);

            // Assert
            Assert.Equal("John encountered a wild animal.", processedOccurrence.DisplayText);
            mockPerson.VerifySet(p => p.Status = It.Is<Status>(s => s.Name == "Injured"));
        }

        [Fact]
        public void PickRandomOccurrence_ReturnsLastOccurrence_IfProbabilitiesMismatch()
        {
            // Arrange
            var occurrences = new List<Occurrence>
            {
                new Occurrence { Name = "Occurrence 1", DisplayText = "Occurrence 1", Probability = 0.0, Effect = "no effect" },
                new Occurrence { Name = "Occurrence 2", DisplayText = "Occurrence 2", Probability = 0.0, Effect = "no effect" },
                new Occurrence { Name = "Occurrence 3", DisplayText = "Occurrence 3", Probability = 1.0, Effect = "no effect" }
            };

            _mockUtility.Setup(u => u.LoadYaml<OccurrenceData>("data/Occurrences.yaml"))
                        .Returns(new OccurrenceData { Occurrences = occurrences });

            _occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", _mockParty.Object, _statuses, _mockUtility.Object);

            // Act
            var selectedOccurrence = _occurrenceEngine.PickRandomOccurrence();

            // Assert
            Assert.NotNull(selectedOccurrence);
            Assert.Equal("Occurrence 3", selectedOccurrence.Name);
        }
    }
}
