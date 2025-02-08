namespace DestinyTrail.Engine
{
    public interface IPerson 
    {
        int ID { get; set; }
        Status Status { get; set; }
        string Name { get; set; } // Inherited from GameComponent
        bool isAlive { get; }
        double Health { get; set; }

        void Kill();
        string ToString();
    }
}
