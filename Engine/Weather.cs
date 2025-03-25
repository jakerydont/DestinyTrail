namespace DestinyTrail.Engine;

internal class Weather : IWeather
{
    public WeatherDescription Description { get; set; }
    public int Temperature { get; set; }
}