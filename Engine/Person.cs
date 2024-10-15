namespace DestinyTrail.Engine
{
    public class Person
    {
        public required int ID { get; set; }  // Unique identifier for the person
        public required string Name { get; set; } // Name of the person
        public required Status Status { get; set; } // Status of the person (now strongly typed)
    }
}
