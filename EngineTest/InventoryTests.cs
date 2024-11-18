using Moq;
using Xunit;
using System.Collections.Generic;

namespace DestinyTrail.Engine.Tests
{
    public class InventoryTests
    {
        private readonly Inventory _inventory;

        public InventoryTests()
        {
            // Initialize Inventory with a mocked or real constructor depending on your needs.
            _inventory = new Inventory();
        }

        // Test Default Property
        [Fact]
        public void DefaultProperty_ReturnsCorrectItem()
        {
            // Act
            var defaultItem = Inventory.Default;

            // Assert
            Assert.Equal("none", defaultItem.Name);
        }

        // Test InventoryItem Properties (Oxen, Food, etc.)
        [Fact]
        public void OxenProperty_ReturnsCorrectItem()
        {
            // Setup
            var mockItems = new List<InventoryItem>
            {
                new InventoryItem { Name = "Oxen" },
                new InventoryItem { Name = "Food" }
            };
            _inventory.InventoryItems = mockItems;

            // Act
            var oxenItem = _inventory.Oxen;

            // Assert
            Assert.Equal("Oxen", oxenItem.Name);
        }

        [Fact]
        public void FoodProperty_ReturnsCorrectItem()
        {
            // Setup
            var mockItems = new List<InventoryItem>
            {
                new InventoryItem { Name = "Oxen" },
                new InventoryItem { Name = "Food" }
            };
            _inventory.InventoryItems = mockItems;

            // Act
            var foodItem = _inventory.Food;

            // Assert
            Assert.Equal("Food", foodItem.Name);
        }

        // Test CustomItems Property
        [Fact]
        public void CustomItemsProperty_SetsAndGetsCustomItems()
        {
            // Arrange
            var customItem = new InventoryItem { Name = "Custom Item" };

            // Act
            _inventory.CustomItems.Add(customItem);

            // Assert
            Assert.Contains(customItem, _inventory.CustomItems);
        }

        // Test if GetByName works properly
        [Fact]
        public void GetByName_ReturnsCorrectItem()
        {
            // Setup
            var mockItems = new List<InventoryItem>
            {
                new InventoryItem { Name = "Oxen" },
                new InventoryItem { Name = "Food" }
            };
            _inventory.InventoryItems = mockItems;

            // Act
            var item = _inventory.GetByName("Food");

            // Assert
            Assert.Equal("Food", item.Name);
        }

        [Fact]
        public void GetByName_ReturnsNull_WhenItemDoesNotExist()
        {
            // Setup
            var mockItems = new List<InventoryItem>
            {
                new InventoryItem { Name = "Oxen" }
            };
            _inventory.InventoryItems = mockItems;

            // Act
            var item = _inventory.GetByName("Hot Sauce");

            // Assert
            Assert.Null(item);
        }

        // Additional tests can be added based on functionality (e.g., handling specific edge cases, or when CustomItems are empty).
    }
}
