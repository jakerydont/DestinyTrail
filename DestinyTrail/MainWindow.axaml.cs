using Avalonia.Controls;
using Avalonia.Input;
using DestinyTrail.Engine;

namespace DestinyTrail
{
    public partial class MainWindow : Window
    {

        Game game { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            var OutputDisplay = new ADisplay(OutputListBox);
            var StatusDisplay = new ADisplay(Status);
            var WorldStatus = new WorldStatus();
            var Utility = new Utility();
            var WagonParty = new WagonParty(Utility);
            var Travel = new Travel(WagonParty, Utility, StatusDisplay, WorldStatus);
            game = new Game(OutputDisplay, StatusDisplay, Utility, WagonParty, Travel, WorldStatus);
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
