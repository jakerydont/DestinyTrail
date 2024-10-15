using Avalonia.Controls;

namespace DestinyTrail.Engine
{

    public class Display {
        private ListBox? _output {get;set;}
        public ItemCollection Items => _output?.Items ?? null!;

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

         
        public void ScrollToBottom()
        {
            if (_output != null) 
            {
                if (_output.Items.Count > 0)
                {
                    var lastItem = _output.Items[_output.Items.Count - 1];

                    if (lastItem != null) {
                        _output.ScrollIntoView(lastItem);
                    }
                }
            }
        }
    }
}