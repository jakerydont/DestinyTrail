
namespace DestinyTrail.Engine;

public interface IWeather
{
    public WeatherDescription Description { get; set; }
    public int Temperature { get; set; }
   
}
