namespace DestinyTrail.Engine;

public class LandmarksData : GameData<Landmark>
{
    public List<Landmark> Landmarks { get => _items; set => _items = value; }
}
