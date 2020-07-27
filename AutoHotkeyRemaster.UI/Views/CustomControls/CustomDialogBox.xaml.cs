using System.Windows;

namespace AutoHotkeyRemaster.WPF.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for CustomDialogBox.xaml
    /// </summary>
    public partial class CustomDialogBox : Window
    {
        public CustomDialogBox(string message)
        {
            InitializeComponent();

            DialogMessage.Text = message;
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
