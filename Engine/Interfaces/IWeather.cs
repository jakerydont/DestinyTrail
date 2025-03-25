
namespace DestinyTrail.Engine.Interfaces;

public interface IWeather
{
    public WeatherDescription Description { get; set; }
    public int Temperature { get; set; }
   
}
