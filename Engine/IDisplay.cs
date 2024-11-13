namespace DestinyTrail.Engine
{
    public interface IDisplay
    {
        void Write(string message);
        void Clear();
        void ScrollToBottom();
    }
}
