using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Events;
using AutoHotkeyRemaster.Services.Helpers;
using AutoHotkeyRemaster.WPF.Events;
using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    public class InfoWindowViewModel : Screen, IHandle<HookStateChangeEvent>, IHandle<InfoWindowStateChangedEvent>
    {
        private string _hookingProfileName = "DEACTIVATED";
        private bool _isVisible = true;
        private readonly Options _options;
        private readonly IAsyncJsonFileManager _jsonFileManager;

        public int LeftPosition { get; set; } = 50;
        public int TopPosition { get; set; } = 50;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public string HookingProfileName
        {
            get { return _hookingProfileName; }
            set
            {
                _hookingProfileName = value;
                NotifyOfPropertyChange(() => HookingProfileName);
            }
        }

        public InfoWindowViewModel(IEventAggregator eventAggregator, ApplicationModel applicationModel, IAsyncJsonFileManager jsonFileManager)
        {
            eventAggregator.SubscribeOnUIThread(this);
            _options = applicationModel.Options;
            _jsonFileManager = jsonFileManager;

            LeftPosition = _options.LeftPosition;
            TopPosition = _options.TopPosition;
        }

        public Task HandleAsync(HookStateChangeEvent message, CancellationToken cancellationToken)
        {
            switch (message.HookState)
            {
                case HookState.Hooking:
                    HookingProfileName = message.HotkeyProfile.ProfileName;
                    break;

                case HookState.UnHooking:
                    HookingProfileName = "DEACTIVATED";
                    break;

                default:
                    break;
            }

            return Task.CompletedTask;
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (_options.SaveLastInfoWindowPosition)
            {
                _options.LeftPosition = LeftPosition;
                _options.TopPosition = TopPosition;
                await _jsonFileManager.SaveAsync(_options, "options");
            }

            await base.OnDeactivateAsync(close, cancellationToken);
        }

        public Task HandleAsync(InfoWindowStateChangedEvent message, CancellationToken cancellationToken)
        {
            IsVisible = !IsVisible;

            return Task.CompletedTask;
        }
    }
}
