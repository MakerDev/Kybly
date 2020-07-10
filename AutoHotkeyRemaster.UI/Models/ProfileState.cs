using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.UI.Models
{
    public class ProfileState
    {
        private readonly ShellViewModel _shellViewModel;

        public bool IsSelectedProfile
        {
            get
            {
                if (Profile.ProfileNum == _shellViewModel.CurrentProfile.ProfileNum)
                {
                    return true;
                }

                return false;
            }
            private set { }
        }


        public HotkeyProfile Profile { get; private set; }

        public ProfileState(HotkeyProfile profile, ShellViewModel shellViewModel)
        {
            Profile = profile;
            _shellViewModel = shellViewModel;
        }


    }
}
