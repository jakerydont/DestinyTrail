using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class WagonPartyTests
    {
        [Fact]
        public void Constructor_ShouldInitializeMembers()
        {
            // Arrange
            string[] names = { "Alice", "Bob", "Charlie", "Diana" };
            int expectedSize = 3;

            // Act
            var wagonParty = new WagonParty(names, expectedSize);

            // Assert
            Assert.Equal(expectedSize, wagonParty.Members.Count);
            Assert.NotNull(wagonParty.Leader);
            Assert.Equal(wagonParty.Leader, wagonParty.Members.First());
            Assert.Equal(100, wagonParty.Health); // Initial health should be 100
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
            Assert.Equal("Alice, Bob, Charlie", displayNames);
        }

        [Fact]
        public void SpendDailyHealth_ShouldAdjustHealthCorrectly()
        {
            // Arrange
            string[] names = { "Alice", "Bob" };
            var wagonParty = new WagonParty(names);
            var initialHealth = wagonParty.Health;
            var pace = new Pace { Name="TestPace", Factor = 4 }; // Example pace factor
            var rations = new Rations { Name="TestRation", Factor = 2 }; // Example rations factor

            // Act
            wagonParty.SpendDailyHealth(pace, rations);

            // Assert
            Assert.NotEqual(initialHealth, wagonParty.Health);
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
            Assert.Equal("Good", person.Status.Name);
        }
    }

}
