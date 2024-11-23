using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DestinyTrail.Engine
{
    public class OccurrenceEngine : IOccurrenceEngine
    {
        private readonly Occurrence[] _occurrences;
        private readonly string[] _statuses;
        public string[] Statuses { get; private set; }
        private readonly IWagonParty _party ;
        private readonly IUtility Utility;

        public OccurrenceEngine(string yamlFilePath,  IWagonParty party) : this(yamlFilePath,party, new Utility()) {}
        public OccurrenceEngine(string yamlFilePath,  IWagonParty party, IUtility utility) 
        { 
            Utility = utility;

            string statusesFilePath = "data/Statuses.yaml"; 
            Statuses = [.. Utility.LoadYaml<StatusData>(statusesFilePath)];     
            _statuses = Statuses;

            _occurrences = LoadOccurrences(yamlFilePath);

            _party = party;
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
                TryProcessEffect(person, occurrence);
                //Display.Write($"\n{person.Name}: {person.Status.Name}");
            }

            return occurrence;
        }

// Effects
// ""
// "Broken Leg"
// "Delay"
// "Dysentery"
// "Snakebite"
// "Dead"
// "Cholera"
// "Snakebite"
// "Type 2 Diabetes"
// "Dysentery"
// "Yellow Fever"
// "Pregnant"
// "???"
// "[oxen] = 0"
// "Consumption"
// "constipation"
// "Dead,[Food] += 20"
// "measles"
// "John Denver"
// "[BajaBlast] = 0"
// "???"
// "[canHunt] = false"
// "???"
// "missing presumed dead"
// "missing presumed getting to 3rd base"
// "???"
// "???"
// "Broken Foot"
// "???"
// "Hand Foot and Mouth Disease"
// "[clothes] -= 1"
// "[horse] -= 1,[glue] += 1"
// "???"
// "homesick"
// "COVID-19"
// "Dead"
// "Oxygen Allergy"
// "Blind"
// "Fridged"
// "Ears Boxed"
        public void TryProcessEffect(IPerson person, IOccurrence occurrence)
        {
            person.Status = new Status { Name = occurrence.Effect };
            if (occurrence.Effect == "Dead")
            {
                _party.KillMember(person);
            } 
            else if (occurrence.Effect.Contains("+="))
            {
                TryIncreaseInventoryItem(occurrence);
            }
            else if (occurrence.Effect.Contains("-="))
            {
                TryDecreaseInventoryItem(occurrence);
            }
            else if (occurrence.Effect.Contains("= 0"))
            {
                TryZeroInventoryItem(occurrence);
            }
            else if (occurrence.Effect.Contains("= false"))
            {
                TrySetBoolean(occurrence);
            }


        }

        private void TryDecreaseInventoryItem(IOccurrence occurrence)
        {
            var validate = new System.Text.RegularExpressions.Regex(@"\[(.*?)\] -= (\d+)");
            var itemDecrementMatch = validate.Match(occurrence.Effect);
            if (!itemDecrementMatch.Success)
            {
                throw new Exception($"Bad decrement on occurrence {occurrence.DisplayText}. Must be in the form '[item] -= 1'. Actual: {occurrence.Effect}");
            }

            var item = _party.Inventory.GetByName(itemDecrementMatch.Groups[1].Value);
            if (item == null)
            {
                throw new Exception($"Inventory item {itemDecrementMatch.Groups[1].Value} not found.");
            }
            var amount = int.Parse(itemDecrementMatch.Groups[2].Value);
            var success = item.Subtract(amount);
            if (!success)
            {
                throw new Exception($"Not enough {item.Name} to subtract {amount}.");
            }
        }

        void TryIncreaseInventoryItem(IOccurrence occurrence)
        {
            var validate = new System.Text.RegularExpressions.Regex(@"\[(.*?)\] \+= (\d+)");
            var itemIncrementMatch = validate.Match(occurrence.Effect);
            if (!itemIncrementMatch.Success)
            {
                throw new Exception($"Bad increment on occurrence {occurrence.DisplayText}. Must be in the form '[item] += 1'. Actual: {occurrence.Effect}");
            }

            var item = _party.Inventory.GetByName(itemIncrementMatch.Groups[1].Value);
            if (item == null)
            {
                throw new Exception($"Inventory item {itemIncrementMatch.Groups[1].Value} not found.");
            }
            var amount = int.Parse(itemIncrementMatch.Groups[2].Value);
            item.Add(amount);
        }

        public void SetStatus(string status)
        {
            throw new NotImplementedException();
        }

        public void ClearStatus()
        {
            throw new NotImplementedException();
        }

        public void TryProcessEffect(string effectText)
        {
            throw new NotImplementedException();
        }
    }
}
