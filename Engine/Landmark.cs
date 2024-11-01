namespace DestinyTrail.Engine
{
    public class Landmark : GameComponent
    {
        public string ID { get; set; }
        public double Distance { get; set; }
        public bool Shop { get; set; }
        public string Lore { get; set; }

        public Landmark()
        {

        }
    }
}
