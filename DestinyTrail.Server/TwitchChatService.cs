// TwitchChatService.cs
using System.Collections.Concurrent;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace DestinyTrail.Server;

public class TwitchChatService : ITwitchChatService, IDisposable
{
    private TwitchClient? _client;
    private string? _channel;
    private ConcurrentQueue<(string Username, string Message)>? _messageQueue;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    
    // Store event handlers as fields so we can unsubscribe later
    private EventHandler<OnConnectedArgs>? _connectedHandler;
    private EventHandler<OnMessageReceivedArgs>? _messageReceivedHandler;
    private EventHandler<OnConnectionErrorArgs>? _connectionErrorHandler;
    private EventHandler<OnRaidNotificationArgs>? _raidHandler;
    
    public event Action<string>? OnConnectionStatusChanged;
    public event Action<string>? Connected;
    public event Action<string, string>? OnMessageReceived;
    
    public bool IsConnected => _client?.IsConnected ?? false;



 public void Initialize(string username, string oauthToken, string channel)
    {
        _channel = channel;
        _messageQueue = new ConcurrentQueue<(string, string)>();

        var credentials = new ConnectionCredentials(username, oauthToken);
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        var webSocketClient = new WebSocketClient(clientOptions);
        _client = new TwitchClient(webSocketClient);
        _client.Initialize(credentials, channel);

        SetupEventHandlers();
    }
 private void SetupEventHandlers()
    {
        if (_client == null) { throw new NullReferenceException("_client is null"); }
        if (_channel == null) { throw new NullReferenceException("_channel is null"); }
        if (_messageQueue == null) { throw new NullReferenceException("_messageQueue is null"); }

        _connectedHandler = (sender, args) =>
        {
            NotifyConnectionStatus("Connected to Twitch.");
            Connected?.Invoke(_channel);
        };
        _client.OnConnected += _connectedHandler;

        _client.OnDisconnected += (sender, args) =>
        {
            NotifyConnectionStatus("Disconnected from Twitch.");
        };

        _connectionErrorHandler = (sender, args) =>
        {
            NotifyConnectionStatus($"Connection error: {args.Error.Message}");
        };
        _client.OnConnectionError += _connectionErrorHandler;

        _messageReceivedHandler = (sender, args) =>
        {
            var message = (args.ChatMessage.Username, args.ChatMessage.Message);
            _messageQueue.Enqueue(message);
            OnMessageReceived?.Invoke(message.Username, message.Message);
        };
        _client.OnMessageReceived += _messageReceivedHandler;

        _raidHandler = (sender, args) =>
        {
            var raidMessage = $"Raid from {args.RaidNotification.DisplayName} with {args.RaidNotification.MsgParamViewerCount} viewers!";
            _messageQueue.Enqueue((args.RaidNotification.DisplayName, raidMessage));
        };
        _client.OnRaidNotification += _raidHandler;
    }
   public async Task ConnectAsync()
    {
        if (_client == null) { throw new NullReferenceException("_client is null"); }
        if (_channel == null) { throw new NullReferenceException("_channel is null"); }

        await _connectionLock.WaitAsync();
        try
        {
            if (!IsConnected)
            {
                OnConnectionStatusChanged?.Invoke("Connecting to Twitch...");
                await Task.Run(() => _client.Connect());
                OnConnectionStatusChanged?.Invoke("Connected to Twitch!");
            }
        }
        catch (Exception ex)
        {
            OnConnectionStatusChanged?.Invoke($"Error connecting: {ex.Message}");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }
    public async Task DisconnectAsync()
    {
        if (_client == null) { throw new NullReferenceException("_client is null"); }


        await _connectionLock.WaitAsync();
        try
        {
            if (IsConnected)
            {
                await Task.Run(() => _client.Disconnect());
                OnConnectionStatusChanged?.Invoke("Disconnected from Twitch.");
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }
    public async Task SendMessageAsync(string message)
    {
        if (_client == null) { throw new NullReferenceException("_client is null"); }


        if (!IsConnected)
        {
            throw new InvalidOperationException("Cannot send message: Not connected to Twitch.");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be empty.", nameof(message));
        }

        await Task.Run(() => _client.SendMessage(_channel, message));
    }

    public IEnumerable<(string Username, string Message)> GetMessages()
    {
        if (_messageQueue == null) { throw new NullReferenceException("_messageQueue is null"); }

        while (_messageQueue.TryDequeue(out var message))
        {
            yield return message;
        }
    }

    private void NotifyConnectionStatus(string status)
    {
        OnConnectionStatusChanged?.Invoke(status);
    }

    public void Dispose()
    {
        try
        {
            if (IsConnected)
            {
                if (_client == null) { throw new NullReferenceException("_client is null"); }
                _client.Disconnect();
            }
            
            // Unsubscribe from all events
            if (_client != null)
            {
                _client.OnConnected -= _connectedHandler;
                _client.OnMessageReceived -= _messageReceivedHandler;
                _client.OnConnectionError -= _connectionErrorHandler;
                _client.OnRaidNotification -= _raidHandler;
            }

            _connectionLock.Dispose();
            _messageQueue = null;
        }
        catch (Exception ex)
        {
            // Log disposal error if needed
            Console.WriteLine($"Error during disposal: {ex.Message}");
        }
    }
}