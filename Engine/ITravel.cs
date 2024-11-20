using System;

namespace DestinyTrail.Engine
{
    public interface ITravel
    {
        string[] Statuses { get; }
        OccurrenceEngine OccurrenceEngine { get; set; }
        Pace Pace { get; }
        Rations Rations { get; set; }

        void TravelLoop();
        void ContinueTravelling();
    }
}
