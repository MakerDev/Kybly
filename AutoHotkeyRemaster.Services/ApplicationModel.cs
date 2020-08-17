using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services.Helpers;
using AutoHotkeyRemaster.Services.Interfaces;
using System.Threading.Tasks;

namespace AutoHotkeyRemaster.Services
{
    public enum ApplicationState
    {
        Editing,
        ReadyToRun,
        Activated,
    }

    public class ApplicationModel : IAsyncInitializationRequired
    {
        public delegate void AppStateChangeHandler(ApplicationState applicationState);
        public event AppStateChangeHandler ApplicationStateChange;

        public delegate void ActivationKeyChangeHandler();
        public event ActivationKeyChangeHandler ActivationKeyChange;

        public Options Options { get; private set; }

        private ApplicationState _applicationState;
        private readonly ProfileSwitchKeyTableManager _switchKeyTable;
        private readonly IAsyncJsonFileManager _jsonSavefileManager;


        public ApplicationState ApplicationState
        {
            get { return _applicationState; }
            set
            {
                _applicationState = value;
                ApplicationStateChange(value);
            }
        }

        public ApplicationModel(ProfileSwitchKeyTableManager switchKeyTable, IAsyncJsonFileManager jsonSavefileManager)
        {
            _switchKeyTable = switchKeyTable;
            _jsonSavefileManager = jsonSavefileManager;
        }

        public async Task InitializeAsync()
        {
            Options = await _jsonSavefileManager.LoadAsync<Options>(AppConstants.OPTION_SAVE_NAME).ConfigureAwait(false);

            if (Options == null) Options = new Options();
        }

        public bool SetActivationKey(int key)
        {
            if (_switchKeyTable.HasKey(key))
            {
                return false;
            }

            Options.ActivationKey = key;
            ActivationKeyChange?.Invoke();

            return true;
        }
    }
}
