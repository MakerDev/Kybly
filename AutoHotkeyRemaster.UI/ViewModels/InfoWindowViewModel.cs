using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    public class InfoWindowViewModel : Screen, IHandle<HookStateChangeEvent>
    {
        private string _hookingProfileName = "DEACTIVATED";
        public string HookingProfileName
        {
            get { return _hookingProfileName; }
            set
            {
                _hookingProfileName = value;
                NotifyOfPropertyChange(() => HookingProfileName);
            }
        }

        public InfoWindowViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.SubscribeOnUIThread(this);
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
    }
}
