namespace DestinyTrail.Engine.Interfaces;

public interface IWagonParty
{
    
    public List<IPerson> Members {get;set;}
    public IPerson Leader {get;set;}
    public IInventory Inventory { get; set; }

    public IDictionary<string, object> Flags { get; }
    public IPerson GetRandomMember();

    public List<IPerson> GetLivingMembers();

    public bool IsAnybodyAlive();

    
    public IPerson GeneratePerson(string name);
    public IPerson GenerateRandomPerson(string[] randomNames);

    public string GetDisplayNames();
    public string GetDisplayHealth();
    
    public void SetHealth(double amount);
    public void IncreaseHealth(double amount);
    
    public void SpendDailyHealth(Pace pace, Rations rations);
    void KillMember(IPerson person);
    void KillCheckParty();
}
