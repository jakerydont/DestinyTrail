using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using DestinyTrail.Engine;
using DestinyTrail.Server;

namespace DestinyTrail
{
    public partial class MainWindow : Window
    {

        Game game { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            var inputHandler = new InputHandler();
            var OutputDisplay = new ADisplay(OutputListBox);
            var StatusDisplay = new ADisplay(Status);
            var WorldStatus = new WorldStatus();
            var Utility = new Utility();
            var wagonParty = WagonParty.CreateAsync(Utility).GetAwaiter().GetResult();
            var travel = Travel.CreateAsync(wagonParty, Utility, StatusDisplay, WorldStatus).GetAwaiter().GetResult();
            var twitchChatService = new TwitchChatService();
            game = Task.Run(() => Game.CreateAsync(OutputDisplay, StatusDisplay, Utility, wagonParty, travel, WorldStatus, inputHandler,twitchChatService)).Result;
            game.StartGameLoop();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var input = InputTextBox.Text ?? "";
                // OutputListBox.Items.Add($"> {input}");
                ProcessInput(input);
                InputTextBox.Text = string.Empty; // Clear the input box
                e.Handled = true;
            }
        }

        private void ProcessInput(string input)
        {
            // Process the input as needed
            // Example: Add a response to the output
            //OutputListBox.Items.Add($"Response: {input.ToUpper()}");
            game.travel.ContinueTravelling();
        }
    }
}
