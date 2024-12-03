
namespace DestinyTrail.Engine
{
    public class WorldStatus : IWorldStatus
    {
        public DateTime CurrentDate { get; set; }
        public IWeather Weather { get; set; }

        public WorldStatus()
        {
            Weather = new Weather();
        }
    }
}