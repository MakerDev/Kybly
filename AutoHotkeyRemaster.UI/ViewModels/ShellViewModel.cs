using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Events;
using AutoHotkeyRemaster.Services.Helpers;
using AutoHotkeyRemaster.WPF.Events;
using AutoHotkeyRemaster.WPF.Models;
using AutoHotkeyRemaster.WPF.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
        private readonly IAsyncJsonFileManager _jsonSavefileManager;
        private readonly InfoWindowViewModel _infoWindowViewModel;
        private ProfileStateModel _selectedProfile;
        public ProfileStateModel SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;

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

        private ObservableCollection<ProfileStateModel> _profileStates = new ObservableCollection<ProfileStateModel>();

        public ObservableCollection<ProfileStateModel> ProfileStates
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
                DisplayMask = value;
                NotifyOfPropertyChange(() => HookActivated);
            }
        }

        private bool _displayMask;

        public bool DisplayMask
        {
            get { return _displayMask; }
            set
            {
                _displayMask = value;
                NotifyOfPropertyChange(() => DisplayMask);
                NotifyOfPropertyChange(() => MaskMessage);
            }
        }

        public string MaskMessage
        {
            get
            {
                if (HookActivated)
                {
                    return "Cannot edit profile while activated.";
                }

                return "";
            }
        }

        public ShellViewModel(ProfileManager profileManager, IEventAggregator eventAggregator, IWindowManager windowManager,
            ApplicationModel application, KeyboardViewModel keyboardViewModel, WindowsHookManager windowsHookManager,
            HotkeyEditViewModel hotkeyEditViewModel, OptionsViewModel optionsViewModel, IAsyncJsonFileManager jsonSavefileManager,
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
            _jsonSavefileManager = jsonSavefileManager;
            _infoWindowViewModel = infoWindowViewModel;

            _eventAggregator.SubscribeOnUIThread(this);

            Items.AddRange(new Screen[] { _hotkeyEditViewModel, _keyboardViewModel, _optionsViewModel });

            if (_profileManager.ProfileCount == 0)
            {
                AddNewProfile();
            }
            else
            {
                SetProfileListItems();
            }

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
            await _windowManager.ShowWindowAsync(_infoWindowViewModel);
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

        public void AddNewProfile()
        {
            _profileManager.CreateNewProfile();

            SetProfileListItems();
            NotifyOfPropertyChange(() => CanAddNewProfile);
        }

        public async void EditSwitchKeyTable()
        {
            var vm = IoC.Get<SwitchKeyTableWindowViewModel>();

            DisplayMask = true;

            await _windowManager.ShowDialogAsync(vm);
            await vm.TryCloseAsync();

            if(HookActivated == false)           
                DisplayMask = false;
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

            CustomDialogBox dialogBox
                = new CustomDialogBox($"Are you sure to close {currentProfile.ProfileName}");
            dialogBox.ShowDialog();

            if (dialogBox.DialogResult.HasValue && dialogBox.DialogResult.Value)
            {
                HotkeyProfile deletedProfile = await _profileManager.DeleteProfileAsync(currentProfile.ProfileNum);

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

        public void PauseHook()
        {
            _windowsHookManager.PauseHook();
        }

        public void ResumeHook()
        {
            _windowsHookManager.ResumeHook();
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
            foreach (var stateModel in ProfileStates)
            {
                stateModel.PropertyChanged -= OnProfileModelPropertyChanged;
            }

            ProfileStates.Clear();

            foreach (var profile in _profileManager.Profiles)
            {
                var profileModel = new ProfileStateModel(profile, _jsonSavefileManager);
                profileModel.PropertyChanged += OnProfileModelPropertyChanged;
                ProfileStates.Add(profileModel);
            }
        }

        private async void OnProfileModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ProfileStateModel stateModel = sender as ProfileStateModel;

            if (stateModel == _selectedProfile && e.PropertyName == nameof(stateModel.ProfileName))
            {
                await _eventAggregator.PublishOnUIThreadAsync(new ProfileNameChangedEvent
                {
                    NewName = stateModel.ProfileName
                });
            }
        }
    }
}
