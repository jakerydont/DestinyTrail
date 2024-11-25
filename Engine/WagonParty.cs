using System.Text;

namespace DestinyTrail.Engine
{
    public class WagonParty : IWagonParty
    {
        private int memberCounter = 0;
        public IUtility Utility;

        public List<IPerson> Members {get;set;}
        public IPerson Leader {get;set;}
        public double Health { get; private set; }

        public IInventory Inventory { get; set; }
        private int _maxRationFactor {get;set;}


        public WagonParty() : this(new Utility()) {}
        public WagonParty(IUtility utility)
        {
            Utility = utility; 
            string randomNamesPath = "data/RandomNames.yaml";
            string[] RandomNames = [.. Utility.LoadYaml<RandomNamesData>(randomNamesPath)];

            Random.Shared.Shuffle(RandomNames);
            var partyNames = RandomNames.Take(26).ToArray();

            Members = new List<IPerson>();

            foreach(var name in partyNames) {
                var member = GeneratePerson(name);
                Members.Add(member);
            }
            Leader = Members.First();
            Health = 100;
        }
        public WagonParty(string[] names) : this(names, new Utility()) {}
        public WagonParty(string[] names, IUtility utility) {
            Utility = utility;
            Members = new List<IPerson>();

            foreach(var name in names) {
                var member = GeneratePerson(name);
                Members.Add(member);
            }
            Leader = Members.First();
            Health = 100;
        }

        public IPerson GetRandomMember() {
            Random random = new Random();
            var person = Members[random.Next(Members.Count)];
            return person;
        }

        
        public IPerson GeneratePerson(string name)
        {
            var id = memberCounter;
            memberCounter++;            
            return new Person
            {
                ID = id,
                Name = name,
                Status = new Status { Name = "Good" }
            };
        }

        public IPerson GenerateRandomPerson(string[] randomNames)
        {
            Random random = new Random();
            var id = memberCounter;
            memberCounter++;
            var name = randomNames[random.Next(randomNames.Length)];

            return new Person
            {
                ID = id,
                Name = name,
                Status = new Status { Name = "Good" }
            };
        }

        public string GetDisplayNames()
        {
            var sb = new StringBuilder();
            sb.AppendJoin(", ",Members.Select(m=>m.Name));
            return sb.ToString();
        }

        public string GetDisplayHealth() => Utility.Abbreviate(Health);
        

        public void SpendDailyHealth(Pace pace, Rations rations)
        {
            double healthChange = -((100 / rations.Factor) * (pace.Factor / 8) - 0.5);
            Health += healthChange;
        }
    }
}
