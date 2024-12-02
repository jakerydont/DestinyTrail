using System;
using System.Collections.Generic;
using DestinyTrail.Engine;
using Xunit;

namespace DestinyTrail.Engine.Tests;

public class GameDataTests
{

    private class ConcreteGameData : GameData<ConcreteGameComponent>
    {
        public ConcreteGameData() : this(new Mock<IDisplay>().Object) { }
        public ConcreteGameData(IDisplay display) : base(display) { }
    }

    private class ConcreteGameComponent : GameComponent { }

    private ConcreteGameData _gameData;

    public GameDataTests()
    {
        _gameData = new ConcreteGameData();
    }

    [Fact]
    public void TestGetByName()
    {
        var component = new ConcreteGameComponent { Name = "Test" };
        _gameData.Add(component);

        var result = _gameData.GetByName("Test");

        Assert.Equal(component, result);
    }

    [Fact]
    public void TestTryGetByName_Success()
    {
        var component = new ConcreteGameComponent { Name = "Test" };
        _gameData.Add(component);

        var success = _gameData.TryGetByName("Test", out var result);

        Assert.True(success);
        Assert.Equal(component, result);
    }

    [Fact]
    public void TestTryGetByName_Failure()
    {
        var success = _gameData.TryGetByName("Item that does not exist", out var result);

        Assert.False(success);
        Assert.Equal(GameData<ConcreteGameComponent>.Default, result);
    }

    [Fact]
    public void TestGetByPropertyValue()
    {
        var component = new ConcreteGameComponent { Name = "Test" };
        _gameData.Add(component);

        var result = _gameData.GetByPropertyValue("Name", "Test");

        Assert.Equal(component, result);
    }

    [Fact]
    public void TestGetByPropertyValue_InvalidProperty()
    {
        Assert.Throws<ArgumentException>(() => _gameData.GetByPropertyValue("InvalidProperty", "Test"));
    }

    [Fact]
    public void TestFirstOrDefaultSafe()
    {
        var component = new ConcreteGameComponent { Name = "Test" };
        _gameData.Add(component);

        var result = _gameData.FirstOrDefaultSafe();

        Assert.Equal(component, result);
    }

    [Fact]
    public void TestFirstOrDefaultSafe_Empty()
    {
        Assert.Equal(GameData<ConcreteGameComponent>.Default, _gameData.FirstOrDefaultSafe());
    }

    [Fact]
    public void TestLastOrDefaultSafe()
    {
        var component = new ConcreteGameComponent { Name = "Test" };
        _gameData.Add(component);

        var result = _gameData.LastOrDefaultSafe();

        Assert.Equal(component, result);
    }

    [Fact]
    public void TestLastOrDefaultSafe_Empty()
    {
        Assert.Equal(GameData<ConcreteGameComponent>.Default, _gameData.LastOrDefaultSafe());
    }

    [Fact]
    public void TestMinBy()
    {
        var component1 = new ConcreteGameComponent { Name = "A" };
        var component2 = new ConcreteGameComponent { Name = "B" };
        _gameData.Add(component1);
        _gameData.Add(component2);

        var result = _gameData.MinBy(c => c.Name);

        Assert.Equal(component1, result);
    }

    [Fact]
    public void TestMaxBy()
    {
        var component1 = new ConcreteGameComponent { Name = "A" };
        var component2 = new ConcreteGameComponent { Name = "B" };
        _gameData.Add(component1);
        _gameData.Add(component2);

        var result = _gameData.MaxBy(c => c.Name);

        Assert.Equal(component2, result);
    }

    [Fact]
    public void TestToArray()
    {
        var component1 = new ConcreteGameComponent { Name = "A" };
        var component2 = new ConcreteGameComponent { Name = "B" };
        _gameData.Add(component1);
        _gameData.Add(component2);

        var result = _gameData.ToArray();

        Assert.Equal(new[] { component1, component2 }, result);
    }
}