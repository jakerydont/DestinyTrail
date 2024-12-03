using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using DestinyTrail.Engine;
using DestinyTrail.Engine.Abstractions;
using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class UtilityTests
    {
        private Utility _utility;

        public UtilityTests()
        {
            _utility = new Utility(
                new Mock<IYamlDeserializer>().Object,
                new Mock<IFileReader>().Object,
                new Mock<IConfigurationProvider>().Object
            );
        }


        [Fact]
        public void LoadYaml_ShouldReturnDeserializedObject_WhenYamlFileExists()
        {


            // Arrange
            var yamlFilePath = "data/Test.yaml";
            var yamlContent = """
                Tests:
                - Name: "First"
                - Name: "Middle One"
                - Name: "Last"
                """;

            var _mockFileReader = new Mock<IFileReader>();

            // Mock the file read operation
            _mockFileReader
                .Setup(fr => fr.ReadAllText(It.IsAny<string>()))
                .Returns(yamlContent);

            var mockYamlDotNetDeserializer = new Mock<IYamlDeserializer>();

            // Mock the deserialization process using the YAML content, not the file path
            mockYamlDotNetDeserializer
                .Setup(d => d.Deserialize<TestTypeData>(yamlContent))
                .Returns(new TestTypeData
                {
                    Tests = new List<TestType>
                    {
                        new() { Name = "First" },
                        new() { Name = "Middle One" },
                        new() { Name = "Last" }
                    }
                });

            var _utility = new Utility(mockYamlDotNetDeserializer.Object, _mockFileReader.Object, new Mock<IConfigurationProvider>().Object);

            // Act
            var result = _utility.LoadYaml<TestTypeData>(yamlFilePath);

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Collection(result.Tests,
                item => Assert.Equal("First", item.Name),
                item => Assert.Equal("Middle One", item.Name),
                item => Assert.Equal("Last", item.Name)
            );
        }

        [Fact]
        public void LoadYaml_ShouldRetrunStringsFromStatus()
        {
            // Arrange
            var yamlContent = @"
                Statuses:
                - Name: 'Healthy'
                - Name: 'Injured'
                - Name: 'Dead'
            ";

            var _mockFileReader = new Mock<IFileReader>();

            // Mock the file read operation
            _mockFileReader
                .Setup(fr => fr.ReadAllText(It.IsAny<string>()))
                .Returns(yamlContent);

            var mockYamlDotNetDeserializer = new Mock<IYamlDeserializer>();

            // Mock the deserialization process using the YAML content, not the file path
            mockYamlDotNetDeserializer
                .Setup(d => d.Deserialize<StatusData>(yamlContent))
                .Returns(new StatusData
                {
                    Statuses = new()
                    {
                        new(){Name="Healthy"} ,
                        new(){Name="Injured"},
                        new(){Name="Dead"}
                    }
                });

            var _utility = new Utility(mockYamlDotNetDeserializer.Object, _mockFileReader.Object, new Mock<IConfigurationProvider>().Object);

            // Act
            var result = _utility.LoadYaml<StatusData>(It.IsAny<string>());

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Collection(result.Statuses,
                item => Assert.Equal("Healthy", item),
                item => Assert.Equal("Injured", item),
                item => Assert.Equal("Dead", item)
            );
        }

        [Fact]
        public void NextOrFirst_ShouldReturnNextElement_WhenPredicateMatches()
        {
            // Arrange
            var collection = new List<int> { 1, 2, 3, 4 };
            Func<int, bool> predicate = x => x == 2;

            // Act
            var result = _utility.NextOrFirst(collection, predicate);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void NextOrFirst_ShouldReturnZero_WhenPredicateDoesNotMatch()
        {
            // Arrange
            var collection = new List<int> { 1, 2, 3, 4 };
            Func<int, bool> predicate = x => x == 5;

            // Act
            var result = _utility.NextOrFirst(collection, predicate);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Abbreviate_ShouldReturnIntegerString_WhenNumberIsDouble()
        {
            // Arrange
            double number = 1234.56;

            // Act
            var result = _utility.Abbreviate(number);

            // Assert
            Assert.Equal("1235", result);
        }

        [Fact]
        public void GetFormatted_ShouldReturnFormattedDate_WhenDateIsProvided()
        {
            // Arrange
            DateTime date = new DateTime(2024, 11, 1);

            // Act
            var result = _utility.GetFormatted(date);

            // Assert
            Assert.Equal("November 1, 2024", result);
        }

        [Fact]
        public void GetAppSetting_ShouldReturnSettingValue_WhenSettingExists()
        {
            // Arrange
            var settingName = "TestSetting";
            var expectedValue = "TestValue";
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider.Setup(cp => cp.GetAppSetting(settingName)).Returns(expectedValue);
            
            var utility = new Utility( 
                new Mock<IYamlDeserializer>().Object, 
                new Mock<IFileReader>().Object, 
                mockConfigurationProvider.Object
            );



            // Act
            var result = utility.GetAppSetting(settingName);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetAppSetting_ShouldThrowException_WhenSettingDoesNotExist()
        {
            // Arrange
            var nonexistentSettingName = "NonExistentSetting";
            var existingSettingName = "ExistingSetting";
            var expectedValue = "TestValue";

            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider.Setup(cp => cp.GetAppSetting(existingSettingName)).Returns(expectedValue);
            
            var utility = new Utility(
                new Mock<IYamlDeserializer>().Object, 
                new Mock<IFileReader>().Object,
                mockConfigurationProvider.Object
            );


            // Act & Assert
            var exception = Assert.Throws<ConfigurationErrorsException>(() => utility.GetAppSetting(nonexistentSettingName));
            Assert.Equal($"{nonexistentSettingName} is not configured.", exception.Message);
        }
    }
}
