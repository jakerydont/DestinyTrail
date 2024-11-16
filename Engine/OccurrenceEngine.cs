using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DestinyTrail.Engine
{
    public class OccurrenceEngine
    {
        private readonly Occurrence[] _occurrences;
        private readonly string[] _statuses;
        private readonly WagonParty _party ;
        private readonly Utility Utility;

        public OccurrenceEngine(string yamlFilePath,  WagonParty party, string[] statuses )
        {
            _occurrences = LoadOccurrences(yamlFilePath);
            _statuses = statuses;
            _party = party;
            Utility = new Utility();

        }

        private Occurrence[] LoadOccurrences(string yamlFilePath)
        {
            return Utility.LoadYaml<OccurrenceData>(yamlFilePath);
        }


        public Occurrence PickRandomOccurrence()
        {
            double totalProbability = _occurrences.Sum(o => o.Probability);
            Random random = new Random();
            double randomValue = random.NextDouble() * totalProbability;

            foreach (var occurrence in _occurrences)
            {
                if (randomValue < occurrence.Probability)
                {
                    return occurrence;
                }
                randomValue -= occurrence.Probability;
            }

            return _occurrences.Last();
        }

        public Occurrence ProcessOccurrence(Occurrence occurrence)
        {
            var person = _party.GetRandomMember();

            if (occurrence.DisplayText.Contains("{name}"))
            {
                occurrence.DisplayText = occurrence.DisplayText.Replace("{name}", person.Name);
                person.Status = new Status { Name = occurrence.Effect }; 
                //Display.Write($"\n{person.Name}: {person.Status.Name}");
            }

            return occurrence;
        }



    }
}
