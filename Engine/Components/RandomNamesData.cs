
namespace DestinyTrail.Engine; 
public class RandomNamesData : GameData<PersonName>
{
     public required List<PersonName> RandomNames { get => _items; set => _items = value; }

     public RandomNamesData()
     {
        
     }
}