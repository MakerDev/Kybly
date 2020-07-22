using AutoHotkeyRemaster.Models.Helpers;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutoHotkeyRemaster.Services
{
    public enum ApplicationState
    {
        Editing,
        ReadyToRun,
        Activated,
    }
    
    //TODO : 이렇게 반드시 초기화 돼야하는 애들한테 인터페이스로 초기화 메서드와 초기화 여부를 구현하게 하고,
    //초기화 안 된 상태로 사용못하도록 하거나, ShellView에서 리플렉션으로 초기화 메서드를 불러주는 것도 괜찮을듯.
    public class ApplicationModel : IAsyncInitializationRequired
    {
        public delegate void AppStateChangeHandler(ApplicationState applicationState);
        public event AppStateChangeHandler ApplicationStateChange;

        public delegate void ActivationKeyChangeHandler();
        public event ActivationKeyChangeHandler ActivationKeyChange;

        public Options Options { get; private set; }

        private ApplicationState _applicationState;
        private readonly ProfileSwitchKeyTable _switchKeyTable;
        private readonly IAsyncJsonFileManager _jsonSavefileManager;

        
        public ApplicationState ApplicationState
        {
            get { return _applicationState; }
            set {
                _applicationState = value;
                ApplicationStateChange(value);
            }
        }

        public ApplicationModel(ProfileSwitchKeyTable switchKeyTable, IAsyncJsonFileManager jsonSavefileManager)
        {
            _switchKeyTable = switchKeyTable;
            _jsonSavefileManager = jsonSavefileManager;
        }

        public async Task InitializeAsync()
        {
            Options = await _jsonSavefileManager.LoadAsync<Options>("options");

            if (Options == null) Options = new Options();
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
