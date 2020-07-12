using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.UI.Events;
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
    public class ShellViewModel : Conductor<object>.Collection.AllActive
    {
        private readonly ProfileManager _profileManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly KeyboardViewModel _keyboardViewModel;
        private readonly HotkeyEditViewModel _hotkeyEditViewModel;

        private ProfileStateModel _selectedProfile;

        public ProfileStateModel SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                //INFO : If somthing goes wrong, consider to change this synchronous
                if (_selectedProfile != null)
                {
                    _eventAggregator.PublishOnUIThreadAsync(new ProfileChangedEvent { Profile = _selectedProfile.Profile });
                }
                else
                {
                    _eventAggregator.PublishOnUIThreadAsync(new ProfileChangedEvent { Profile = null });
                }


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

        public ShellViewModel(IWindowManager windowManager, ProfileManager profileManager, IEventAggregator eventAggregator,
            KeyboardViewModel keyboardViewModel, HotkeyEditViewModel hotkeyEditViewModel)
        {
            _profileManager = profileManager;
            _eventAggregator = eventAggregator;
            _keyboardViewModel = keyboardViewModel;
            _hotkeyEditViewModel = hotkeyEditViewModel;

            _eventAggregator.SubscribeOnUIThread(this);

            SetProfileListItems();

            Items.AddRange(new Screen[] { _hotkeyEditViewModel, _keyboardViewModel });
        }


        public void AddNewProfile(object sender, RoutedEventArgs e)
        {
            _profileManager.CreateNewProfile().Save();

            SetProfileListItems();
        }


        protected override async void OnViewReady(object view)
        {
            base.OnViewReady(view);

            await ActivateItemAsync(_hotkeyEditViewModel);
            await ActivateItemAsync(_keyboardViewModel);
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
