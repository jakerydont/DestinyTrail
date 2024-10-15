using System.Collections.Generic;

namespace DestinyTrail.Engine
{
    public class Pace : ITravelSetting
    {
        public required string Name { get; set; }
        public int Factor { get; set; }
    }
}
