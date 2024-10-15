using System;
using System.IO;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.IO.Pipes;
using System.Security;

namespace DestinyTrail.Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            game.StartGameLoop();
        }
    }
}
