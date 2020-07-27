using System.Windows;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Views
{
    /// <summary>
    /// Interaction logic for InfoWindowView.xaml
    /// </summary>
    public partial class InfoWindowView : Window
    {
        public InfoWindowView()
        {
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

    }
}
