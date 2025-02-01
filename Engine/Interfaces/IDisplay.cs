namespace DestinyTrail.Engine
{
    public interface IDisplay
    { 
        public List<string> Items { get; }
        Task Write(string message);

        Task WriteError(string message);

        Task WriteTitle(string message);
        Task Clear();
        Task ScrollToBottom();
    }
}
