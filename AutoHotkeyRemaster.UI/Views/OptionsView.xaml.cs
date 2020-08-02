using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Views
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : UserControl
    {
        public OptionsView()
        {
            InitializeComponent();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        //TODO : Move this codes to anothor custom control
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isNumeric = IsTextAllowed(e.Text);

            e.Handled = !isNumeric;
        }
    }
}
