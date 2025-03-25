using DestinyTrail.Engine;
using DestinyTrail.Engine.Abstractions;
using DestinyTrail.Server;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DestinyTrail.Blazor.Components
{
    // Example of how MainGame would extend MainGameBase
    public class MainGameExample : MainGameBase
    {
        private BlazorDisplay? _outputDisplay;
        private BlazorDisplay? _statusDisplay;
        private string _lastChatMessage;
        private string commandInput = "";

        [Inject]
        private HttpClient HttpClient { get; set; }

        public MainGameExample()
        {
            _lastChatMessage = string.Empty;
        }

        protected override IDisplay OutputDisplay => _outputDisplay ?? throw new InvalidOperationException("Output display not initialized");
        protected override IDisplay StatusDisplay => _statusDisplay ?? throw new InvalidOperationException("Status display not initialized");

        public void SetDisplays(BlazorDisplay outputDisplay, BlazorDisplay statusDisplay)
        {
            _outputDisplay = outputDisplay;
            _statusDisplay = statusDisplay;
        }

        protected override Utility CreateUtility()
        {
            var configurationProvider = new DestinyTrail.Blazor.ConfigurationProvider();
            var fileReader = new DestinyTrail.Blazor.FileReader(HttpClient);
            return new Utility(new YamlDeserializer(), fileReader, configurationProvider);
        }

        protected override ITwitchChatService CreateTwitchChatService()
        {
            return new TwitchIntegration.TwitchChatService();
        }

        protected override void OnStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        protected override void HandleMessageReceived(string username, string message)
        {
            base.HandleMessageReceived(username, message);
            _lastChatMessage = $"{username}: {message}";
            InvokeAsync(StateHasChanged);
        }

        public async Task InitializeAsync()
        {
            await InitializeGameAsync();
            await StartGameLoopAsync();
        }

        public void HandleCommand()
        {
            if (!string.IsNullOrWhiteSpace(commandInput))
            {
                ProcessInput(commandInput);
                commandInput = string.Empty;
            }
        }

        public string CommandInput
        {
            get => commandInput;
            set => commandInput = value;
        }

        public string ConnectionStatus => _connectionStatus;
        public string LastChatMessage => _lastChatMessage;
        public IReadOnlyList<(string Username, string Message)> ChatMessages => _chatMessages;
    }
}