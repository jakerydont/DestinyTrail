using System;
using System.Collections.Generic;
using System.IO;
using DestinyTrail.Engine;

using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class UtilityTests {
        [Fact]
        public void LoadYaml_ShouldReturnDeserializedObject_WhenYamlFileExists() {
            // Arrange
            var yamlFilePath = "data/Test.yaml";

            // Act
            var result = Utility.LoadYaml<TestTypeData>(yamlFilePath);

            // Assert
            Assert.Collection(result,
                item => Assert.Equal("First", item.Name),
                item => Assert.Equal("Middle One", item.Name),
                item => Assert.Equal("Last", item.Name)
            );
        }

        [Fact]
        public void NextOrFirst_ShouldReturnNextElement_WhenPredicateMatches() {
            // Arrange
            var collection = new List<int> { 1, 2, 3, 4 };
            Func<int, bool> predicate = x => x == 2;

            // Act
            var result = collection.NextOrFirst(predicate);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void NextOrFirst_ShouldReturnZero_WhenPredicateDoesNotMatch() {
            // Arrange
            var collection = new List<int> { 1, 2, 3, 4 };
            Func<int, bool> predicate = x => x == 5;

            // Act
            var result = collection.NextOrFirst(predicate);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Abbreviate_ShouldReturnIntegerString_WhenNumberIsDouble() {
            // Arrange
            double number = 1234.56;

            // Act
            var result = Utility.Abbreviate(number);

            // Assert
            Assert.Equal("1235", result);
        }

        [Fact]
        public void GetFormatted_ShouldReturnFormattedDate_WhenDateIsProvided() {
            // Arrange
            DateTime date = new DateTime(2024, 11, 1);

            // Act
            var result = date.GetFormatted();

            // Assert
            Assert.Equal("November 1, 2024", result);
        }
    }
}
