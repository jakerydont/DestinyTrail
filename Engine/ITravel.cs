using System;

namespace DestinyTrail.Engine
{
    public interface ITravel
    {
        IOccurrenceEngine OccurrenceEngine { get; set; }
        Pace Pace { get; }
        Rations Rations { get; set; }

        void TravelLoop();
        void ContinueTravelling();
    }
}
