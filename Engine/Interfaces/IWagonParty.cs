namespace DestinyTrail.Engine
{
    public interface IWagonParty
    {
        
        public List<IPerson> Members {get;set;}
        public IPerson Leader {get;set;}
        public double Health { get; }
        public IInventory Inventory { get; set; }

        public IDictionary<string, object> Flags { get; }
        public IPerson GetRandomMember();

        public IEnumerable<IPerson> GetLivingMembers();

        public bool IsAnybodyAlive();

        
        public IPerson GeneratePerson(string name);
        public IPerson GenerateRandomPerson(string[] randomNames);

        public string GetDisplayNames();
        public string GetDisplayHealth();
        

        public void SpendDailyHealth(Pace pace, Rations rations);
        void KillMember(IPerson person);
    }
}
