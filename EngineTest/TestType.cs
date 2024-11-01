using DestinyTrail.Engine;

namespace DestinyTrail.Engine.Tests
{
    public class TestType : GameComponent
    {

        public override bool Equals(object obj)
        {
            return this.Name == ((TestType)obj).Name;
        }
    }
       
}