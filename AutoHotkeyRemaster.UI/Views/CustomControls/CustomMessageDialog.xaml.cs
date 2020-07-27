using System.Windows;

namespace AutoHotkeyRemaster.WPF.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for CustomMessageDialog.xaml
    /// </summary>
    public partial class CustomMessageDialog : Window
    {
        public CustomMessageDialog(string message)
        {
            InitializeComponent();
            DialogMessage.Text = message;
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
