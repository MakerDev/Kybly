using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.UI.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoHotkeyRemaster.UI.ViewModels
{
    public class HotkeyEditViewModel : Screen, IHandle<ProfileChangedEvent>
    {
        private readonly IEventAggregator _eventAggregator;

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
                CurrentHotkey.Explanation = value;                
                NotifyOfPropertyChange(() => Explanation);
            }
        }

        public string ProfileName
        {
            get
            {
                if (CurrentProfile == null)
                    return "Select profile to edit";

                return CurrentProfile.ProfileName;
            }
            set { }
        }

        public bool CanEdit
        {
            get
            {
                if (CurrentProfile == null)
                {
                    return false;
                }

                return true;
            }
            set { }
        }


        private Hotkey _currenHotkey;

        public Hotkey CurrentHotkey
        {
            get { return _currenHotkey; }
            set { _currenHotkey = value; }
        }


        private HotkeyProfile _currentProfile;
        public HotkeyProfile CurrentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;
                NotifyOfPropertyChange(() => CurrentProfile);
                NotifyOfPropertyChange(() => ProfileName);
                NotifyOfPropertyChange(() => Explanation);
            }
        }

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {            
            //TODO : 입력기가 기본 윈도우 입력기가 아닌 경우 e.Key 값이 이상한 경우가 있다. 나중에 확인 필요
            ((TextBox)sender).Text = e.Key.ToString();            

            e.Handled = true;
        }

        public void OkClick()
        {

        }

        public void CancelClick()
        {

        }



        public HotkeyEditViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            CurrentProfile = message.Profile;
            return Task.CompletedTask;
        }
    }
}
