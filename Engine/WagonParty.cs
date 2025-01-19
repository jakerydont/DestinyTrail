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
        public IDictionary<string, object> Flags { 
            get => new Dictionary<string, object>{
                { "CanHunt", true }
            };
        }


        public WagonParty() : this(new Utility()) {}
        public WagonParty(IUtility utility)
        {
            Utility = utility; 
            string randomNamesPath = Utility.GetAppSetting("RandomNamesFilePath");
            string[] RandomNames = [.. Utility.LoadYaml<RandomNamesData>(randomNamesPath)];

            Random.Shared.Shuffle(RandomNames);
            var partyNames = RandomNames.Take(5).ToArray();

            Members = new List<IPerson>();

            foreach(var name in partyNames) {
                var member = GeneratePerson(name);
                Members.Add(member);
            }
            Leader = Members.First();
            Health = 100;

            Inventory = new Inventory();
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
            
            Inventory = new Inventory();
        }

        public IPerson GetRandomMember()
        {
            Random random = new Random();
            var livingMembers = GetLivingMembers();
            var person = livingMembers.ToArray()[random.Next(Members.Count)];
            return person;
        }

        public IEnumerable<IPerson> GetLivingMembers()
        {
            return Members.Where(p => p.isAlive);
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

        public void KillMember(IPerson person)
        {
            person.Status.Name = "dead";
            
        }
    }
}
