namespace DestinyTrail.Engine.Tests;

public class GameComponentTests
{
    [Fact]
    public void Name_Property_SetAndGet_ReturnsExpectedValue()
    {
        // Arrange
        var mockComponent = new Mock<GameComponent>() { CallBase = true };
        mockComponent.SetupProperty(c => c.Name, "TestComponent");

        // Act
        var name = mockComponent.Object.Name;

        // Assert
        Assert.Equal("TestComponent", name);
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        // Arrange
        var mockComponent = new Mock<GameComponent>() { CallBase = true };
        mockComponent.Setup(c => c.Name).Returns("TestComponent");

        // Act
        var result = mockComponent.Object.ToString();

        // Assert
        Assert.Equal("TestComponent", result);
    }

    [Fact]
    public void ToLower_ReturnsLowercaseName()
    {
        // Arrange
        var mockComponent = new Mock<GameComponent>() { CallBase = true };
        mockComponent.Setup(c => c.Name).Returns("TestComponent");

        // Act
        var result = mockComponent.Object.ToLower();

        // Assert
        Assert.Equal("testcomponent", result);
    }

    [Fact]
    public void ImplicitOperator_ReturnsNameAsString()
    {
        // Arrange
        var mockComponent = new Mock<GameComponent>() { CallBase = true };
        mockComponent.Setup(c => c.Name).Returns("TestComponent");

        // Act
        string result = mockComponent.Object;

        // Assert
        Assert.Equal("TestComponent", result);
    }

    [Fact]
    public void GetHashCode_ReturnsSameValue_ForSameName()
    {
        // Arrange
        var mockComponent1 = new Mock<GameComponent>() { CallBase = true };
        mockComponent1.Setup(c => c.Name).Returns("TestComponent");

        var mockComponent2 = new Mock<GameComponent>() { CallBase = true };
        mockComponent2.Setup(c => c.Name).Returns("TestComponent");

        // Act
        var hash1 = mockComponent1.Object.GetHashCode();
        var hash2 = mockComponent2.Object.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentValue_ForDifferentNames()
    {
        // Arrange
        var mockComponent1 = new Mock<GameComponent>() { CallBase = true };
        mockComponent1.Setup(c => c.Name).Returns("ComponentOne");

        var mockComponent2 = new Mock<GameComponent>() { CallBase = true };
        mockComponent2.Setup(c => c.Name).Returns("ComponentTwo");

        // Act
        var hash1 = mockComponent1.Object.GetHashCode();
        var hash2 = mockComponent2.Object.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }
}
