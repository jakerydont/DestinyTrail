namespace DestinyTrail.Engine
{
    public class Person : GameComponent, IPerson
    {
        public required int ID { get; set; }
        public required Status Status { get; set; } 

        public override string ToString()
        {
            return $"{Name} ({ID})";
        }
    }
}
