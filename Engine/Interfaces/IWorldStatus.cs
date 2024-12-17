
namespace DestinyTrail.Engine;


public interface IWorldStatus

{

    DateTime CurrentDate { get; set; }

    IWeather Weather { get; set; }

}

