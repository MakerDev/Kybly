using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Models.MacroFeatures;
using AutoHotkeyRemaster.WPF.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    public class MacroEditViewModel : Screen, IHandle<KeySelectedEvent>, IHandle<ProfileChangedEvent>
    {
        public string Name { get; set; } = "Madsf";

        private HotkeyProfile _currentProfile = null;
        public HotkeyProfile CurrentProfile
        {
            get
            {
                return _currentProfile;
            }
            set
            {
                _currentProfile = value;
                NotifyOfPropertyChange(() => CurrentProfile);
            }
        }

        private Hotkey _hotkey;

        public Hotkey Hotkey
        {
            get { return _hotkey; }
            set
            {
                _hotkey = value;
                MacroAction = _hotkey.MacroAction;
                NotifyOfPropertyChange(() => Hotkey);
            }
        }

        private MacroAction _macroAction;

        public MacroAction MacroAction
        {
            get { return _macroAction; }
            set
            {
                _macroAction = value;
                Hotkey.MacroAction = value;
                NotifyOfPropertyChange(() => MacroAction);
            }
        }

        private bool _isMacroMode = false;

        public bool IsMacroMode
        {
            get { return _isMacroMode; }
            set
            {
                _isMacroMode = value;
                NotifyOfPropertyChange(() => IsMacroMode);
            }
        }

        //Hack of Hack: DO not try this at work.
        public HotkeyEditViewModel HotkeyEditViewModel { get; set; }

        public MacroEditViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.SubscribeOnUIThread(this);
        }


        public async void AddNewMacro()
        {
            if (Hotkey == null)
            {
                return;
            }

            MacroAction = new FileChangeAction();
            await HotkeyEditViewModel.SaveOrEditAsync(Hotkey);
        }

        public Task HandleAsync(KeySelectedEvent message, CancellationToken cancellationToken)
        {
            Hotkey = message.Hotkey;

            return Task.CompletedTask;
        }

        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            CurrentProfile = message.Profile;

            return Task.CompletedTask;
        }
    }
}
