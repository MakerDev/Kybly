using AutoHotkeyRemaster.Models.Helpers;
using AutoHotkeyRemaster.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.Services
{
    public enum ApplicationState
    {
        Editing,
        ReadyToRun,
        Activated,
    }
    
    public class ApplicationModel
    {
        public delegate void AppStateChangeHandler(ApplicationState applicationState);
        public event AppStateChangeHandler ApplicationStateChange;

        public delegate void ActivationKeyChangeHandler();
        public event ActivationKeyChangeHandler ActivationKeyChange;

        public Options Options { get; private set; }

        private ApplicationState _applicationState;
        private readonly ProfileSwitchKeyTable _switchKeyTable;

        public ApplicationState ApplicationState
        {
            get { return _applicationState; }
            set {
                _applicationState = value;
                ApplicationStateChange(value);
            }
        }

        public ApplicationModel(ProfileSwitchKeyTable switchKeyTable)
        {
            Options = JsonFileManager.Load<Options>(Options.SAVE_NAME) ?? new Options();
            _switchKeyTable = switchKeyTable;
        }

        public bool SetActivationKey(int key)
        {
            if(_switchKeyTable.HasKey(key))
            {
                return false;
            }

            Options.ActivationKey = key;
            ActivationKeyChange?.Invoke();

            return true;
        }
    }
}
