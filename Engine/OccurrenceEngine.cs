using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DestinyTrail.Engine
{
    public class OccurrenceEngine : IOccurrenceEngine
    {
        private readonly Occurrence[] _occurrences;
        private readonly string[] _statuses;
        public string[] Statuses { get; private set; }


        private readonly IWagonParty _party;
        private readonly IUtility Utility;


        private static Regex DecrementEventPattern = new Regex(@"\[(.*?)\] -= (\d+)");
        private static Regex IncrementEventPattern = new Regex(@"\[(.*?)\] \+= (\d+)");
        public static Regex SetItemQuantityPattern = new Regex(@"\[(.*?)\] = (\d+)");
        private static Regex BooleanEventPattern = new Regex(@"\[Flags\.(.*?)\] = (true|false)");

        public OccurrenceEngine(string yamlFilePath, IWagonParty party) : this(yamlFilePath, party, new Utility()) { }
        public OccurrenceEngine(string yamlFilePath, IWagonParty party, IUtility utility)
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
                TryProcessEffect(occurrence, person);

            }
            else
            {
                TryProcessEffect(occurrence);

            }

            return occurrence;
        }

        private void TryProcessEffect(IOccurrence occurrence)
        {
            TryProcessEffect(occurrence, Person.Nobody);
        }
        private void TryProcessEffect(IOccurrence occurrence, IPerson person)
        {
            if (person.ID != Person.Nobody.ID)
            {
                person.Status = new Status { Name = occurrence.Effect };
                if (occurrence.Effect.ToLower() == "dead")
                {
                    _party.KillMember(person);
                }
            }

            else if (IncrementEventPattern.Match(occurrence.Effect).Success)
            {
                TryIncreaseInventoryItem(occurrence);
            }
            else if (DecrementEventPattern.Match(occurrence.Effect).Success)
            {
                TryDecreaseInventoryItem(occurrence);
            }
            else if (SetItemQuantityPattern.Match(occurrence.Effect).Success)
            {
                TryZeroInventoryItem(occurrence);
            }
            else if (BooleanEventPattern.Match(occurrence.Effect).Success)
            {
                TrySetFlag(occurrence);
            }
        }

        public void TrySetFlag(IOccurrence occurrence)
        {
            var booleanMatch = BooleanEventPattern.Match(occurrence.Effect);
            if (!booleanMatch.Success)
            {
                throw new Exception($"Bad boolean setting on occurrence {occurrence.DisplayText}. Must be in the form '[Flags.flag] = true' or '[Flags.flag] = false'. Actual: {occurrence.Effect}");
            }

            var flag = booleanMatch.Groups[1].Value;
            var value = booleanMatch.Groups[2].Value;
            if (value.ToLower() == "true")
            {
                _party.Flags[flag] = true;
            }
            else if (value.ToLower() == "false")
            {
                _party.Flags[flag] = false;
            }
            else
            {
                throw new Exception($"Bad boolean setting on occurrence {occurrence.DisplayText}. Must be in the form '[Flags.flag] = true' or '[Flags.flag] = false'. Actual: {occurrence.Effect}");
            }
        }

        private void TrySetBoolean(IOccurrence occurrence)
        {
            var validate = new System.Text.RegularExpressions.Regex(@"\[(.*?)\] = false");
            var itemBooleanMatch = validate.Match(occurrence.Effect);
            if (!itemBooleanMatch.Success)
            {
                throw new Exception($"Bad boolean set on occurrence {occurrence.DisplayText}. Must be in the form '[item] = false'. Actual: {occurrence.Effect}");
            }

            var item = _party.Inventory.GetByName(itemBooleanMatch.Groups[1].Value);
            if (item == null)
            {
                throw new Exception($"Inventory item {itemBooleanMatch.Groups[1].Value} not found.");
            }
            item.SetBoolean(false);
        }

        private void TrySet InventoryItem(IOccurrence occurrence)
        {
            var validate = new System.Text.RegularExpressions.Regex(@"\[(.*?)\] = 0");
            var itemZeroMatch = validate.Match(occurrence.Effect);
            if (!itemZeroMatch.Success)
            {
                throw new Exception($"Bad zeroing on occurrence {occurrence.DisplayText}. Must be in the form '[item] = 0'. Actual: {occurrence.Effect}");
            }

            var item = _party.Inventory.GetByName(itemZeroMatch.Groups[1].Value);
            if (item == null)
            {
                throw new Exception($"Inventory item {itemZeroMatch.Groups[1].Value} not found.");
            }
            item.SetQuantity(0);
        }

        public void TryZeroInventoryItem(IOccurrence occurrence)
        {
            var itemZeroMatch = SetItemQuantityPattern.Match(occurrence.Effect);
            if (!itemZeroMatch.Success)
            {
                throw new Exception($"Bad zero on occurrence {occurrence.DisplayText}. Must be in the form '[item] = 0'. Actual: {occurrence.Effect}");
            }

            var item = _party.Inventory.GetByName(itemZeroMatch.Groups[1].Value);
            if (item == null)
            {
                throw new Exception($"Inventory item {itemZeroMatch.Groups[1].Value} not found.");
            }
            item.Quantity = 0;
        }

        public void TryDecreaseInventoryItem(IOccurrence occurrence)
        {

            var itemDecrementMatch = DecrementEventPattern.Match(occurrence.Effect);
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

        public void TryIncreaseInventoryItem(IOccurrence occurrence)
        {
            var itemIncrementMatch = IncrementEventPattern.Match(occurrence.Effect);
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



        void IOccurrenceEngine.TryIncreaseInventoryItem(IOccurrence occurrence) => TryIncreaseInventoryItem(occurrence);
    }
}
