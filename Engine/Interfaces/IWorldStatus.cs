namespace DestinyTrail.Engine.Interfaces;

public interface IWorldStatus
{
    DateTime CurrentDate { get; set; }
    IWeather Weather { get; set; }
}
