using System.Text;

namespace DestinyTrail.Engine
{
    public class WagonParty : IWagonParty
    {
        public static WagonParty Default = new ();
        private int memberCounter = 0;
        public IUtility Utility;

        
        public List<IPerson> Members {get;set;}
        public IPerson Leader {get;set;}


        public IInventory Inventory { get; set; }
        private int _maxRationFactor {get;set;}
        public IDictionary<string, object> Flags { 
            get => new Dictionary<string, object>{
                { "CanHunt", true }
            };
        }
        public static async Task<WagonParty> CreateAsync(IUtility utility)
        {
            var wagonParty = new WagonParty(utility);
            await wagonParty.LoadMembersAsync();
            return wagonParty;
        }

        public WagonParty() : this(new Utility()) {}
        private WagonParty(IUtility utility)
        {
            Members = [];
            Leader = Person.Nobody;
            Utility = utility;

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

            
            Inventory = new Inventory();
        }

        
        public async Task LoadMembersAsync()
        {
            string randomNamesPath = Utility.GetAppSetting("RandomNamesFilePath");
            var randomNamesData = await Utility.LoadYamlAsync<RandomNamesData>(randomNamesPath);

            var randomNames = randomNamesData.RandomNames;
            var partyNames = randomNames.Take(5).ToArray();

            foreach (var name in partyNames)
            {
                var member = GeneratePerson(name);
                Members.Add(member);
            }

            Leader = Members.First();
            SetHealth( 2 );
        }


        public IPerson GetRandomMember()
        {
            Random random = new Random();
            var livingMembers = GetLivingMembers();
            var person = livingMembers.ToArray()[random.Next(Members.Count)];
            return person;
        }

        public List<IPerson> GetLivingMembers()
        {
            return Members.Where(p => p.isAlive).ToList();
        }

        public void KillCheckParty() 
        {
            GetLivingMembers()
            .ForEach((m) => {
                m.KillCheck();
            });
        }

        public bool IsAnybodyAlive() => GetLivingMembers().Any();

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

        public string GetDisplayHealth() => Utility.Abbreviate(Members.Average(m=>m.Health));
        
        public void SetHealth(double amount)
        {
            GetLivingMembers().ForEach((m)=>{
                m.Health = amount;
            });
        }

        public void IncreaseHealth(double amount) 
        {
            GetLivingMembers().ForEach((m)=>{
                m.Health = amount;
            });;
        }

        static readonly double minimumDailyHealthSpend = -0.5;

        public void SpendDailyHealth(Pace pace, Rations rations)
        {
            
            double healthChange = Math.Min(minimumDailyHealthSpend, -((100 / rations.Factor) * (pace.Factor / 8) - 0.5));
            Members.ForEach((m)=>{
                m.Health += healthChange;
            });
        }

        public void KillMember(IPerson person)
        {
            person.Kill();    
        }
    }


}
