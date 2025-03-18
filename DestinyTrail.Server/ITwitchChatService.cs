namespace DestinyTrail.Server;

public interface ITwitchChatService
{
    event Action<string> OnConnectionStatusChanged;
    event Action<string>? Connected;
    event Action<string, string>? OnMessageReceived;
    
    void Initialize(string username, string oauthToken, string channel);
    Task ConnectAsync();
    Task DisconnectAsync();
    IEnumerable<(string Username, string Message)> GetMessages();
    Task SendMessageAsync(string message);
    bool IsConnected { get; }
}
