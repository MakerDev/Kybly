using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.WPF.Events;
using AutoHotkeyRemaster.WPF.Helpers;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    public class KeyboardViewModel : Screen, IHandle<ProfileChangedEvent>, IHandle<HotkeyModifiedEvent>, IHandle<ProfileDeletedEvent>, IHandle<SelectingKeyEvent>
    {
        private readonly IEventAggregator _eventAggregator;

        private HotkeyProfile _profile = null;
        private bool _isSelectingKey = false;
        public bool IsSelectingKey
        {
            get { return _isSelectingKey; }
            set
            {
                _isSelectingKey = value;
                NotifyOfPropertyChange(() => IsSelectingKey);
            }
        }

        //For optimization
        public Dictionary<int, Hotkey> TriggerHotkeyPairs { get; set; } = new Dictionary<int, Hotkey>();

        public HotkeyProfile Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                NotifyOfPropertyChange(() => Profile);
            }
        }


        //Key 눌렀을 때 이벤트를 발생시키기 위해
        public KeyboardViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        private void SetTriggerHotkeyPairs(HotkeyProfile profile)
        {
            TriggerHotkeyPairs.Clear();

            foreach (var hotkey in profile.Hotkeys)
            {
                TriggerHotkeyPairs.Add(hotkey.Trigger.Key, hotkey);
            }
        }

        private ToggleButton _selectedButton = null;

        public async void OnKeyClick(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as ToggleButton;

            //HACK : 나중에 tag 바꿔야 되니까 얘만 캐시해둔다.
            if (!_isSelectingKey)
                _selectedButton = clickedButton;

            int keycode = KeyConversionHelper.ExtractFromElementName(clickedButton.Name);

            Hotkey hotkey;
            bool hasHotkey = TriggerHotkeyPairs.ContainsKey(keycode);

            if (hasHotkey)
            {
                hotkey = TriggerHotkeyPairs[keycode];
            }
            else
            {
                hotkey = new Hotkey(new KeyInfo(keycode, 0), new KeyInfo());
            }

            await _eventAggregator.PublishOnUIThreadAsync(new KeySelectedEvent { Hotkey = hotkey, IsNew = !hasHotkey });
        }

        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            //HACK : Profiel = message.Profile가 먼저 실행되면 왜인지는 모르겠으나, OnProfileChanged가 먼저 fire되면서
            //다소 이상한 동작을 함.
            SetTriggerHotkeyPairs(message.Profile);

            Profile = message.Profile;

            return Task.CompletedTask;
        }

        public Task HandleAsync(HotkeyModifiedEvent message, CancellationToken cancellationToken)
        {
            switch (message.ModifiedEvent)
            {
                case EHotkeyModifiedEvent.Added:
                    _selectedButton.Tag = "True";
                    TriggerHotkeyPairs.Add(message.Hotkey.Trigger.Key, message.Hotkey);
                    break;

                case EHotkeyModifiedEvent.Modified:
                    TriggerHotkeyPairs[message.Hotkey.Trigger.Key] = message.Hotkey;
                    break;

                case EHotkeyModifiedEvent.Deleted:
                    _selectedButton.Tag = "False";
                    TriggerHotkeyPairs.Remove(message.Hotkey.Trigger.Key);
                    break;

                default:
                    break;
            }

            return Task.CompletedTask;
        }

        public Task HandleAsync(ProfileDeletedEvent message, CancellationToken cancellationToken)
        {
            Profile = null;
            TriggerHotkeyPairs.Clear();

            return Task.CompletedTask;
        }

        public Task HandleAsync(SelectingKeyEvent message, CancellationToken cancellationToken)
        {
            _isSelectingKey = message.IsSelecting;

            return Task.CompletedTask;
        }
    }
}
