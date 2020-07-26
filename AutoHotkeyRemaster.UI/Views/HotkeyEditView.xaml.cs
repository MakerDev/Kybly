using AutoHotkeyRemaster.WPF.Events;
using AutoHotkeyRemaster.WPF.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public partial class HotkeyEditView : UserControl, IHandle<ProfileChangedEvent>, IHandle<ProfileDeletedEvent>
    {
        private HotkeyEditViewModel _hotkeyEditViewModel = null;

        private ToggleButton _currentOnSpecialKey = null;
        private ToggleButton _currentOnSelectKeyButton = null;

        public HotkeyEditView()
        {
            InitializeComponent();

            IoC.Get<IEventAggregator>().SubscribeOnUIThread(this);
        }

        private void OnSelectKeyChecked(object sender, RoutedEventArgs e)
        {
            if (_hotkeyEditViewModel == null)
            {
                _hotkeyEditViewModel = (HotkeyEditViewModel)DataContext;
            }

            var toggleButton = sender as ToggleButton;

            if (_currentOnSelectKeyButton != null && _currentOnSelectKeyButton != toggleButton)
            {
                _currentOnSelectKeyButton.IsChecked = false;                
            }

            HotkeyEditViewModel.ESelectKeyTarget target;

            if (toggleButton.Name.Contains(HotkeyEditViewModel.ESelectKeyTarget.ActionKey.ToString()))
            {
                target = HotkeyEditViewModel.ESelectKeyTarget.ActionKey;
            }
            else
            {
                target = HotkeyEditViewModel.ESelectKeyTarget.EndingKey;
            }

            _hotkeyEditViewModel.StartSelectingKey(target);
            _currentOnSelectKeyButton = toggleButton;
        }

        private void OnSelectKeyUnChecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (_currentOnSelectKeyButton == toggleButton)
            {
                _currentOnSelectKeyButton = null;
                _hotkeyEditViewModel.StopSelectingKey();
            }
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

        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            if(_currentOnSelectKeyButton != null)
                _currentOnSelectKeyButton.IsChecked = false;

            return Task.CompletedTask;
        }

        public Task HandleAsync(ProfileDeletedEvent message, CancellationToken cancellationToken)
        {
            if (_currentOnSelectKeyButton != null)
                _currentOnSelectKeyButton.IsChecked = false;

            return Task.CompletedTask;
        }
    }
}
