namespace DestinyTrail.Engine
{
    public interface IDisplay
    { 
        public List<string> Items { get; }
        void Write(string message);

        void WriteTitle(string message);
        void Clear();
        void ScrollToBottom();
    }
}
