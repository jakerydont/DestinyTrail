using System;

namespace DestinyTrail.Engine
{
    public interface ITravel
    {
        public IWagonParty Party { get;  set; }
            public IUtility Utility {get;set;}
        IOccurrenceEngine OccurrenceEngine { get; set; }
        Pace Pace { get; }
        Rations Rations { get; set; }

        void TravelLoop();
        void ContinueTravelling();

        double MilesToNextLandmark { get; set; }
        double MilesTraveled { get; set; }
        public Landmark NextLandmark { get; set; }

    }
}
