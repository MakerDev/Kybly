using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.UI.Models;
using AutoHotkeyRemaster.UI.Views;
using AutoHotkeyRemaster.UI.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AutoHotkeyRemaster.UI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private readonly IWindowManager _windowManager;
        private readonly ProfileManager _profileManager;

        private HotkeyProfile _currentProfile = null;

        public HotkeyProfile CurrentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;

                NotifyOfPropertyChange(() => CurrentProfile);
                NotifyOfPropertyChange(() => CurrentProfileName);
            }
        }

        public string CurrentProfileName
        {
            get
            {
                if (CurrentProfile == null)
                    return "Select profile";
                else
                    return CurrentProfile.ProfileName ?? $"profile{CurrentProfile.ProfileNum}";
            }
            set
            {
                CurrentProfile.ProfileName = value;
                CurrentProfile.Save($"profile{CurrentProfile.ProfileNum}");
                NotifyOfPropertyChange(() => CurrentProfileName);
            }
        }

        private ProfileStateModel _selectedProfile;

        public ProfileStateModel SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                NotifyOfPropertyChange(() => SelectedProfile);
            }
        }

        private BindingList<ProfileStateModel> _profileStates = new BindingList<ProfileStateModel>();

        public BindingList<ProfileStateModel> ProfileStates
        {
            get { return _profileStates; }
            set
            {
                _profileStates = value;
                NotifyOfPropertyChange(() => ProfileStates);
            }
        }

        public ShellViewModel(IWindowManager windowManager, ProfileManager profileManager)
        {
            _windowManager = windowManager;
            _profileManager = profileManager;

            SetProfileListItems();
        }

        private void SetProfileListItems()
        {
            ProfileStates.Clear();

            foreach (var profile in _profileManager.Profiles)
            {
                ProfileStateModel profileState = new ProfileStateModel(profile);
                ProfileStates.Add(profileState);
            }
        }
    }
}
