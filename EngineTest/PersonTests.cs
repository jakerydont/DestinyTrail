using Xunit;
using Moq;

namespace DestinyTrail.Engine.Tests;

public class PersonTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var person = new Person
        {
            ID = 1,
            Name = "Test Person",
            Status = new Status { Name = "Healthy" }
        };

        // Act & Assert
        Assert.Equal(1, person.ID);
        Assert.Equal("Test Person", person.Name);
        Assert.Equal("Healthy", person.Status.Name);
    }

    [Fact]
    public void IsAlive_ShouldReturnTrueIfStatusIsNotDead()
    {
        // Arrange
        var person = new Person
        {
            ID = 1,
            Name = "Test Person",
            Status = new Status { Name = "Healthy" }
        };

        // Act & Assert
        Assert.True(person.isAlive);
    }

    [Fact]
    public void IsAlive_ShouldReturnFalseIfStatusIsDead()
    {
        // Arrange
        var person = new Person
        {
            ID = 1,
            Name = "Test Person",
            Status = Status.Dead
        };

        // Act & Assert
        Assert.False(person.isAlive);
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var person = new Person
        {
            ID = 42,
            Name = "Jane Doe",
            Status = new Status(){Name="Good"}
        };

        // Act
        var result = person.ToString();

        // Assert
        Assert.Equal("Jane Doe (42)", result);
    }

    [Fact]
    public void SetStatus_UpdatesStatusCorrectly()
    {
        // Arrange
        var person = new Person
        {
            ID = 2,
            Name = "John Smith",
            Status = new Status { Name = "Healthy" }
        };

        // Act
        person.Status = new Status { Name = "Sick" };

        // Assert
        Assert.Equal("Sick", person.Status.Name);
    }

    [Fact]
    public void InheritsFromGameComponent()
    {
        // Arrange
        var person = new Person
        {
            ID = 3,
            Name = "Test Person",
            Status = new Status { Name = "Healthy" }
        };

        // Act
        string name = person; // Implicit conversion from GameComponent to string

        // Assert
        Assert.Equal("Test Person", name);
    }

    [Fact]
    public void GetHashCode_UsesNameAndID()
    {
        // Arrange
        var person = new Person
        {
            ID = 5,
            Name = "HashTest Person",
            Status = new Status { Name = "Healthy" }
        };

        // Act
        var hashCode = person.GetHashCode();

        // Assert
        Assert.Equal(HashCode.Combine("HashTest Person"), hashCode);
    }

    [Fact]
    public void Equality_UsesNameAndID()
    {
        // Arrange
        var person1 = new Person
        {
            ID = 10,
            Name = "Person A",
            Status = new Status { Name = "Healthy" }
        };

        var person2 = new Person
        {
            ID = 10,
            Name = "Person A",
            Status = new Status { Name = "Sick" }
        };

        var person3 = new Person
        {
            ID = 20,
            Name = "Person B",
            Status = new Status { Name = "Healthy" }
        };

        // Act & Assert
        Assert.Equal(person1.GetHashCode(), person2.GetHashCode());
        Assert.NotEqual(person1.GetHashCode(), person3.GetHashCode());
    }
}
