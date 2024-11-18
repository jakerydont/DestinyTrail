using System;
using Xunit;

namespace DestinyTrail.Engine.Tests
{
    public class InventoryTests
    {
        public InventoryTests()
        {
            
        }

        [Fact]
        public void Constructor_ShouldCreateInventory() {
            var inventory = new DestinyTrail.Engine.Inventory();
        }
    }
}