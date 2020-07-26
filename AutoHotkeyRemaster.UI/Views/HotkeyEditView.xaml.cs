using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoHotkeyRemaster.WPF.Views
{
    /// <summary>
    /// Interaction logic for ProfileEditView.xaml
    /// </summary>
    public partial class HotkeyEditView : UserControl
    {
        private ToggleButton _currentOnSpecialKey = null;

        public HotkeyEditView()
        {
            InitializeComponent();
        }

        private void OnSpecialKeyUnChecked(object sender, RoutedEventArgs e)
        {
            ToggleButton specialKeyButton = sender as ToggleButton;

            if (_currentOnSpecialKey == specialKeyButton)
                _currentOnSpecialKey = null;
        }

        private void OnSpecialKeyChecked(object sender, RoutedEventArgs e)
        {
            if (_currentOnSpecialKey != null)
                _currentOnSpecialKey.IsChecked = false;

            _currentOnSpecialKey = sender as ToggleButton;
        }
    }
}
