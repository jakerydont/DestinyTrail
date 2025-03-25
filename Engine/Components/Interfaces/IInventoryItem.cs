namespace DestinyTrail.Engine;

public interface IInventoryItem
{
    public string Name { get; set; }
    public string NameSingular { get; set; }
    public string NamePlural { get; set; }

    public string SingularOrPluralName(int Quantity);
    public int Quantity { get; set; }

    public string Unit { get; set; }
    public string GetLore();
    public bool Add(int Quantity);
    public bool Subtract(int Quantity);
    void SetQuantity(int v);
    void SetBoolean(bool v);
}
