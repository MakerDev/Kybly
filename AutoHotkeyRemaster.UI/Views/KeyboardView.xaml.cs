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
        private KeyboardViewModel _keyboardViewModel = null;
        private readonly Dictionary<int, RadioButton> _keyButtonPairs = new Dictionary<int, RadioButton>();

        private bool _isSelectingKey = false;
        private ToggleButton _selectedButton = null;

        public KeyboardView()
        {
            InitializeComponent();

            var eventAggregator = IoC.Get<IEventAggregator>();
            eventAggregator.SubscribeOnUIThread(this);

            RegisterButtons();
        }

        private void RegisterButtons()
        {
            var children = xGridKeyButtons.Children;

            foreach (var item in children)
            {
                RadioButton button = item as RadioButton;
                int keycode = KeyConversionHelper.ExtractFromElementName((button.Name));

                _keyButtonPairs.Add(keycode, button);
            }
        }

        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            if (_keyboardViewModel == null)
            {
                _keyboardViewModel = (KeyboardViewModel)DataContext;
            }

            ResetButton();

            return Task.CompletedTask;
        }

        public Task HandleAsync(ProfileDeletedEvent message, CancellationToken cancellationToken)
        {
            ResetButton();

            return Task.CompletedTask;
        }

        private void ResetButton()
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

        public Task HandleAsync(SelectingKeyEvent message, CancellationToken cancellationToken)
        {
            _isSelectingKey = message.IsSelecting;

            return Task.CompletedTask;
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

    }
}
