using Xunit;
using Moq;
using System;
using System.Linq;
using DestinyTrail.Engine; 

namespace DestinyTrail.Engine.Tests;

public class OccurrenceEngineTests
{
    private readonly Mock<IUtility> _mockUtility;
    private readonly Mock<IWagonParty> mockWagonParty;

    private readonly  Mock<IPerson> mockPerson;


    private readonly List<Status> _statuses = [new(){Name="Healthy"},new(){Name="Sick"}, new(){Name="Injured"}];

    private readonly List<Occurrence> occurrences = new List<Occurrence>
    {
        new() { Name = "Occurrence 1", DisplayText = "Occurrence 1", Probability = 0.0, Effect = "no effect" },
        new() { Name = "Occurrence 2", DisplayText = "Occurrence 2", Probability = 0.0, Effect = "no effect" },
        new() { Name = "Occurrence 3", DisplayText = "Occurrence 3", Probability = 1.0, Effect = "no effect" }
    };


    public OccurrenceEngineTests()
    {
        _mockUtility = new Mock<IUtility>();

        _mockUtility.Setup(u => u.LoadYamlAsync<OccurrenceData>(It.IsAny<string>()))
                    .ReturnsAsync(new OccurrenceData { Occurrences = occurrences });

        _mockUtility.Setup(u => u.LoadYamlAsync<StatusData>(It.IsAny<string>()))
            .ReturnsAsync(new StatusData { Statuses = _statuses });

        mockWagonParty = new Mock<IWagonParty>();

        mockPerson = new Mock<IPerson>();
        mockPerson.Setup(p => p.Name).Returns("John");

    }

    [Fact]
    public async Task PickRandomOccurrence_ReturnsCorrectOccurrence_BasedOnProbability()
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


        mockUtility.Setup(u => u.LoadYamlAsync<OccurrenceData>(It.IsAny<string>()))
            .ReturnsAsync(new OccurrenceData { Occurrences = occurrences });

        // Mock the status loading method if needed
        mockUtility.Setup(u => u.LoadYamlAsync<StatusData>(It.IsAny<string>()))
            .ReturnsAsync(new StatusData { Statuses = _statuses });


        // Create the OccurrenceEngine
        var occurrenceEngine = await OccurrenceEngine.CreateAsync(mockParty.Object, mockUtility.Object);

        // Act
        var pickedOccurrence = occurrenceEngine.PickRandomOccurrence();

        // Assert
        // Ensure the returned occurrence is one of the mock occurrences
        Assert.Contains(occurrences, o => o.Name == pickedOccurrence.Name);
    }



    [Fact]
    public async Task ProcessOccurrence_ReplacesNameAndAppliesEffect()
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


        var occurrenceEngine = await OccurrenceEngine.CreateAsync( mockWagonParty.Object, _mockUtility.Object);

        // Act
        var processedOccurrence = occurrenceEngine.ProcessOccurrence(occurrence);

        // Assert
        Assert.Equal("John encountered a wild animal.", processedOccurrence.DisplayText);
        mockPerson.VerifySet(p => p.Status = It.Is<Status>(s => s.Name == "Injured"));
    }

    [Fact]
    public async Task TryProcessEffect_ShouldIncreaseInventoryItem_IfEffectContainsIncrement() {
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

        var _occurrenceEngine = await OccurrenceEngine.CreateAsync(mockWagonParty.Object, _mockUtility.Object);

        // Act
        _occurrenceEngine.ProcessOccurrence(occurrence);

        // Assert
        Assert.Equal(67, mockInventory.Object.Food.Quantity);
    }


    [Fact]
    public async Task TrySetFlag_ShouldSetBooleanSetting()
    {
        // Arrange
        var mockParty = new Mock<IWagonParty>();

        mockParty.Setup(p => p.Flags).Returns(new Dictionary<string, object>
        {
            { "CanHunt", true }
        });
        

        _mockUtility.Setup(u => u.LoadYamlAsync<OccurrenceData>(It.IsAny<string>()))
                    .ReturnsAsync(new OccurrenceData { Occurrences = occurrences });

        _mockUtility.Setup(u => u.LoadYamlAsync<StatusData>(It.IsAny<string>()))
            .ReturnsAsync(new StatusData { Statuses = _statuses });

        var occurrenceEngine = await OccurrenceEngine.CreateAsync( mockParty.Object, _mockUtility.Object);
        var mockOccurrence = new Mock<IOccurrence>();


        mockOccurrence.Setup(o => o.Effect).Returns("[Flags.CanHunt] = false");

        // Act
        occurrenceEngine.TrySetFlag(mockOccurrence.Object);

        // Assert
        Assert.Equal(false, mockParty.Object.Flags["CanHunt"]);
    }

    [Fact]
    public async Task TrySetQuantityInventoryItem_ShouldErrorOnNotFoundItem()
    {
        // Arrange
        var mockInventory = new Mock<Inventory>();
        mockInventory.Object.Food.Add(100);
        mockWagonParty.Setup(wp => wp.Inventory).Returns(mockInventory.Object);

        var occurrence = new Occurrence
        {
            Name = "NotExist",
            DisplayText = "This item doesn't exist and should throw an error",
            Probability = 1.0,
            Effect = "[NotExistItem] = 0"
        };

        mockWagonParty.Setup(p => p.GetRandomMember()).Returns(mockPerson.Object);


        _mockUtility.Setup(u => u.LoadYamlAsync<OccurrenceData>(It.IsAny<string>()))
                    .ReturnsAsync(new OccurrenceData { Occurrences = occurrences });

        _mockUtility.Setup(u => u.LoadYamlAsync<StatusData>(It.IsAny<string>()))
            .ReturnsAsync(new StatusData { Statuses = _statuses });

        var _occurrenceEngine = await OccurrenceEngine.CreateAsync(mockWagonParty.Object, _mockUtility.Object);


        // Assert
        Assert.Throws<Exception>(() => _occurrenceEngine.TrySetQuantityInventoryItem(occurrence));

    }

    [Fact]
    public async Task TrySetQuantityInventoryItem_ShouldZeroInventoryItem()
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

        var _occurrenceEngine = await OccurrenceEngine.CreateAsync(mockWagonParty.Object, _mockUtility.Object);

        // Act
        _occurrenceEngine.TrySetQuantityInventoryItem(occurrence);

        // Assert
        Assert.Equal(0, mockInventory.Object.Food.Quantity);
    }
}
