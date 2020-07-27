using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Events;
using AutoHotkeyRemaster.WPF.Models;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    public class SwitchKeyTableWindowViewModel : Screen, IHandle<HookStateChangeEvent>
    {
        public ObservableCollection<string> ProfileNames { get; set; }
            = new ObservableCollection<string>();

        public ObservableCollection<ProfileSwitchKeyTableRowModel> SwitchKeyRows { get; set; }
            = new ObservableCollection<ProfileSwitchKeyTableRowModel>();

        public SwitchKeyTableWindowViewModel(IEventAggregator eventAggregator,
            ProfileSwitchKeyTableManager profileSwitchKeyTable,
            ProfileManager profileManager)
        {
            eventAggregator.SubscribeOnUIThread(this);

            int profileCount = profileManager.ProfileCount;

            foreach (var profile in profileManager.Profiles)
            {
                ProfileNames.Add(profile.ProfileName);

                SwitchKeyRows.Add(new ProfileSwitchKeyTableRowModel(
                    profileSwitchKeyTable,
                    profile,
                    profileCount));
            }

        }

        public async Task HandleAsync(HookStateChangeEvent message, CancellationToken cancellationToken)
        {
            if (message.HookState == HookState.Hooking)
                await TryCloseAsync();
        }
    }
}
