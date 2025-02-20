using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace DestinyTrail.TwitchIntegration;



public class TwitchChatService : ITwitchChatService
{
    private readonly TwitchClient _client;
    private readonly ConcurrentQueue<(string Username, string Message)> _messageQueue;

    public event Action<string>? Connected;

    public TwitchChatService()
    {
        _client = new TwitchClient();
        _messageQueue = new ConcurrentQueue<(string, string)>();
    }

    public void Initialize(string twitchUsername, string oauthToken, string channelName)
    {
        var credentials = new ConnectionCredentials(twitchUsername, oauthToken);
        _client.Initialize(credentials, channelName);

        _client.OnMessageReceived += (sender, args) =>
        {
            _messageQueue.Enqueue((args.ChatMessage.Username, args.ChatMessage.Message));
        };

        _client.OnConnected += (sender, args) =>
        {
            Connected?.Invoke($"Connected to channel: {args.AutoJoinChannel}");
        };
    }

    public void Connect()
    {
        if (!_client.IsConnected)
        {
            _client.Connect();
        }
    }

    public async Task DisconnectAsync()
    {
        if (_client.IsConnected)
        {
            _client.Disconnect();
            await Task.CompletedTask;
        }
    }

    public IEnumerable<(string Username, string Message)> GetMessages()
    {
        while (_messageQueue.TryDequeue(out var message))
        {
            yield return message;
        }
    }
}
