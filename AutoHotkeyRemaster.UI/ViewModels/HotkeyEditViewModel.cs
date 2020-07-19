﻿using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Events;
using AutoHotkeyRemaster.WPF.Events;
using AutoHotkeyRemaster.WPF.Views.CustomControls;
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

namespace AutoHotkeyRemaster.WPF.ViewModels
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
                Save();
                NotifyOfPropertyChange(() => HotkeyActionKey);
            }
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

                Save();
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

                Save();
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

                Save();
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

                Save();
                NotifyOfPropertyChange(() => Win);
            }

        }
        #endregion

        public void Save()
        {
            CurrentHotkey.Action = HotkeyAction;

            if (IsActionSet())
            {
                SaveOrEdit(CurrentHotkey);
            }
            else
            {
                DeleteIfExists(CurrentHotkey);
            }
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
            CurrentProfile = message.Profile;
            CurrentHotkey = null;
            HotkeyAction = null;

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

            return Task.CompletedTask;
        }

        public void ClearAction()
        {
            HotkeyAction = new KeyInfo();
            DeleteIfExists(CurrentHotkey);
        }

        //Trigger : PreviewMouseRightButtonDown
        public void ClearActionKey(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (HotkeyAction == null)
                return;

            HotkeyActionKey = -1;
        }

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (!CanEdit || HotkeyAction == null)
            {
                return;
            }

            Key key = e.ImeProcessedKey == Key.None ? e.Key : e.ImeProcessedKey;

            HotkeyActionKey = KeyInterop.VirtualKeyFromKey(key);
        }

        private void SaveOrEdit(Hotkey hotkey)
        {
            int result = CurrentProfile.AddOrEditHotkeyIfExisting(hotkey);

            switch (result)
            {
                case -1:
                    //TODO : replace this with custom alert
                    MessageBox.Show("No more hotkey is available");
                    break;

                case 0:
                    _eventAggregator.PublishOnUIThreadAsync(new HotkeyModifiedEvent
                    {
                        Hotkey = hotkey,
                        ModifiedEvent = EHotkeyModifiedEvent.Modified
                    });
                    break;

                case 1:
                    _eventAggregator.PublishOnUIThreadAsync(new HotkeyModifiedEvent
                    {
                        Hotkey = hotkey,
                        ModifiedEvent = EHotkeyModifiedEvent.Added
                    });
                    break;

                default:
                    break;
            }

        }

        private void DeleteIfExists(Hotkey hotkey)
        {
            if (CurrentProfile.DeleteHotkeyIfExisting(hotkey))
            {
                _eventAggregator.PublishOnUIThreadAsync(new HotkeyModifiedEvent
                {
                    Hotkey = hotkey,
                    ModifiedEvent = EHotkeyModifiedEvent.Deleted
                });
            }
        }

        private bool IsActionSet()
        {
            if (HotkeyAction.Modifier == 0 && HotkeyAction.Key == -1)
                return false;

            return true;
        }
    }
}
