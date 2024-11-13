using System.Collections;

namespace DestinyTrail.Engine
{

    public class Display : IDisplay
    {
        public void Write(string message) {
            Console.WriteLine(message);
        }

        public void Clear() {
            Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        }

         
        public void ScrollToBottom()
        {
        }
    }
}