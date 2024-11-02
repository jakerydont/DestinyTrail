namespace DestinyTrail.Engine
{
    public class Landmark : GameComponent
    {
        public required string ID { get; set; }
        public double Distance { get; set; }
        public bool Shop { get; set; }
        public required string Lore { get; set; }

        public Landmark()
        {

        }
    }
}
