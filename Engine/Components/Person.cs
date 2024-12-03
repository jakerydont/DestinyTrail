namespace DestinyTrail.Engine
{
    public class Person : GameComponent, IPerson
    {
        public static IPerson Nobody => new Person { Name = "Nobody", ID = -1, Status = new Status { Name = "Healthy" } };
        public required int ID { get; set; }
        public required Status Status { get; set; } 

        public override string ToString()
        {
            return $"{Name} ({ID})";
        }
    }
}
