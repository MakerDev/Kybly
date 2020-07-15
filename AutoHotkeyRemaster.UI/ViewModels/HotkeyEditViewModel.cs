using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.UI.Events;
using AutoHotkeyRemaster.UI.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsInput.Native;

namespace AutoHotkeyRemaster.UI.ViewModels
{
    public class HotkeyEditViewModel : Screen, IHandle<ProfileChangedEvent>, IHandle<KeySelectedEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ProfileManager _profileManager;

        //TODO : 더 깔끔하게 해놓을 방법 없나..?
        public string Explanation
        {
            get
            {
                if (CurrentHotkey == null)
                    return null;

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
            if (HotkeyAction.Key < 0)
            {
                //TODO : Replace this with custom alert window
                System.Windows.Forms.MessageBox.Show("Action key is not set!");

                return;
            }

            CurrentHotkey.Action = HotkeyAction;

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

            //TODO : 예외처리
            CurrentHotkey = null;
            HotkeyAction = null;
        }

        public void DeleteProfile()
        {
            CustomDialogBox dialogBox = new CustomDialogBox($"Are you sure to close {CurrentProfile.ProfileName}");
            dialogBox.ShowDialog();

            if(dialogBox.DialogResult.HasValue && dialogBox.DialogResult.Value)
            {
                HotkeyProfile deletedProfile = _profileManager.DeleteProfile(CurrentProfile.ProfileNum);

                CurrentProfile = null;
                CurrentHotkey = null;
                HotkeyAction = null;

                _eventAggregator.PublishOnUIThreadAsync(new ProfileDeletedEvent
                {
                    DeletedProfile = deletedProfile
                });
            }            
        }

        public void CancelClick()
        {
            //TODO : 편집창 깨끗이
            //TODO : CancelEvent 발생시키기
        }

        public HotkeyEditViewModel(IEventAggregator eventAggregator, ProfileManager profileManager)
        {
            _eventAggregator = eventAggregator;
            _profileManager = profileManager;

            _eventAggregator.SubscribeOnUIThread(this);
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

            //TODO : 현재 상태로 세팅해두거나 delete만 나오게 패널바꿔놓도록
            CanEdit = message.IsNew;

            return Task.CompletedTask;
        }

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (!CanEdit || HotkeyAction == null)
            {
                return;
            }

            //TODO : 입력기가 기본 윈도우 입력기가 아닌 경우 e.Key 값이 이상한 경우가 있다. 나중에 확인 필요
            ((TextBox)sender).Text = e.Key.ToString();

            HotkeyAction.Key = int.Parse(KeyInterop.VirtualKeyFromKey(e.Key).ToString());
        }
    }
}
