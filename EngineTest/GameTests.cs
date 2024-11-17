using System;
using Xunit;
using Moq;
using DestinyTrail.Engine;
using System.Threading.Tasks;

namespace DestinyTrail.Engine.Tests
{
    public class GameTests
    {
        private readonly Mock<IDisplay> _mockDisplay;
        private readonly Mock<IDisplay> _mockStatus;
        private readonly Mock<IUtility> _mockUtility;
        private readonly Game _game;

        public GameTests()
        {
            _mockDisplay = new Mock<IDisplay>();
            _mockStatus = new Mock<IDisplay>();
            _mockUtility = new Mock<IUtility>();

            // Mocking Utility.LoadYaml method to return mock data
            _mockUtility.Setup(u => u.LoadYaml<RandomNamesData>("data/RandomNames.yaml")).Returns(new RandomNamesData{RandomNames=new List<string>{"Name One", "Name Two", "Name Three"}});
            _mockUtility.Setup(u => u.LoadYaml<LandmarksData>("data/Landmarks.yaml")).Returns(new LandmarksData());
            _mockUtility.Setup(u => u.LoadYaml<Inventory>("data/Inventory.yaml")).Returns(new Inventory());

            // Initialize the Game with the mocked dependencies
            _game = new Game(_mockDisplay.Object, _mockStatus.Object, _mockUtility.Object);
        }

        [Fact]
        public void Game_InitializesCorrectly()
        {
            // Verify that key properties are initialized correctly
            Assert.NotNull(_game.RandomNames);
            Assert.Equal(0, _game.MilesTraveled);
            Assert.Equal(0, _game.MilesToNextLandmark);
            Assert.Equal(Modes.Travelling, _game.GameMode);
        }

        [Fact]
        public void ChangeMode_ChangesGameMode()
        {
            // Arrange: Game starts in Travelling mode
            var initialMode = _game.GameMode;

            // Act: Change mode to AtLandmark
            _game.ChangeMode(Modes.AtLandmark);

            // Assert: The mode should be changed to AtLandmark
            Assert.Equal(Modes.AtLandmark, _game.GameMode);

            // Act: Change mode to Shopping
            _game.ChangeMode(Modes.Shopping);

            // Assert: The mode should be changed to Shopping
            Assert.Equal(Modes.Shopping, _game.GameMode);
        }

        [Fact]
        public void ShoppingLoop_StateTransitionsCorrectly()
        {
            // Arrange: Initial ShoppingState should be Init
            Assert.Equal(ShoppingState.Init, _game.ShoppingEngine.ShoppingState);

            // Act: Start shopping loop
            _game.ShoppingEngine.SelectShoppingItem("Oxen");

            // Assert: The state should transition to HowMany
            Assert.Equal(ShoppingState.AskQuantity, _game.ShoppingEngine.ShoppingState);
        }

        [Fact]
        public void DrawStatusPanel_UpdatesStatusDisplay()
        {
            // Act: Call DrawStatusPanel
            _game.DrawStatusPanel();

            // Assert: Verify that the status display writes the correct information
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Date:"))), Times.Once);
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Weather:"))), Times.Once);
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Health:"))), Times.Once);
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Distance to next landmark:"))), Times.Once);
            _mockStatus.Verify(s => s.Write(It.Is<string>(str => str.Contains("Distance traveled:"))), Times.Once);
        }

        [Fact]
        public void SelectShoppingItem_SelectsValidItem()
        {
            // Arrange: Assume inventory has an item "Oxen"
            var itemName = "Oxen";
            _mockUtility
                .Setup(u => u.LoadYaml<Inventory>("data/Inventory.yaml"))
                .Returns(new Inventory());

            // Act: Select shopping item
            _game.ShoppingEngine.SelectShoppingItem(itemName);

            // Assert: The shopping selection should be the "Oxen" item
            Assert.Equal(itemName, _game.ShoppingEngine.SelectedItem.Name);
        }

        [Fact]
        public void SelectShoppingItem_HandlesInvalidItem()
        {
            // Arrange: Assume no "Beef Jerky" item exists in inventory
            var invalidItemName = "Beef Jerky";

            // Act: Try selecting an invalid item
            _game.ShoppingEngine.SelectShoppingItem(invalidItemName);

            // Assert: The display should show an error message
            _mockDisplay.Verify(d => d.Write(It.Is<string>(msg => msg.Contains($"Hey, you old poophead, I ain't got no {invalidItemName} for sale"))), Times.Once);
        }
    }
}
