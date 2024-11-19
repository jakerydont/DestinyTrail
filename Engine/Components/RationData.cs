namespace DestinyTrail.Engine
{
    public class RationData : GameData<Rations>
    {
        public required List<Rations> Rations { get => _items; set => _items = value; }
    }
}