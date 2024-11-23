namespace DestinyTrail.Engine
{
    public interface IWagonParty
    {
        
        public List<IPerson> Members {get;set;}
        public IPerson Leader {get;set;}
        public double Health { get; }
        public IPerson GetRandomMember();

        
        public IPerson GeneratePerson(string name);
        public IPerson GenerateRandomPerson(string[] randomNames);

        public string GetDisplayNames();
        public string GetDisplayHealth();
        

        public void SpendDailyHealth(Pace pace, Rations rations);
    
    }
}
