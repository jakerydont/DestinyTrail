using DestinyTrail.Engine;

namespace DestinyTrail.EngineTest
{
    public class TestType : GameComponent
    {

        public override bool Equals(object obj)
        {
            return this.Name == ((TestType)obj).Name;
        }
    }
       
}