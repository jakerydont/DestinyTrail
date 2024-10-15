using Avalonia.Controls;

namespace DestinyTrail.Engine
{

    public class Display {
        private ListBox? _output {get;set;}

        public Display()
        {
            _output = null;
        }

        public Display(ListBox output)
        {
            _output = output;
        }

        public void Write(string message) {
            if (_output != null) {
                _output.Items.Add(message);
            }
            else {
                Console.WriteLine(message);
            }
        }

        public void Clear() {
            if (_output != null) {
                _output.Items.Clear();
            }
            else {
                Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
            }
        }
    }

}