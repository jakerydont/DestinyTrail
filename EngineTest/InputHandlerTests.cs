using Xunit;
using Moq;

namespace DestinyTrail.Engine.Tests
{
    public class InputHandlerTests
    {
        private readonly Mock<IGame> _mockGame;
        private readonly Mock<ITravel> _mockTravel;
        private readonly Mock<IShoppingEngine> _mockShoppingEngine;
        private readonly InputHandler _inputHandler;

        public InputHandlerTests()
        {
            _mockGame = new Mock<IGame>();
            _mockTravel = new Mock<ITravel>();
            _mockShoppingEngine = new Mock<IShoppingEngine>();

            _mockGame.SetupGet(g => g.travel).Returns(_mockTravel.Object);
            _mockGame.SetupGet(g => g.ShoppingEngine).Returns(_mockShoppingEngine.Object);

            _inputHandler = new InputHandler(_mockGame.Object);
        }

        [Fact]
        public void ProcessCommand_EmptyInput_AtLandmark_ContinuesTravel()
        {
            // Arrange
            _mockGame.SetupGet(g => g.GameMode).Returns(Modes.AtLandmark);

            // Act
            _inputHandler.ProcessCommand("");

            // Assert
            _mockTravel.Verify(t => t.ContinueTravelling(), Times.Once);
            _mockGame.Verify(g => g.ChangeMode(It.IsAny<Modes>()), Times.Never);
            _mockShoppingEngine.Verify(s => s.ProcessInput(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ProcessCommand_BuyInput_AtLandmark_ChangesToShoppingMode()
        {
            // Arrange
            _mockGame.SetupGet(g => g.GameMode).Returns(Modes.AtLandmark);

            // Act
            _inputHandler.ProcessCommand("buy");

            // Assert
            _mockGame.Verify(g => g.ChangeMode(Modes.Shopping), Times.Once);
            _mockTravel.Verify(t => t.ContinueTravelling(), Times.Never);
            _mockShoppingEngine.Verify(s => s.ProcessInput(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ProcessCommand_ShoppingMode_ProcessesShoppingInput()
        {
            // Arrange
            _mockGame.SetupGet(g => g.GameMode).Returns(Modes.Shopping);
            _mockShoppingEngine.SetupGet(s => s.ShoppingState).Returns(ShoppingState.AwaitSelection);
            // Act
            _inputHandler.ProcessCommand("oxen");

            // Assert
            _mockShoppingEngine.Verify(s => s.ProcessInput("oxen"), Times.Once);
            _mockTravel.Verify(t => t.ContinueTravelling(), Times.Never);
            _mockGame.Verify(g => g.ChangeMode(It.IsAny<Modes>()), Times.Never);
        }

        [Fact]
        public void ProcessCommand_UnknownCommand_AtLandmark_NoAction()
        {
            // Arrange
            _mockGame.SetupGet(g => g.GameMode).Returns(Modes.AtLandmark);

            // Act
            _inputHandler.ProcessCommand("unknown");

            // Assert
            _mockTravel.Verify(t => t.ContinueTravelling(), Times.Never);
            _mockGame.Verify(g => g.ChangeMode(It.IsAny<Modes>()), Times.Never);
            _mockShoppingEngine.Verify(s => s.ProcessInput(It.IsAny<string>()), Times.Never);
        }
    }
}
