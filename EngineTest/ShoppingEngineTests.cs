using System;
using Moq;
using Xunit;
using DestinyTrail.Engine;

namespace DestinyTrail.Engine.Tests
{
    public class ShoppingEngineTests
    {
        private readonly Mock<IDisplay> _mockDisplay;
       private readonly Mock<IInventory> _mockInventory;

       //private readonly Inventory Inventory;
        private readonly ShoppingEngine _shoppingEngine;

        public ShoppingEngineTests()
        {
            _mockDisplay = new Mock<IDisplay>();
            _mockInventory = new Mock<IInventory>();
            _mockInventory.Setup(i => i.Default).Returns(Inventory.Default);

            _shoppingEngine = new ShoppingEngine(_mockDisplay.Object, _mockInventory.Object);
            
        }

        [Fact]
        public void InitializeState_SetsInitialState()
        {
            _shoppingEngine.InitializeState();

            Assert.Equal(ShoppingState.Init, _shoppingEngine.ShoppingState);
            Assert.Equal(Inventory.Default, _shoppingEngine.SelectedItem);
            Assert.Equal(0, _shoppingEngine.Quantity);
        }


        [Fact]
        public void ShoppingLoop_Init_DisplaysWelcomeMessageAndUpdatesState()
        {
            _shoppingEngine.InitializeState();

            _shoppingEngine.ShoppingLoop();

            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Welcome to the store."))), Times.Once);
            Assert.Equal(ShoppingState.AskSelection, _shoppingEngine.ShoppingState);
        }

        [Fact]
        public void ShoppingLoop_AskSelection_DisplaysItemsAndUpdatesState()
        {
            _shoppingEngine.ShoppingState = ShoppingState.AskSelection;

            _shoppingEngine.ShoppingLoop();

            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.StartsWith("We have"))), Times.Once);
            Assert.Equal(ShoppingState.AwaitSelection, _shoppingEngine.ShoppingState);
        }

        [Fact]
        public void ProcessInput_ExitCommand_ChangesStateToLeave()
        {
            // Arrange
            _shoppingEngine.ShoppingState = ShoppingState.AskSelection;

            // Act
            _shoppingEngine.ProcessInput("exit");

            // Assert
            Assert.Equal(ShoppingState.Leave, _shoppingEngine.ShoppingState);
        }

        [Fact]
        public void ProcessInput_HelpCommand_DisplaysHelpMessage()
        {
            _shoppingEngine.ProcessInput("help");

            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Type what you want to buy"))), Times.Once);
        }

        [Fact]
        public void SelectShoppingItem_ValidInput_ChangesStateToAskQuantity()
        {
            _shoppingEngine.ShoppingState = ShoppingState.AwaitSelection;

            var item = new InventoryItem { Name = "Food" };
            _mockInventory.Setup(i => i.GetByName("Food")).Returns(item);

            _shoppingEngine.ProcessInput("Food");

            Assert.Equal(item, _shoppingEngine.SelectedItem);
            Assert.Equal(ShoppingState.AskQuantity, _shoppingEngine.ShoppingState);
        }

        [Fact]
        public void SelectShoppingItem_InvalidInput_DisplaysErrorMessage()
        {
            _shoppingEngine.ShoppingState = ShoppingState.AwaitSelection;
            _mockInventory.Setup(i => i.GetByName(It.IsAny<string>())).Throws<NullReferenceException>();

            _shoppingEngine.ProcessInput("NonExistent");

            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("I ain't got no \"NonExistent\" for sale"))), Times.Once);
        }

        [Fact]
        public void SelectQuantity_ValidNumber_ChangesStateToConfirmPurchase()
        {
            _shoppingEngine.ShoppingState = ShoppingState.AwaitQuantity;
            _shoppingEngine.SelectedItem = new InventoryItem { Name = "Food" };

            _shoppingEngine.ProcessInput("5");

            Assert.Equal(5, _shoppingEngine.Quantity);
            Assert.Equal(ShoppingState.ConfirmPurchase, _shoppingEngine.ShoppingState);
        }

        [Fact]
        public void SelectQuantity_InvalidInput_DisplaysErrorMessage()
        {
            _shoppingEngine.ShoppingState = ShoppingState.AwaitQuantity;

            _shoppingEngine.ProcessInput("not even a number");

            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("That ain't no number"))), Times.Once);
        }

        [Fact]
        public void GetConfirmation_Yes_WithSufficientFunds_ChangesStateToComplete()
        {
            // Arrange
            _mockInventory.Setup(i => i.Dollars.Subtract(It.IsAny<int>())).Returns(true);
            _mockInventory.Setup(i => i.GetByName(It.IsAny<string>())).Returns(new InventoryItem { Name = "Oxen" });
            _shoppingEngine.ShoppingState = ShoppingState.AwaitConfirm;

            // Act
            _shoppingEngine.ProcessInput("yes");

            // Assert
            Assert.Equal(ShoppingState.Complete, _shoppingEngine.ShoppingState);
        }

        [Fact]
        public void GetConfirmation_Yes_WithoutSufficientFunds_ChangesStateToAskSelection()
        {

            // Arrange
            var mockDollars = new Mock<IInventoryItem>();
            mockDollars.Setup(i => i.Name).Returns("Dollars");
            mockDollars.Setup(i => i.Quantity).Returns(5);
            mockDollars.Setup(i => i.Subtract(It.IsAny<int>())).Returns(false);
            mockDollars.As<IConvertible>().Setup(item => item.ToInt32(null)).Returns(() => mockDollars.Object.Quantity);

            _shoppingEngine.ShoppingState = ShoppingState.AwaitConfirm;
            _mockInventory.Setup(i => i.Dollars).Returns( mockDollars.Object );
            _mockInventory.Setup(i => i.GetByName("Oxen")).Returns(new InventoryItem { Name = "Oxen" });

            // Act
            _shoppingEngine.ProcessInput("yes");

            // Assert
            Assert.Equal(ShoppingState.AskSelection, _shoppingEngine.ShoppingState);
            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("commune"))), Times.Once);

        }

        [Fact]
        public void GetConfirmation_No_GoesToAskSelectionState()
        {
            // Arrange
            _shoppingEngine.ShoppingState = ShoppingState.AwaitConfirm;

            // Act
            _shoppingEngine.ProcessInput("no");

            // Assert
            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("Have it your way"))), Times.Once);
            Assert.Equal(ShoppingState.AskSelection, _shoppingEngine.ShoppingState);
        }

        [Fact]
        public void GetConfirmation_EmptyInput_AsksForAnswer()
        {
            // Arrange
            _shoppingEngine.ShoppingState = ShoppingState.AwaitConfirm;

            _shoppingEngine.ProcessInput("");

            _mockDisplay.Verify(d => d.Write(It.Is<string>(s => s.Contains("A deal ain't a deal unless you answer"))), Times.Once);
        }
    }

}