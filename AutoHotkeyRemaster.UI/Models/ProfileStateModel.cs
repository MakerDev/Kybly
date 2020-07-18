using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Models
{
    public class ProfileStateModel : INotifyPropertyChanged
    {
        public string ProfileName
        {
            get { return Profile.ProfileName; }
            set
            {
                Profile.ProfileName = value;
                Profile.Save();
                CallPropertyChanged(nameof(ProfileName));
            }
        }

        private bool _canEditName = true;

        public bool CanEditName
        {
            get { return _canEditName; }
            set
            {
                _canEditName = value;
                CallPropertyChanged(nameof(CanEditName));
            }
        }

        public HotkeyProfile Profile { get; private set; }

        public ProfileStateModel(HotkeyProfile profile)
        {
            Profile = profile;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
