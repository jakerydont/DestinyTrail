using DestinyTrail.Engine;

namespace DestinyTrail.Engine.Tests
{
    public class TestTypeData : GameData<TestType> {
        public required List<TestType> Tests { get => _items; set => _items = value; }


    }
}