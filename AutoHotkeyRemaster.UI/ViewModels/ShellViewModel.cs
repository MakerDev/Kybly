using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.UI.Views;
using AutoHotkeyRemaster.UI.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
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

                NotifyOfPropertyChange(() => CanEditProfileName);
                NotifyOfPropertyChange(() => CurrentProfile);
                NotifyOfPropertyChange(() => CurrentProfileName);
            }
        }

        public bool CanEditProfileName
        {
            get
            {
                if (CurrentProfile == null)
                    return false;

                return true;
            }
            private set { }
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

        private BindableCollection<ProfileListViewItem> _profileItems = new BindableCollection<ProfileListViewItem>();

        public BindableCollection<ProfileListViewItem> ProfileItems
        {
            get { return _profileItems; }
            set
            {
                _profileItems = value;
                NotifyOfPropertyChange(() => ProfileItems);
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
            ProfileItems.Clear();

            var profiles = _profileManager.Profiles;

            foreach (var profile in profiles)
            {
                ProfileItems.Add(CreateProfileListViewItem(profile));
            }
        }

        private ProfileListViewItem CreateProfileListViewItem(HotkeyProfile profile)
        {
            ProfileListViewItem profileItem = new ProfileListViewItem();

            profileItem.MouseDoubleClick += (s, e) =>
            {
                ProfileEditorView profileEditorView = new ProfileEditorView(this);
                profileEditorView.ShowDialog();
            };

            profileItem.PreviewMouseDown += OnProfileItemPreviewMouseDown;
            profileItem.DataContext = this;
            profileItem.Tag = profile.ProfileNum;

            var listViewItemContent = (profileItem.Content as ListViewItem);
            listViewItemContent.DataContext = profile;

            listViewItemContent.Content = new EditableTextbox();

            return profileItem;
        }

        private void OnProfileItemPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ProfileListViewItem profileItem = sender as ProfileListViewItem;
            CurrentProfile = _profileManager.Profiles[int.Parse(profileItem.Tag.ToString()) - 1];
        }

        private void OnProfileSelected(object sender, RoutedEventArgs e)
        {
            Button profileBtn = sender as Button;
            int profileNum = int.Parse(profileBtn.Tag.ToString());

            CurrentProfile = _profileManager.Profiles[profileNum - 1];
        }
    }
}
