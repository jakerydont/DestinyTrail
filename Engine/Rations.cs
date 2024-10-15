namespace DestinyTrail.Engine
{
    public class Rations : ITravelSetting
    {
        public required string Name { get ; set; }
        public int Factor { get; set; }
    }
}