using System.Text;

namespace DestinyTrail.Engine
{
    public class WagonParty
    {
        private int memberCounter = 0;
        public List<Person> Members {get;set;}
        public Person Leader {get;set;}
        public double Health { get; private set; }

        private int _maxRationFactor {get;set;}


        public WagonParty(string[] names) {
            Members = new List<Person>();

            foreach(var name in names) {
                var member = GeneratePerson(name);
                Members.Add(member);
            }
            Leader = Members.First();
            Health = 100;
        }

        public Person GetRandomMember() {
            Random random = new Random();
            var person = Members[random.Next(Members.Count)];
            return person;
        }

        
        public Person GeneratePerson(string name)
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

        public Person GenerateRandomPerson(string[] randomNames)
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

        public string GetDisplayHealth() => Health.Abbreviate();
        

        public void SpendDailyHealth(Pace pace, Rations rations)
        {
            double healthChange = -((100 / rations.Factor) * (pace.Factor / 8) - 0.5);
            Health += healthChange;
        }
    }
}
