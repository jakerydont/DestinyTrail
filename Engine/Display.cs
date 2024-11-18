using System.Collections;
using System.Text;

namespace DestinyTrail.Engine
{

    public class Display : IDisplay
    {
        public List<string> Items { get; set; } = new List<string>();
        public void Write(string message) 
        {
            Console.WriteLine(message);
        }


        public void WriteTitle(string message) 
        {
            var formattedTitle = BuildConsoleTitle(message);
            Console.WriteLine(formattedTitle);
        }


        static int TitlePaddingSize = 1;
        static char TitleHorizontalChar = '-';
        static char TitleVerticalChar = '|';
        static char TitleCorner = '+';
        protected virtual string BuildConsoleTitle(string message) 
        {   
            string padding = new(' ', TitlePaddingSize);
            string topBottomBorder = new(TitleHorizontalChar, Math.Max(0, message.Length + (TitlePaddingSize * 2)));
            var sb = new StringBuilder();
            sb.Append(TitleCorner + topBottomBorder + TitleCorner + '\n');
            sb.Append(TitleVerticalChar + padding + message + padding + TitleVerticalChar + '\n');
            sb.Append(TitleCorner + topBottomBorder + TitleCorner);
            return sb.ToString(); 
        }

        public void Clear() {
            // noop
        }

         
        public void ScrollToBottom()
        {
            // noop
        } 
    }
}