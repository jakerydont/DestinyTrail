using System.Collections.Generic;

namespace DestinyTrail.Engine
{
    public class StatusData : GameData<string>
    {
        public required List<string> Statuses{ get => _items; set => _items = value; }
        
    }
}
