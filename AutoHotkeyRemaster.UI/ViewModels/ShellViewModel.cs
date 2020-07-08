using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AutoHotkeyRemaster.UI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private BindableCollection<Button> _profileBtns = new BindableCollection<Button>();
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
            private set { }
        }

        public BindableCollection<Button> ProfileBtns
        {
            get { return _profileBtns; }
            set
            {
                _profileBtns = value;
                NotifyOfPropertyChange(() => ProfileBtns);
            }
        }

        public ShellViewModel(ProfileManager profileManager)
        {
            _profileManager = profileManager;

            SetProfileButtons();
        }

        private void SetProfileButtons()
        {
            ProfileBtns.Clear();

            var profiles = _profileManager.Profiles;

            foreach (var profile in profiles)
            {
                ProfileBtns.Add(CreateProfileButton(profile));
            }

            Button createProfileBtn = new Button();

            //TODO : ICommand로 대체. 최대 갯수에서 disable하기 위해
            createProfileBtn.Margin = new Thickness(0, 0, 0, 5);
            createProfileBtn.Width = 330;
            createProfileBtn.Height = 35;
            createProfileBtn.Content = "Create New";
            createProfileBtn.Click += (s, e) =>
            {
                _profileManager.CreateNewProfile();
                _profileManager.SaveAllProfiles();
                SetProfileButtons();
            };


            ProfileBtns.Add(createProfileBtn);
        }

        private Button CreateProfileButton(HotkeyProfile profile)
        {
            Button profileBtn = new Button();

            profileBtn.Content = profile.ProfileName ?? $"profile{profile.ProfileNum}";
            profileBtn.Tag = profile.ProfileNum.ToString();
            profileBtn.Margin = new Thickness(0, 0, 0, 5);
            profileBtn.Width = 330;
            profileBtn.Height = 35;
            profileBtn.Click += OnProfileClicked;

            return profileBtn;
        }

        private void OnProfileClicked(object sender, RoutedEventArgs e)
        {
            Button profileBtn = sender as Button;
            int profileNum = int.Parse(profileBtn.Tag.ToString());

            CurrentProfile = _profileManager.Profiles[profileNum - 1];
        }
    }
}
