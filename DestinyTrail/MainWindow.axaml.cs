using Avalonia.Controls;
using Avalonia.Input;

namespace DestinyTrail
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var input = InputTextBox.Text;
                OutputListBox.Items.Add($"> {input}");
                ProcessInput(input);
                InputTextBox.Text = string.Empty; // Clear the input box
                e.Handled = true;
            }
        }

        private void ProcessInput(string input)
        {
            // Process the input as needed
            // Example: Add a response to the output
            OutputListBox.Items.Add($"Response: {input.ToUpper()}");
        }
    }
}
