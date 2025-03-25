using Avalonia.Controls;
using Avalonia.Input;
using DestinyTrail.Engine;
using DestinyTrail.Server;
using System;
using System.Threading.Tasks;

namespace DestinyTrail
{
    // Example of how MainWindow would extend MainGameBase
    public class MainWindowExample : MainGameBase
    {
        private ADisplay _outputDisplay;
        private ADisplay _statusDisplay;
        private TextBox _inputTextBox;
        private ListBox _outputListBox;
        private ListBox _status;

        public MainWindowExample(ListBox outputListBox, ListBox status, TextBox inputTextBox)
        {
            _outputListBox = outputListBox;
            _status = status;
            _inputTextBox = inputTextBox;
            _outputDisplay = new ADisplay(_outputListBox);
            _statusDisplay = new ADisplay(_status);
            
            // Initialize the game
            InitializeGameAsync().ContinueWith(_ => StartGameLoopAsync());
        }

        protected override IDisplay OutputDisplay => _outputDisplay;
        protected override IDisplay StatusDisplay => _statusDisplay;

        protected override Utility CreateUtility()
        {
            return new Utility();
        }

        protected override ITwitchChatService CreateTwitchChatService()
        {
            return new TwitchChatService();
        }

        protected override void OnStateChanged()
        {
            // Avalonia UI updates would go here
            // This might involve dispatcher invocation
        }

        public void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var input = _inputTextBox.Text ?? "";
                ProcessInput(input);
                _inputTextBox.Text = string.Empty; // Clear the input box
                e.Handled = true;
            }
        }

        protected override void ProcessInput(string input)
        {
            base.ProcessInput(input);
            // Additional Avalonia-specific input processing
            if (game?.travel != null)
            {
                game.travel.ContinueTravelling();
            }
        }
    }
}