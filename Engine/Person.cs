namespace DestinyTrail.Engine
{
    public class Person : GameComponent
    {
        public required int ID { get; set; }  // Unique identifier for the person
        public required Status Status { get; set; } // Status of the person (now strongly typed)

        public override string ToString()
        {
            return $"{Name} ({ID})";
        }
    }
}
