using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Events;
using AutoHotkeyRemaster.WPF.Events;
using AutoHotkeyRemaster.WPF.Models;
using AutoHotkeyRemaster.WPF.Views;
using AutoHotkeyRemaster.WPF.Views.CustomControls;
using Caliburn.Micro;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.AllActive, IHandle<HookStateChangeEvent>
    {
        private readonly ProfileManager _profileManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly ApplicationModel _application;
        private readonly KeyboardViewModel _keyboardViewModel;
        private readonly WindowsHookManager _windowsHookManager;
        private readonly HotkeyEditViewModel _hotkeyEditViewModel;
        private readonly OptionsViewModel _optionsViewModel;
        private readonly InfoWindowViewModel _infoWindowViewModel;
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
                    _eventAggregator.PublishOnUIThreadAsync(new ProfileChangedEvent { Profile = _selectedProfile.Profile })
                        .ContinueWith((task) =>
                        {
                            NotifyOfPropertyChange(() => SelectedProfile);
                            NotifyOfPropertyChange(() => CanDeleteProfile);
                        });
                }
                else
                {
                    NotifyOfPropertyChange(() => SelectedProfile);
                    NotifyOfPropertyChange(() => CanDeleteProfile);
                }

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

        public OptionsViewModel OptionsViewModel
        {
            get
            {
                return _optionsViewModel;
            }
            private set { }
        }

        private bool _hookActivated;
        public bool HookActivated
        {
            get { return _hookActivated; }
            set
            {
                _hookActivated = value;
                NotifyOfPropertyChange(() => HookActivated);
            }
        }

        public ShellViewModel(ProfileManager profileManager, IEventAggregator eventAggregator, IWindowManager windowManager,
            ApplicationModel application, KeyboardViewModel keyboardViewModel, WindowsHookManager windowsHookManager,
            HotkeyEditViewModel hotkeyEditViewModel, OptionsViewModel optionsViewModel,
            InfoWindowViewModel infoWindowViewModel)
        {
            _profileManager = profileManager;
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _application = application;
            _keyboardViewModel = keyboardViewModel;
            _windowsHookManager = windowsHookManager;
            _hotkeyEditViewModel = hotkeyEditViewModel;
            _optionsViewModel = optionsViewModel;
            _infoWindowViewModel = infoWindowViewModel;
            _eventAggregator.SubscribeOnUIThread(this);

            _windowManager.ShowWindowAsync(infoWindowViewModel);

            SetProfileListItems();

            Items.AddRange(new Screen[] { _hotkeyEditViewModel, _keyboardViewModel, _optionsViewModel });
        }



        protected override void OnViewLoaded(object view)
        {
            if (_application.Options.MinimizeOnStartUp)
            {
                ((Window)view).WindowState = WindowState.Minimized;
            }

            base.OnViewLoaded(view);
        }

        protected override async void OnViewReady(object view)
        {
            base.OnViewReady(view);

            await ActivateItemAsync(_hotkeyEditViewModel);
            await ActivateItemAsync(_keyboardViewModel);
            await ActivateItemAsync(_optionsViewModel);
        }

        public bool CanAddNewProfile
        {
            get
            {
                if (_profileManager.ProfileCount < ProfileManager.MAX_PROFILE_NUM)
                {
                    return true;
                }

                return false;
            }
            set { }
        }

        public void AddNewProfile(object sender, RoutedEventArgs e)
        {
            _profileManager.CreateNewProfile().Save();
            NotifyOfPropertyChange(() => CanAddNewProfile);
            SetProfileListItems();
        }

        public bool CanDeleteProfile
        {
            get
            {
                if (SelectedProfile != null)
                    return true;

                return false;
            }
            set { }
        }

        public async void DeleteProfile()
        {
            var currentProfile = SelectedProfile.Profile;

            CustomDialogBox dialogBox = new CustomDialogBox($"Are you sure to close {currentProfile.ProfileName}");
            dialogBox.ShowDialog();

            if (dialogBox.DialogResult.HasValue && dialogBox.DialogResult.Value)
            {
                HotkeyProfile deletedProfile = _profileManager.DeleteProfile(currentProfile.ProfileNum);

                SelectedProfile = null;
                NotifyOfPropertyChange(() => CanAddNewProfile);
                SetProfileListItems();

                await _eventAggregator.PublishOnUIThreadAsync(new ProfileDeletedEvent
                {
                    DeletedProfile = deletedProfile
                });
            }
        }

        public Task HandleAsync(HookStateChangeEvent message, CancellationToken cancellationToken)
        {
            if (message.HookState == HookState.Hooking)
                HookActivated = true;
            else
                HookActivated = false;

            return Task.CompletedTask;
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _windowsHookManager.Shutdown();

            var closeInfoTask = _infoWindowViewModel.TryCloseAsync();
            var baseTask = base.OnDeactivateAsync(close, cancellationToken);

            await Task.WhenAll(baseTask, closeInfoTask);
        }

        private void SetProfileListItems()
        {
            ProfileStates.Clear();

            foreach (var profile in _profileManager.Profiles)
            {
                ProfileStates.Add(new ProfileStateModel(profile));
            }
        }

    }
}
