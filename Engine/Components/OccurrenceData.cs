namespace DestinyTrail.Engine;

public class OccurrenceData : GameData<Occurrence>
{
    public List<Occurrence> Occurrences { get => _items; set => _items = value; }
    public OccurrenceData()
    {

    }
    
}
