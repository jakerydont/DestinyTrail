using System.Collections;
using System.Text;

namespace DestinyTrail.Engine
{

    public class Display : IDisplay
    {
        public List<string> Items { get; set; } = new List<string>();

        private readonly ConsoleColor ErrorColor = ConsoleColor.Red;

        public async Task Write(string message) 
        {
            await Task.Run(() =>Console.WriteLine(message));
        }


        public async Task WriteTitle(string message) 
        {
            var formattedTitle = BuildConsoleTitle(message);
            await Task.Run(() => Console.WriteLine(formattedTitle));
        }


        static int TitlePaddingSize = 1;
        static char TitleHorizontalChar = '-';
        static char TitleVerticalChar = '|';
        static char TitleCorner = '+';
        public virtual string BuildConsoleTitle(string message) 
        {   
            string padding = new(' ', TitlePaddingSize);
            string topBottomBorder = new(TitleHorizontalChar, Math.Max(0, message.Length + (TitlePaddingSize * 2)));
            var sb = new StringBuilder();
            sb.Append(TitleCorner + topBottomBorder + TitleCorner + '\n');
            sb.Append(TitleVerticalChar + padding + message + padding + TitleVerticalChar + '\n');
            sb.Append(TitleCorner + topBottomBorder + TitleCorner);
            return sb.ToString(); 
        }

        public virtual async Task Clear() {
            await Task.Run(() => {
                for (var i = 0; i < 10; i++){
                    Console.WriteLine(".");
                    Console.WriteLine(",");
                }
            });
        }



#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task ScrollToBottom()
        {
            // noop
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public virtual async Task WriteError(string message)
        {
            await Task.Run(() => {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ErrorColor;
                Console.WriteLine(message);
                Console.ForegroundColor = previousColor;
            });
        }
    }
}