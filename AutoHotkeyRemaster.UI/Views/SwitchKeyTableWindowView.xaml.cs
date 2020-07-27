using System;
using System.Windows;

namespace AutoHotkeyRemaster.WPF.Views
{
    /// <summary>
    /// Interaction logic for SwitchKeyTableWindowView.xaml
    /// </summary>
    public partial class SwitchKeyTableWindowView : Window
    {
        public SwitchKeyTableWindowView()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
