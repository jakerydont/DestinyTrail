using DestinyTrail.Engine;

namespace DestinyTrail.EngineTest
{
    public class TestTypeData : GameData<TestType> {
        public required List<TestType> Tests { get => _items; set => _items = value; }


    }
}