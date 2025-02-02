namespace DestinyTrail.Engine
{
    public interface IPerson 
    {
        int ID { get; set; }
        Status Status { get; set; }
        string Name { get; set; } // Inherited from GameComponent
        bool isAlive { get; }
        int Health { get; set; }

        void Kill();
        string ToString();
    }
}
