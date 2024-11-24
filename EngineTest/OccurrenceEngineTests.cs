using Xunit;
using Moq;
using System;
using System.Linq;
using DestinyTrail.Engine; // Add this line to include the namespace where IWagonParty is defined

namespace DestinyTrail.Engine.Tests
{
    public class OccurrenceEngineTests
    {
        private readonly Mock<IUtility> _mockUtility;
        private readonly Mock<IWagonParty> mockWagonParty;

        private readonly  Mock<IPerson> mockPerson;


        private readonly List<string> _statuses = ["Healthy", "Sick", "Injured"];

                    // Arrange
        private readonly List<Occurrence> occurrences = new List<Occurrence>
            {
                new() { Name = "Occurrence 1", DisplayText = "Occurrence 1", Probability = 0.0, Effect = "no effect" },
                new() { Name = "Occurrence 2", DisplayText = "Occurrence 2", Probability = 0.0, Effect = "no effect" },
                new() { Name = "Occurrence 3", DisplayText = "Occurrence 3", Probability = 1.0, Effect = "no effect" }
            };
        private OccurrenceEngine? _occurrenceEngine;

        public OccurrenceEngineTests()
        {
            _mockUtility = new Mock<IUtility>();
            mockWagonParty = new Mock<IWagonParty>();

            mockPerson = new Mock<IPerson>();
            mockPerson.Setup(p => p.Name).Returns("John");

        }




        [Fact]
        public void PickRandomOccurrence_ReturnsCorrectOccurrence_BasedOnProbability()
        {

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

            mockWagonParty.Setup(wp => wp.Inventory).Returns(new Inventory());
            mockWagonParty.Setup(wp => wp.GetRandomMember()).Returns(mockPerson.Object);

            _occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", mockWagonParty.Object);

            // Act
            var processedOccurrence = _occurrenceEngine.ProcessOccurrence(occurrence);

            // Assert
            Assert.Equal("John encountered a wild animal.", processedOccurrence.DisplayText);
            mockPerson.VerifySet(p => p.Status = It.Is<Status>(s => s.Name == "Injured"));
        }

        [Fact]
        public void TryProcessEffect_ShouldIncreaseInventoryItem_IfEffectContainsIncrement() {
            // Arrange


            var mockInventory = new Mock<Inventory>();
            mockWagonParty.Setup(wp => wp.Inventory).Returns(mockInventory.Object);

            var occurrence = new Occurrence
            {
                Name = "FOOD",
                DisplayText = "You found food",
                Probability = 1.0,
                Effect = "[Food] += 67"
            };

            mockWagonParty.Setup(p => p.GetRandomMember()).Returns(mockPerson.Object);

            _occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", mockWagonParty.Object);

            // Act
            _occurrenceEngine.ProcessOccurrence(occurrence);

            // Assert
            Assert.Equal(67, mockInventory.Object.Food.Quantity);
        }


        [Fact]
        public void TrySetBoolean_ShouldSetBooleanSetting()
        {
            // Arrange
            var mockParty = new Mock<IWagonParty>();

            mockParty.Setup(p => p.Flags).Returns(new Dictionary<string, object>
            {
                { "CanHunt", true }
            });
            

            _mockUtility.Setup(u => u.LoadYaml<OccurrenceData>("data/Occurrences.yaml"))
                        .Returns(new OccurrenceData { Occurrences = occurrences });

            _mockUtility.Setup(u => u.LoadYaml<List<string>>("data/Statuses.yaml"))
                .Returns(new StatusData { Statuses = _statuses });

            var occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", mockParty.Object, _mockUtility.Object);
            var mockOccurrence = new Mock<IOccurrence>();


            mockOccurrence.Setup(o => o.Effect).Returns("[Flags.CanHunt] = false");

            // Act
            occurrenceEngine.TrySetFlag(mockOccurrence.Object);

            // Assert
            Assert.Equal(false, mockParty.Object.Flags["CanHunt"]);
        }

        [Fact]
        public void TryZeroInventoryItem_ShouldZeroInventoryItem()
        {
            // Arrange
            var mockInventory = new Mock<Inventory>();
            mockInventory.Object.Food.Add(100);
            mockWagonParty.Setup(wp => wp.Inventory).Returns(mockInventory.Object);

            var occurrence = new Occurrence
            {
                Name = "FOOD",
                DisplayText = "A thief stole all your food",
                Probability = 1.0,
                Effect = "[Food] = 0"
            };

            mockWagonParty.Setup(p => p.GetRandomMember()).Returns(mockPerson.Object);

            _occurrenceEngine = new OccurrenceEngine("data/Occurrences.yaml", mockWagonParty.Object);

            // Act
            _occurrenceEngine.TryZeroInventoryItem(occurrence);

            // Assert
            Assert.Equal(0, mockInventory.Object.Food.Quantity);
        }
    }
}
