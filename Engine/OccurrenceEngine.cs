
using System.Text.RegularExpressions;
using System.Configuration;

namespace DestinyTrail.Engine
{
    public class OccurrenceEngine : IOccurrenceEngine
    {

        public static OccurrenceEngine Default = new OccurrenceEngine(WagonParty.Default);
        public string[] Statuses { get; private set; }

        private readonly IWagonParty _party;
        private readonly IUtility _utility;
        private Occurrence[] _occurrences;
        private string[] _statuses;

        private static Regex DecrementEventPattern = new Regex(@"\[(.*?)\] -= (\d+)");
        private static Regex IncrementEventPattern = new Regex(@"\[(.*?)\] \+= (\d+)");
        public static Regex SetItemQuantityPattern = new Regex(@"\[(.*?)\] = (\d+)");
        private static Regex BooleanEventPattern = new Regex(@"\[Flags\.(.*?)\] = (true|false)");

        public static async Task<OccurrenceEngine> CreateAsync(IWagonParty party, IUtility utility)
        {
            var oe = new OccurrenceEngine(party, utility);

            await oe.GetStatusesAsync();
            await oe.LoadOccurrencesAsync();

            return oe;
        }


        private async Task GetStatusesAsync()
        {
            string statusesFilePath = _utility.GetAppSetting("StatusesFilePath");
            var statusYaml = await _utility.LoadYamlAsync<StatusData>(statusesFilePath);
            Statuses = [.. statusYaml];
            _statuses = Statuses;
        }

        private OccurrenceEngine(IWagonParty party) : this(party, new Utility()) { }
        private OccurrenceEngine(IWagonParty party, IUtility utility)
        {
            Statuses= [];
            _occurrences = [];
            _statuses = [];
            _utility = utility;
            _party = party;
        }

        private async Task LoadOccurrencesAsync()
        {
            var yamlFilePath = _utility.GetAppSetting("OccurrencesFilePath");
            _occurrences = await _utility.LoadYamlAsync<OccurrenceData>(yamlFilePath);

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

        private void TryProcessEffect(IOccurrence occurrence) => TryProcessEffect(occurrence, Person.Nobody);
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
                TrySetQuantityInventoryItem(occurrence);
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

        public void TrySetQuantityInventoryItem(IOccurrence occurrence)
        {
            var validate = new System.Text.RegularExpressions.Regex(@"\[(.*?)\] = (\d+)");
            var itemZeroMatch = validate.Match(occurrence.Effect);
            if (!itemZeroMatch.Success)
            {
                throw new Exception($"Bad zeroing on occurrence {occurrence.DisplayText}. Must be in the form '[item] = amount'. Actual: {occurrence.Effect}");
            }

            var item = _party.Inventory.TryGetByName(itemZeroMatch.Groups[1].Value, out var itemResult) ? itemResult : null;
            if (item == null) {
                throw new Exception($"Inventory item {itemZeroMatch.Groups[1].Value} not found.");

               // Console.WriteLine($"Inventory item {itemZeroMatch.Groups[1].Value} not found.");
               // return;
            }
                
            
            int? amount = int.TryParse(itemZeroMatch.Groups[2].Value, out var amountResult) ? amountResult 
                : null;
            if (amount == null) {
                throw new Exception($"Invalid amount {itemZeroMatch.Groups[2].Value}.");
                //Console.WriteLine($"Invalid amount {itemZeroMatch.Groups[2].Value}.");
               // return;
            }


            item.SetQuantity((int)amount);
        }



        public void TryDecreaseInventoryItem(IOccurrence occurrence)
        {

            var itemDecrementMatch = DecrementEventPattern.Match(occurrence.Effect);
            if (!itemDecrementMatch.Success)
            {
                //throw new Exception($"Bad decrement on occurrence {occurrence.DisplayText}. Must be in the form '[item] -= 1'. Actual: {occurrence.Effect}");
                Console.WriteLine($"Bad decrement on occurrence {occurrence.DisplayText}. Must be in the form '[item] -= 1'. Actual: {occurrence.Effect}");
                return;
            }

            var item = _party.Inventory.GetByName(itemDecrementMatch.Groups[1].Value);
            if (item == null)
            {
                //throw new Exception($"Inventory item {itemDecrementMatch.Groups[1].Value} not found.");
                Console.WriteLine($"Inventory item {itemDecrementMatch.Groups[1].Value} not found.");
                return;
            }
            var amount = int.Parse(itemDecrementMatch.Groups[2].Value);
            var success = item.Subtract(amount);
            if (!success)
            {
                //throw new Exception($"Not enough {item.Name} to subtract {amount}.");
                Console.WriteLine($"Not enough {item.Name} to subtract {amount}.");
                return;
                
            }
        }

        public void TryIncreaseInventoryItem(IOccurrence occurrence)
        {
            var itemIncrementMatch = IncrementEventPattern.Match(occurrence.Effect);
            if (!itemIncrementMatch.Success)
            {
                //throw new Exception($"Bad increment on occurrence {occurrence.DisplayText}. Must be in the form '[item] += 1'. Actual: {occurrence.Effect}");
                Console.WriteLine($"Bad increment on occurrence {occurrence.DisplayText}. Must be in the form '[item] += 1'. Actual: {occurrence.Effect}");
                return;
            }

            var item = _party.Inventory.GetByName(itemIncrementMatch.Groups[1].Value);
            if (item == null)
            {
                //throw new Exception($"Inventory item {itemIncrementMatch.Groups[1].Value} not found.");
                Console.WriteLine($"Inventory item {itemIncrementMatch.Groups[1].Value} not found.");
                return;
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
