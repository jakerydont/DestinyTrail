using Xunit;

namespace DestinyTrail.Engine.Tests;

public class InventoryItemTests
{
    [Fact]
    public void Quantity_SetNegative_SetsToZero()
    {
        // Arrange
        var item = new InventoryItem() {Name="MyItem"};

        // Act
        item.Quantity = -5;

        // Assert
        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public void Add_IncreasesQuantity()
    {
        // Arrange
        var item = new InventoryItem() {Name="MyItem", Quantity = 10 };

        // Act
        item.Add(5);

        // Assert
        Assert.Equal(15, item.Quantity);
    }

    [Fact]
    public void Subtract_DecreasesQuantity_WhenEnoughAvailable()
    {
        // Arrange
        var item = new InventoryItem() {Name="MyItem", Quantity = 10 };

        // Act
        var result = item.Subtract(5);

        // Assert
        Assert.True(result);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void Subtract_DoesNotDecreaseQuantity_WhenNotEnoughAvailable()
    {
        // Arrange
        var item = new InventoryItem() { Name="MyItem", Quantity = 3 };

        // Act
        var result = item.Subtract(5);

        // Assert
        Assert.False(result);
        Assert.Equal(3, item.Quantity);
    }

    [Fact]
    public void SingularOrPluralName_ReturnsSingular_WhenQuantityIsOne()
    {
        // Arrange
        var item = new InventoryItem
        {
            NameSingular = "Ox",
            Name = "Oxen"
        };

        // Act
        var result = item.SingularOrPluralName(1);

        // Assert
        Assert.Equal("Ox", result);
    }

    [Fact]
    public void SingularOrPluralName_ReturnsPlural_WhenQuantityIsMoreThanOne()
    {
        // Arrange
        var item = new InventoryItem
        {
            NameSingular = "Ox",
            Name = "Oxen"
        };

        // Act
        var result = item.SingularOrPluralName(2);

        // Assert
        Assert.Equal("Oxen", result);
    }

    [Fact]
    public void ImplicitOperator_ReturnsQuantity()
    {
        // Arrange
        var item = new InventoryItem() {Name="MyItem", Quantity = 42 };

        // Act
        int quantity = item;

        // Assert
        Assert.Equal(42, quantity);
    }

    [Fact]
    public void Unit_ReturnsEmptyString_WhenNotSet()
    {
        // Arrange
        var item = new InventoryItem() {Name="MyItem"};

        // Act
        var result = item.Unit;

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void Unit_ReturnsFormattedString_WhenSet()
    {
        // Arrange
        var item = new InventoryItem { Name = "My Item", Unit = "kg" };

        // Act
        var result = item.Unit;

        // Assert
        Assert.Equal("kg of ", result);
    }

    [Fact]
    public void GetLore_ReturnsLore()
    {
        // Arrange
        var item = new InventoryItem() {Name="MyItem"};
        var lore = "This is a legendary item.";
        
        typeof(InventoryItem).GetProperty("Lore", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(item, lore);

        // Act
        var result = item.GetLore();

        // Assert
        Assert.Equal(lore, result);
    }
}
