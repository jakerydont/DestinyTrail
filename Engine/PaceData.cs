namespace DestinyTrail.Engine
{
    public class PaceData : GameData<Pace>, ITravelSettingData
    {
        public required List<Pace> Paces { get => _items; set => _items = value; }
    }
}