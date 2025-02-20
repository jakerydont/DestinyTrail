using System;

namespace DestinyTrail.TwitchIntegration;

public interface ITwitchChatService
{
    event Action<string>? Connected;

    Task Connect();
    Task DisconnectAsync();
    IEnumerable<(string Username, string Message)> GetMessages();
    void Initialize(string twitchUsername, string oauthToken, string channelName);
}