namespace DestinyTrail.Engine;

public class Landmark : GameComponent
{
    public string ID { get; set; }
    public double Distance { get; set; }
    public bool Shop { get; set; }
    public string Lore { get; set; }


    public Landmark() : this("", "", 0, false, "Lore")
    {
    }

    public Landmark(string ID, string Name, double Distance, bool Shop, string Lore)
    {
        this.ID = ID;
        this.Name = Name;
        this.Distance = Distance;
        this.Shop = Shop;
        this.Lore = Lore;
    }
}
