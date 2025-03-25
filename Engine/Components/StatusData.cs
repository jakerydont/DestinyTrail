using System.Collections.Generic;

namespace DestinyTrail.Engine;

public class StatusData : GameData<Status>
{
    public required List<Status> Statuses{ get => _items; set => _items = value; }
    public StatusData()
    {
        
    }
}
