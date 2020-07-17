using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.UI.Events;
using AutoHotkeyRemaster.UI.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsInput.Native;

namespace AutoHotkeyRemaster.UI.ViewModels
{
    public class HotkeyEditViewModel : Screen, IHandle<ProfileChangedEvent>, IHandle<KeySelectedEvent>, IHandle<ProfileDeletedEvent>
    {
        private readonly IEventAggregator _eventAggregator;

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

        //INFO : 요청으로 현재는 사용하지 않음
        public string Explanation
        {
            get
            {
                if (CurrentHotkey == null)
                    return "";

                return CurrentHotkey.Explanation;
            }
            set
            {
                if (CurrentHotkey != null)
                {
                    CurrentHotkey.Explanation = value;
                    NotifyOfPropertyChange(() => Explanation);
                }
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get
            {
                if (CurrentProfile == null || HotkeyAction == null)
                {
                    return false;
                }

                return _canEdit;
            }
            set
            {
                _canEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
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
                NotifyOfPropertyChange(() => HotkeyActionKeyDisplay);
                NotifyOfPropertyChange(() => HotkeyAction);
            }
        }

        //TODO : Consider to replace this with converter
        public string HotkeyActionKeyDisplay
        {
            get
            {
                if (HotkeyAction == null || HotkeyAction.Key == -1)
                {
                    return "";
                }

                return KeyInterop.KeyFromVirtualKey(HotkeyAction.Key).ToString();
            }
            private set { }
        }

        private Hotkey _currenHotkey;

        public Hotkey CurrentHotkey
        {
            get { return _currenHotkey; }
            set
            {
                _currenHotkey = value;

                NotifyOfPropertyChange(() => Explanation);
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

                NotifyOfPropertyChange(() => Control);
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

                NotifyOfPropertyChange(() => Alt);
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

                NotifyOfPropertyChange(() => Shift);
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

                NotifyOfPropertyChange(() => Win);
            }

        }
        #endregion


        public void OkClick()
        {
            //If key not set
            if (!IsActionSet())
            {
                //TODO : 만약 기존에 존재하던 핫키였으면 지우고, 없었으면 그냥 생략
                System.Windows.Forms.MessageBox.Show("Action is not set!");

                return;
            }

            CurrentHotkey.Action = HotkeyAction;

            //TODO : 만약 이미 있으면 수정하는 액션으로
            int result = CurrentProfile.AddHotkey(CurrentHotkey);

            if (result > 0)
            {
                CurrentProfile.Save();
                CanEdit = false;

                _eventAggregator.PublishOnUIThreadAsync(new HotkeyModifiedEvent
                {
                    Hotkey = CurrentHotkey,
                    ModifiedEvent = EHotkeyModifiedEvent.Added
                });
            }

            //TODO : result 예외처리
            HotkeyAction = null;
            CurrentHotkey = null;
        }

        public void CancelClick()
        {
            //TODO : 편집창 깨끗이
            //TODO : CancelEvent 발생시키기
        }

        public HotkeyEditViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        public Task HandleAsync(ProfileDeletedEvent message, CancellationToken cancellationToken)
        {
            CurrentHotkey = null;
            HotkeyAction = null;
            CurrentProfile = null;

            return Task.CompletedTask;
        }

        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            //TODO : 편집창 깨끗하게 다시 세팅
            CurrentProfile = message.Profile;
            return Task.CompletedTask;
        }

        public Task HandleAsync(KeySelectedEvent message, CancellationToken cancellationToken)
        {
            CurrentHotkey = message.Hotkey;

            HotkeyAction = new KeyInfo
            {
                Key = CurrentHotkey.Action.Key,
                Modifier = CurrentHotkey.Action.Modifier
            };

            NotifyOfPropertyChange(() => HotkeyActionKeyDisplay);

            CanEdit = message.IsNew;

            return Task.CompletedTask;
        }

        public void ClearActionKey(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (HotkeyAction == null)
                return;

            HotkeyAction.Key = -1;
            NotifyOfPropertyChange(() => HotkeyActionKeyDisplay);
        }

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (!CanEdit || HotkeyAction == null)
            {
                return;
            }

            Key key = e.ImeProcessedKey == Key.None ? e.Key : e.ImeProcessedKey;

            HotkeyAction.Key = KeyInterop.VirtualKeyFromKey(key);
            NotifyOfPropertyChange(() => HotkeyActionKeyDisplay);
        }

        private bool IsActionSet()
        {
            if (Control || Alt || Win || Shift || HotkeyAction.Key != -1)
                return true;

            return false;
        }
    }
}
