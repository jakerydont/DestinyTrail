namespace DestinyTrail.Engine;

public class Person : GameComponent, IPerson
{
    public static IPerson Nobody => new Person { Name = "Nobody", ID = -1, Status = new Status { Name = "Healthy" } };
    public required int ID { get; set; }
    public required Status Status { get; set; } 
    public double Health { get; set; }

    public void KillCheck() {
        if (Health <= 0)
        {
            Kill();
        }
    }
    
    public void Kill() {
        Health = 0;
        Status = Status.Dead;
    }
    
    public bool isAlive => Status.Name.ToLower() != "dead";

    public override string ToString()
    {
        return $"{Name} ({ID})";
    }

    public string GetHealthStatus()
    {
        //StatusData.Get
        return "";
    }
}
