using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.WPF.Events;
using AutoHotkeyRemaster.WPF.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsInput.Native;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    public class HotkeyEditViewModel : Screen,
        IHandle<ProfileChangedEvent>, IHandle<KeySelectedEvent>, IHandle<ProfileDeletedEvent>, IHandle<ProfileNameChangedEvent>
    {
        public enum ESelectKeyTarget
        {
            ActionKey,
            EndingKey,
            ProfileSwitchKey,
        }

        private readonly IEventAggregator _eventAggregator;
        private readonly ProfileManager _profileManager;

        //TODO : profile3가 선택된 상태에서 profile3의 이름이 바뀌면 여기의 이름은 안바뀐다
        private HotkeyProfile _currentProfile;
        public HotkeyProfile CurrentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;

                NotifyOfPropertyChange(() => CanEdit);
                NotifyOfPropertyChange(() => CurrentProfile);
            }
        }


        private string _profileName;

        public string ProfileName
        {
            get
            {
                return _profileName;
            }
            set
            {
                _profileName = value;
                NotifyOfPropertyChange(() => ProfileName);
            }
        }

        public bool CanEdit
        {
            get
            {
                if (CurrentProfile == null || HotkeyAction == null)
                {
                    return false;
                }

                return true;
            }
        }

        private KeyInfo _hotkeyAction;
        public KeyInfo HotkeyAction
        {
            get { return _hotkeyAction; }
            set
            {
                _hotkeyAction = value;

                NotifyOfPropertyChange(() => Control);
                NotifyOfPropertyChange(() => Alt);
                NotifyOfPropertyChange(() => Shift);
                NotifyOfPropertyChange(() => Win);
                NotifyOfPropertyChange(() => HotkeyActionKey);
                NotifyOfPropertyChange(() => HotkeyAction);
            }
        }

        public int HotkeyActionKey
        {
            get
            {
                return HotkeyAction?.Key ?? -1;
            }
            set
            {
                HotkeyAction.Key = value;
                SaveAsync().ContinueWith((task) =>
                {
                    NotifyOfPropertyChange(() => HotkeyActionKey);
                });
            }
        }

        private int _hotkeyEndingKey = -1;
        public int HotkeyEndingKey
        {
            get
            {
                return _hotkeyEndingKey;
            }
            set
            {
                _hotkeyEndingKey = value;
                SaveAsync().ContinueWith((task) =>
                {
                    NotifyOfPropertyChange(() => HotkeyEndingKey);
                });
            }
        }

        private Hotkey _currenHotkey;
        private bool _isSelectingKey = false;

        public Hotkey CurrentHotkey
        {
            get { return _currenHotkey; }
            set
            {
                _currenHotkey = value;

                NotifyOfPropertyChange(() => CurrentHotkey);
            }
        }

        #region MODIFIERS
        public bool Control
        {
            get
            {
                return (HotkeyAction?.Modifier & Modifiers.Ctrl) > 0;
            }
            set
            {
                if (value == true)
                    HotkeyAction.Modifier |= Modifiers.Ctrl;
                else
                    HotkeyAction.Modifier &= ~Modifiers.Ctrl;

                SaveAsync().ContinueWith((task) => NotifyOfPropertyChange(() => Control));
            }
        }

        public bool Alt
        {
            get
            {
                return (HotkeyAction?.Modifier & Modifiers.Alt) > 0;
            }
            set
            {
                if (value == true)
                    HotkeyAction.Modifier |= Modifiers.Alt;
                else
                    HotkeyAction.Modifier &= ~Modifiers.Alt;

                SaveAsync().ContinueWith((task) => NotifyOfPropertyChange(() => Alt));
            }
        }
        public bool Shift
        {
            get
            {
                return (HotkeyAction?.Modifier & Modifiers.Shift) > 0;
            }
            set
            {
                if (value == true)
                    HotkeyAction.Modifier |= Modifiers.Shift;
                else
                    HotkeyAction.Modifier &= ~Modifiers.Shift;

                SaveAsync().ContinueWith((task) => NotifyOfPropertyChange(() => Shift));
            }
        }

        public bool Win
        {
            get
            {
                return (HotkeyAction?.Modifier & Modifiers.Win) > 0;
            }
            set
            {
                if (value == true)
                    HotkeyAction.Modifier |= Modifiers.Win;
                else
                    HotkeyAction.Modifier &= ~Modifiers.Win;

                SaveAsync().ContinueWith((task) => NotifyOfPropertyChange(() => Win));
            }
        }
        #endregion

        #region MOUSE EVENTS
        public MouseEvents SelectedMouseEvent { get; set; } = MouseEvents.Click;

        public bool IsMouseKeySelected
        {
            get
            {
                if (HotkeyAction != null && HotkeyAction.MouseEvent != MouseEvents.None)
                {
                    return true;
                }

                return false;
            }
            private set { }
        }

        public bool MouseClick
        {
            get
            {
                return SelectedMouseEvent == MouseEvents.Click;
            }
            set
            {
                if (value)
                {
                    SelectedMouseEvent = MouseEvents.Click;
                    SaveAsync().ContinueWith((t) =>
                    {
                        NotifyOfPropertyChange(() => MouseClick);
                    });
                }
            }
        }
        public bool MouseDown
        {
            get
            {
                return SelectedMouseEvent == MouseEvents.Down;
            }
            set
            {
                if (value)
                {
                    SelectedMouseEvent = MouseEvents.Down;
                    SaveAsync().ContinueWith((t) =>
                    {
                        NotifyOfPropertyChange(() => MouseDown);
                    });
                }
            }
        }
        public bool MouseDoubleClick
        {
            get
            {
                return SelectedMouseEvent == MouseEvents.DoubleClick;
            }
            set
            {
                if (value)
                {
                    SelectedMouseEvent = MouseEvents.DoubleClick;
                    SaveAsync().ContinueWith((t) =>
                    {
                        NotifyOfPropertyChange(() => MouseDoubleClick);
                    });
                }
            }
        }

        #endregion

        #region SPECIAL KEYS
        public bool MouseLeftButton
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.LBUTTON;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.LBUTTON;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.LBUTTON)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => IsMouseKeySelected);
                NotifyOfPropertyChange(() => MouseLeftButton);
            }
        }

        public bool MouseRightButton
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.RBUTTON;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.RBUTTON;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.RBUTTON)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => IsMouseKeySelected);
                NotifyOfPropertyChange(() => MouseRightButton);
            }
        }

        public bool MouseMiddleButton
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.MBUTTON;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.MBUTTON;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.MBUTTON)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => IsMouseKeySelected);
                NotifyOfPropertyChange(() => MouseMiddleButton);
            }
        }

        public bool MediaPlay
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.MEDIA_PLAY_PAUSE;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.MEDIA_PLAY_PAUSE;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.MEDIA_PLAY_PAUSE)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => MediaPlay);
            }
        }

        public bool VolumeMute
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.VOLUME_MUTE;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.VOLUME_MUTE;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.VOLUME_MUTE)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => VolumeMute);
            }
        }

        public bool VolumeUp
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.VOLUME_UP;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.VOLUME_UP;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.VOLUME_UP)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => VolumeUp);
            }
        }

        public bool VolumeDown
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.VOLUME_DOWN;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.VOLUME_DOWN;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.VOLUME_DOWN)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => VolumeDown);
            }
        }

        public bool BrowserBack
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.BROWSER_BACK;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.BROWSER_BACK;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.BROWSER_BACK)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => BrowserBack);
            }
        }

        public bool BrowserForward
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.BROWSER_FORWARD;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.BROWSER_FORWARD;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.BROWSER_FORWARD)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => BrowserForward);
            }
        }
        public bool BrowserRefresh
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.BROWSER_REFRESH;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.BROWSER_REFRESH;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.BROWSER_REFRESH)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => BrowserRefresh);
            }
        }
        public bool BrowserSearch
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.BROWSER_SEARCH;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.BROWSER_SEARCH;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.BROWSER_SEARCH)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => BrowserSearch);
            }
        }

        public bool BrowserHome
        {
            get
            {
                return HotkeyActionKey == (int)VirtualKeyCode.BROWSER_HOME;
            }
            set
            {
                if (value)
                {
                    HotkeyActionKey = (int)VirtualKeyCode.BROWSER_HOME;
                }
                else if (HotkeyActionKey == (int)VirtualKeyCode.BROWSER_HOME)
                {
                    HotkeyActionKey = -1;
                }

                NotifyOfPropertyChange(() => BrowserHome);
            }
        }
        #endregion


        private ESelectKeyTarget _selectKeyTarget;

        public bool IsSelectingKey
        {
            get { return _isSelectingKey; }
            set
            {
                _isSelectingKey = value;

                if (_isSelectingKey == false)
                {
                    //To reset view
                    NotifyOfPropertyChange(() => IsSelectingKey);
                }

                _eventAggregator.PublishOnUIThreadAsync(new SelectingKeyEvent
                {
                    IsSelecting = _isSelectingKey
                });
            }
        }

        public void StartSelectingKey(ESelectKeyTarget selectKeyTarget)
        {
            IsSelectingKey = true;
            _selectKeyTarget = selectKeyTarget;
        }

        public void StopSelectingKey()
        {
            IsSelectingKey = false;
        }

        public HotkeyEditViewModel(IEventAggregator eventAggregator, ProfileManager profileManager)
        {
            _profileManager = profileManager;
            _eventAggregator = eventAggregator;

            _eventAggregator.SubscribeOnUIThread(this);
        }

        private void ResetState()
        {
            HotkeyAction = null;
            IsSelectingKey = false;
            CurrentHotkey = null;
            IsSelectingKey = false;
            _hotkeyEndingKey = -1;

            NotifyAllSpecialKeysChanged();
        }

        public Task HandleAsync(ProfileDeletedEvent message, CancellationToken cancellationToken)
        {
            CurrentProfile = null;
            ResetState();

            return Task.CompletedTask;
        }


        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            CurrentProfile = message.Profile;
            ProfileName = CurrentProfile.ProfileName;
            ResetState();

            return Task.CompletedTask;
        }

        public Task HandleAsync(ProfileNameChangedEvent message, CancellationToken cancellationToken)
        {
            ProfileName = message.NewName;

            return Task.CompletedTask;
        }

        public Task HandleAsync(KeySelectedEvent message, CancellationToken cancellationToken)
        {
            if (IsSelectingKey)
            {
                switch (_selectKeyTarget)
                {
                    case ESelectKeyTarget.ActionKey:
                        HotkeyActionKey = message.Hotkey.Trigger.Key;
                        break;

                    case ESelectKeyTarget.EndingKey:
                        HotkeyEndingKey = message.Hotkey.Trigger.Key;
                        break;

                    case ESelectKeyTarget.ProfileSwitchKey:
                        break;

                    default:
                        break;
                }

                return Task.CompletedTask;
            }

            CurrentHotkey = message.Hotkey;

            //this must be here
            if (CurrentHotkey.Action.MouseEvent == MouseEvents.None)
            {
                SelectedMouseEvent = MouseEvents.Click;
            }
            else
            {
                SelectedMouseEvent = CurrentHotkey.Action.MouseEvent;
            }

            HotkeyAction = new KeyInfo(
                CurrentHotkey.Action.Key,
                CurrentHotkey.Action.Modifier,
                CurrentHotkey.Action.MouseEvent);

            HotkeyEndingKey = CurrentHotkey.EndingAction?.Key ?? -1;

            NotifyAllSpecialKeysChanged();

            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            CurrentHotkey.Action = HotkeyAction;

            //If mouse event
            if (IsMouseAction(CurrentHotkey.Action.Key))
            {
                CurrentHotkey.Action.MouseEvent = SelectedMouseEvent;
            }
            else
            {
                CurrentHotkey.Action.MouseEvent = MouseEvents.None;
            }

            if (HotkeyEndingKey != -1)
            {
                CurrentHotkey.EndingAction = new KeyInfo(HotkeyEndingKey, 0);
            }
            else
            {
                CurrentHotkey.EndingAction = null;
            }

            if (IsActionSet())
            {
                await SaveOrEditAsync(CurrentHotkey).ConfigureAwait(false);
            }
            else
            {
                await DeleteIfExistsAsync(CurrentHotkey).ConfigureAwait(false);
            }
        }

        public async Task ClearActionAsync()
        {
            HotkeyAction = new KeyInfo();
            HotkeyEndingKey = -1;
            NotifyAllSpecialKeysChanged();
            await DeleteIfExistsAsync(CurrentHotkey).ConfigureAwait(false);
        }

        public void ClearActionKey(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (HotkeyAction == null)
                return;

            var name = (sender as TextBox).Name;

            if (name == "ActionKeyTextBox")
                HotkeyActionKey = -1;
            else
                HotkeyEndingKey = -1;
        }

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (!CanEdit || HotkeyAction == null)
            {
                return;
            }

            Key key = e.SystemKey;

            if (key == Key.None)
            {
                key = e.ImeProcessedKey == Key.None ? e.Key : e.ImeProcessedKey;
            }

            var name = (sender as TextBox).Name;

            if (name == "ActionKeyTextBox")
            {
                HotkeyActionKey = KeyInterop.VirtualKeyFromKey(key);
                NotifyAllSpecialKeysChanged();
            }
            else
            {
                HotkeyEndingKey = KeyInterop.VirtualKeyFromKey(key);
            }
        }

        private async Task SaveOrEditAsync(Hotkey hotkey)
        {
            int result = CurrentProfile.AddOrEditHotkeyIfExisting(hotkey);

            if (result >= 0)
                await _profileManager.SaveProfileAsync(CurrentProfile).ConfigureAwait(false);

            switch (result)
            {
                case -1:
                    CustomMessageDialog messageDialog
                        = new CustomMessageDialog("No more hotkey is available");
                    messageDialog.ShowDialog();
                    break;

                case 0:
                    await _eventAggregator.PublishOnUIThreadAsync(new HotkeyModifiedEvent
                    {
                        Hotkey = hotkey,
                        ModifiedEvent = EHotkeyModifiedEvent.Modified
                    });
                    break;

                case 1:
                    await _eventAggregator.PublishOnUIThreadAsync(new HotkeyModifiedEvent
                    {
                        Hotkey = hotkey,
                        ModifiedEvent = EHotkeyModifiedEvent.Added
                    });
                    break;

                default:
                    break;
            }
        }

        private void NotifyAllSpecialKeysChanged()
        {
            NotifyOfPropertyChange(() => IsMouseKeySelected);
            NotifyOfPropertyChange(() => MouseLeftButton);
            NotifyOfPropertyChange(() => MouseRightButton);
            NotifyOfPropertyChange(() => MouseMiddleButton);
            NotifyOfPropertyChange(() => MouseClick);
            NotifyOfPropertyChange(() => MouseDoubleClick);
            NotifyOfPropertyChange(() => MouseDown);
            NotifyOfPropertyChange(() => MediaPlay);
            NotifyOfPropertyChange(() => VolumeUp);
            NotifyOfPropertyChange(() => VolumeDown);
            NotifyOfPropertyChange(() => VolumeMute);
            NotifyOfPropertyChange(() => BrowserBack);
            NotifyOfPropertyChange(() => BrowserForward);
            NotifyOfPropertyChange(() => BrowserRefresh);
            NotifyOfPropertyChange(() => BrowserSearch);
            NotifyOfPropertyChange(() => BrowserHome);
        }

        private async Task DeleteIfExistsAsync(Hotkey hotkey)
        {
            if (CurrentProfile.DeleteHotkeyIfExisting(hotkey))
            {
                await _profileManager.SaveProfileAsync(CurrentProfile).ConfigureAwait(false);

                await _eventAggregator.PublishOnUIThreadAsync(new HotkeyModifiedEvent
                {
                    Hotkey = hotkey,
                    ModifiedEvent = EHotkeyModifiedEvent.Deleted
                });
            }
        }

        private bool IsMouseAction(int key)
        {
            return 1 <= key && key <= 4;
        }

        private bool IsActionSet()
        {
            if (HotkeyAction.Modifier == 0 && HotkeyAction.Key == -1 && HotkeyEndingKey == -1)
                return false;

            return true;
        }
    }
}
