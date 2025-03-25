using System;
using DestinyTrail.Engine;
using Xunit;

namespace DestinyTrail.Engine.Tests;

public class DisplayTest
{
    [Fact]
    public async Task Write_ShouldOutputMessage()
    {
        // Arrange
        var display = new Display();
        var message = "Hello, World!";
        using var sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        await display.Write(message);

        // Assert
        var result = sw.ToString().Trim();
        Assert.Equal(message, result);
    }

    [Fact]
    public async Task WriteTitle_ShouldOutputFormattedTitle()
    {
        // Arrange
        var display = new Display();
        var message = "Title";
        using var sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        await display.WriteTitle(message);

        // Assert
        var result = sw.ToString().Trim();
        var expected = $"+-------+\n| Title |\n+-------+";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void BuildConsoleTitle_ShouldReturnFormattedTitle()
    {
        // Arrange
        var display = new Display();
        var message = "Title";

        // Act
        var result = display.BuildConsoleTitle(message);

        // Assert
        var expected = $"+-------+\n| Title |\n+-------+";
        Assert.Equal(expected, result);
    }
}