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


        private readonly List<string> _statuses = ["Healthy", "Sick", "Injured"];
        private OccurrenceEngine? _occurrenceEngine;

        public OccurrenceEngineTests()
        {
            _mockUtility = new Mock<IUtility>();
            _mockParty = new Mock<IWagonParty>();



        }




        [Fact]
        public void PickRandomOccurrence_ReturnsCorrectOccurrence_BasedOnProbability()
        {
            // Arrange
            var mockParty = new Mock<IWagonParty>();
            var mockUtility = new Mock<IUtility>();



            // Define mock occurrences with different probabilities
            var occurrences = new List<Occurrence>
            {
                new() { Name = "WILDANIMAL", DisplayText = "Wild Animal Encounter", Effect = "injured", Probability = 0.5 },
                new() { Name = "FOOD", DisplayText = "Found Food", Effect = "well-fed", Probability = 0.3 },
                new() { Name = "STORM", DisplayText = "Caught in a Storm", Effect = "wet", Probability = 0.2 }
            };

            // Mock the occurrence loading method
            mockUtility.Setup(u => u.LoadYaml<OccurrenceData>("data/Occurrences.yaml"))
                .Returns(new OccurrenceData { Occurrences = occurrences });

            // Mock the status loading method if needed
            mockUtility.Setup(u => u.LoadYaml<List<string>>("data/Statuses.yaml"))
                .Returns(new StatusData { Statuses = _statuses });


            // Create the OccurrenceEngine
            var occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", mockParty.Object, mockUtility.Object);

            // Act
            var pickedOccurrence = occurrenceEngine.PickRandomOccurrence();

            // Assert
            // Ensure the returned occurrence is one of the mock occurrences
            Assert.Contains(occurrences, o => o.Name == pickedOccurrence.Name);
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

            _occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", _mockParty.Object);

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
                new() { Name = "Occurrence 1", DisplayText = "Occurrence 1", Probability = 0.0, Effect = "no effect" },
                new() { Name = "Occurrence 2", DisplayText = "Occurrence 2", Probability = 0.0, Effect = "no effect" },
                new() { Name = "Occurrence 3", DisplayText = "Occurrence 3", Probability = 1.0, Effect = "no effect" }
            };

            _mockUtility.Setup(u => u.LoadYaml<OccurrenceData>("data/Occurrences.yaml"))
                        .Returns(new OccurrenceData { Occurrences = occurrences });

                                   
            _mockUtility.Setup(u => u.LoadYaml<List<string>>("data/Statuses.yaml"))
                .Returns(new StatusData { Statuses = _statuses });


            // Act
            _occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", _mockParty.Object, _mockUtility.Object);

            // Act
            var selectedOccurrence = _occurrenceEngine.PickRandomOccurrence();

            // Assert
            Assert.NotNull(selectedOccurrence);
            Assert.Equal("Occurrence 3", selectedOccurrence.Name);
        }
    }
}
