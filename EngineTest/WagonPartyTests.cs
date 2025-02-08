using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class WagonPartyTests
    {
        [Fact]
        public void Constructor_ShouldInitializeMembers()
        {
            // Arrange
            string[] names = { "Bob", "Alice", "Charlie" };

            // Act
            var wagonParty = new WagonParty(names);

            // Assert
            Assert.Equal("Bob", wagonParty.Leader.Name);
            Assert.Equal(wagonParty.Leader, wagonParty.GetLivingMembers().First());
          
        }

        [Fact]
        public void GetRandomMember_ShouldReturnMember()
        {
            // Arrange
            string[] names = { "Alice", "Bob", "Charlie" };
            var wagonParty = new WagonParty(names);

            // Act
            var member = wagonParty.GetRandomMember();

            // Assert
            Assert.Contains(member, wagonParty.Members);
        }

        [Fact]
        public void GetDisplayNames_ShouldReturnCommaSeparatedNames()
        {
            // Arrange
            string[] names = { "Alice", "Bob", "Charlie" };
            var wagonParty = new WagonParty(names);

            // Act
            var displayNames = wagonParty.GetDisplayNames();

            // Assert
            Assert.Contains("Alice, Bob, Charlie", displayNames);
        }

        [Fact]
        public void SpendDailyHealth_ShouldAdjustHealthCorrectly()
        {
            // Arrange
            string[] names = { "Alice", "Bob" };
            var wagonParty = new WagonParty(names);
            var initialHealth = 100;
            wagonParty.SetHealth(initialHealth);
            var pace = new Pace { Name="TestPace", Factor = 8 }; 
            var rations = new Rations { Name="TestRation", Factor = 2 }; 

            // Act
            wagonParty.SpendDailyHealth(pace, rations);

            // Assert
            Assert.NotEqual(initialHealth.ToString(), wagonParty.GetDisplayHealth());
        }

        [Fact]
        public void GeneratePerson_ShouldAssignUniqueID()
        {
            // Arrange
            var wagonParty = new WagonParty(["Alice", "Bob", "Charlie"]);

            // Act
            var person1 = wagonParty.GeneratePerson("Eve");
            var person2 = wagonParty.GeneratePerson("Mallory");

            // Assert
            Assert.NotEqual(person1.ID, person2.ID);
        }

        [Fact]
        public void GenerateRandomPerson_ShouldReturnPersonWithRandomName()
        {
            // Arrange
            string[] names = { "Alice", "Bob", "Charlie" };
            var wagonParty = new WagonParty(names);
            var person = wagonParty.GenerateRandomPerson(names);

            // Assert
            Assert.Contains(person.Name, names);
            Assert.Equal("Good", person.Status);
        }

         [Fact]
    public void GetLivingMembers_ShouldReturnOnlyAliveMembers()
    {
        // Arrange
        var mockUtility = new Mock<IUtility>();
        var wagonParty = new WagonParty(new[] { "Alice", "Bob", "Charlie" }, mockUtility.Object);
        
        // Kill one member
        wagonParty.Members[1].Kill();

        // Act
        var livingMembers = wagonParty.GetLivingMembers().ToList();

        // Assert
        Assert.Equal(2, livingMembers.Count);
        Assert.DoesNotContain(wagonParty.Members[1], livingMembers);
    }

    [Fact]
    public void IsAnybodyAlive_ShouldReturnTrue_WhenThereAreLivingMembers()
    {
        // Arrange
        var mockUtility = new Mock<IUtility>();
        var wagonParty = new WagonParty(new[] { "Alice", "Bob" }, mockUtility.Object);
        
        // Act
        var result = wagonParty.IsAnybodyAlive();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAnybodyAlive_ShouldReturnFalse_WhenAllMembersAreDead()
    {
        // Arrange
        var mockUtility = new Mock<IUtility>();
        var wagonParty = new WagonParty(new[] { "Alice", "Bob" }, mockUtility.Object);
        
        foreach (var member in wagonParty.Members)
        {
            member.Kill();
        }

        // Act
        var result = wagonParty.IsAnybodyAlive();

        // Assert
        Assert.False(result);
    }

        [Fact]
    public void KillMember_ShouldMarkMemberAsDead()
    {
        // Arrange
        var mockUtility = new Mock<IUtility>();
        var wagonParty = new WagonParty(new[] { "Alice", "Bob" }, mockUtility.Object);
        var memberToKill = wagonParty.Members[0];
        
        // Act
        wagonParty.KillMember(memberToKill);

        // Assert
        Assert.False(memberToKill.isAlive);
    }

    [Fact]
    public void KillMember_ShouldNotAffectOtherMembers()
    {
        // Arrange
        var mockUtility = new Mock<IUtility>();
        var wagonParty = new WagonParty(new[] { "Alice", "Bob" }, mockUtility.Object);
        var memberToKill = wagonParty.Members[0];
        var otherMember = wagonParty.Members[1];
        
        // Act
        wagonParty.KillMember(memberToKill);

        // Assert
        Assert.False(memberToKill.isAlive);
        Assert.True(otherMember.isAlive);
    }


    [Fact]
    public async Task LoadMembersAsync_ShouldPopulateMembers()
    {
        // Arrange
        var mockUtility = new Mock<IUtility>();
        var mockData = new RandomNamesData { RandomNames = ["Alice", "Bob", "Charlie", "Dave", "Eve"] };
        mockUtility.Setup(u => u.GetAppSetting("RandomNamesFilePath")).Returns("mockPath");
        mockUtility.Setup(u => u.LoadYamlAsync<RandomNamesData>("mockPath")).ReturnsAsync(mockData);
        
        // Act
        var wagonParty = await WagonParty.CreateAsync(mockUtility.Object);

        // Assert
        Assert.Equal(5, wagonParty.Members.Count);
    }

        [Fact]
        public async Task GetDisplayHealth_ShouldReturnFormattedHealth()
        {

                    // Arrange
            var mockUtility = new Mock<IUtility>();
            var mockData = new RandomNamesData { RandomNames = ["Alice", "Bob", "Charlie", "Dave", "Eve"] };
            mockUtility.Setup(u => u.GetAppSetting("RandomNamesFilePath")).Returns("mockPath");
            mockUtility.Setup(u => u.LoadYamlAsync<RandomNamesData>("mockPath")).ReturnsAsync(mockData);
            // Arrange

            mockUtility.Setup(u => u.Abbreviate(It.IsAny<double>())).Returns<double>(h => Convert.ToInt32(h).ToString());
            var wagonParty = await WagonParty.CreateAsync(mockUtility.Object);
            wagonParty.SetHealth(100.5);

            // Act
            string result = wagonParty.GetDisplayHealth();

            // Assert
            Assert.Equal("100", result);
        }
    }

}
