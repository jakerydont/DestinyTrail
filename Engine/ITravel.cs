using System;

namespace DestinyTrail.Engine
{
    public interface ITravel
    {
        string[] Statuses { get; }
        IOccurrenceEngine OccurrenceEngine { get; set; }
        Pace Pace { get; }
        Rations Rations { get; set; }

        void TravelLoop();
        void ContinueTravelling();
    }
}
