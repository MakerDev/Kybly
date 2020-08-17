using AutoHotkeyRemaster.WPF.Events;
using AutoHotkeyRemaster.WPF.Helpers;
using AutoHotkeyRemaster.WPF.ViewModels;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AutoHotkeyRemaster.WPF.Views
{
    /// <summary>
    /// Interaction logic for KeyboardPage.xaml
    /// </summary>
    public partial class KeyboardView : UserControl, IHandle<ProfileChangedEvent>, IHandle<SelectingKeyEvent>, IHandle<ProfileDeletedEvent>
    {
        private readonly Dictionary<int, RadioButton> _keyButtonPairs = new Dictionary<int, RadioButton>();
        private KeyboardViewModel _keyboardViewModel = null;
        private bool _isSelectingKey = false;
        private ToggleButton _selectedButton = null;

        public KeyboardView()
        {
            InitializeComponent();

            var eventAggregator = IoC.Get<IEventAggregator>();
            eventAggregator.SubscribeOnUIThread(this);

            RegisterButtons();
        }

        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            if (_keyboardViewModel == null)
            {
                _keyboardViewModel = (KeyboardViewModel)DataContext;
            }

            ResetButtons();

            return Task.CompletedTask;
        }

        public Task HandleAsync(ProfileDeletedEvent message, CancellationToken cancellationToken)
        {
            ResetButtons();

            return Task.CompletedTask;
        }

        public Task HandleAsync(SelectingKeyEvent message, CancellationToken cancellationToken)
        {
            _isSelectingKey = message.IsSelecting;

            return Task.CompletedTask;
        }

        private void ResetButtons()
        {
            var keyHotkeyPair = _keyboardViewModel.TriggerHotkeyPairs;

            foreach (var keycode in _keyButtonPairs.Keys)
            {
                if (keyHotkeyPair.ContainsKey(keycode))
                {
                    _keyButtonPairs[keycode].Tag = "True";
                }
                else
                {
                    _keyButtonPairs[keycode].Tag = "False";
                }

                _keyButtonPairs[keycode].IsChecked = false;
            }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;

            if (_isSelectingKey)
            {
                if (_selectedButton != button)
                {
                    _selectedButton.IsChecked = true;
                    button.IsChecked = false;
                }

                return;
            }

            if (_selectedButton != null)
            {
                _selectedButton.IsChecked = false;
            }

            _selectedButton = button;
        }

        private void RegisterButtons()
        {
            var children = _gridKeyButtons.Children;

            foreach (var item in children)
            {
                var button = item as RadioButton;
                var keycode = KeyConversionHelper.ExtractFromElementName((button.Name));

                _keyButtonPairs.Add(keycode, button);
            }
        }
    }
}
