namespace DestinyTrail.Engine {
    public class RandomNamesData : GameData<string>
    {
         public required List<string> RandomNames { get => _items; set => _items = value; }
    }
}