using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
