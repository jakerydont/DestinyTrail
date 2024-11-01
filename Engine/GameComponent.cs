namespace DestinyTrail.Engine
{
    public class GameComponent
    {
        public required string Name { get; set; } // The name of the status

        public override string ToString() => Name;
        
    }
}
