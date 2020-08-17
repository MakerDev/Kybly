using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Helpers;
using AutoHotkeyRemaster.WPF.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    //INFO : As this viewmodel is manually set, name binding doesn't work.
    //Use explicit binding syntax
    public class OptionsViewModel : Screen
    {
        private readonly Options _options;
        private readonly ApplicationModel _applicationModel;
        private readonly IAsyncJsonFileManager _jsonSavefileManager;

        public int ActivationKey
        {
            get { return _options.ActivationKey; }
        }

        public bool SaveInfoWindowPosition
        {
            get
            {
                return _options.SaveLastInfoWindowPosition;
            }
            set
            {
                _options.SaveLastInfoWindowPosition = value;
                _jsonSavefileManager.SaveAsync(_options, "options")
                    .ContinueWith((task) => NotifyOfPropertyChange(() => SaveInfoWindowPosition));
            }
        }

        public bool MinimizeOnStartUp
        {
            get
            {
                return _options.MinimizeOnStartUp;
            }
            set
            {
                _options.MinimizeOnStartUp = value;
                _jsonSavefileManager.SaveAsync(_options, "options")
                    .ContinueWith((task) => NotifyOfPropertyChange(() => MinimizeOnStartUp));
            }
        }

        public int MouseUpDelayMiliseconds
        {
            get
            {
                return _options.MouseUpDelayMiliseconds;
            }
            set
            {
                _options.MouseUpDelayMiliseconds = value;
                _jsonSavefileManager.SaveAsync(_options, "options")
                    .ContinueWith((task) => NotifyOfPropertyChange(() => MouseUpDelayMiliseconds));
            }
        }

        public int MouseDownDelayMiliseconds
        {
            get
            {
                return _options.MouseDownDelayMiliseconds;
            }
            set
            {
                _options.MouseDownDelayMiliseconds = value;
                _jsonSavefileManager.SaveAsync(_options, "options")
                    .ContinueWith((task) => NotifyOfPropertyChange(() => MouseDownDelayMiliseconds));
            }
        }

        public OptionsViewModel(ApplicationModel applicationModel, IAsyncJsonFileManager jsonSavefileManager)
        {
            _options = applicationModel.Options;
            _applicationModel = applicationModel;
            _jsonSavefileManager = jsonSavefileManager;
        }

        public async void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var key = e.ImeProcessedKey == Key.None ? e.Key : e.ImeProcessedKey;
            var activationKey = KeyInterop.VirtualKeyFromKey(key);

            if (_applicationModel.SetActivationKey(activationKey))
            {
                await _jsonSavefileManager.SaveAsync(_options, "options");
                NotifyOfPropertyChange(() => ActivationKey);

                return;
            }

            CustomMessageDialog dialog = new CustomMessageDialog("This key is already registered in switchkey table!");
            dialog.ShowDialog();
        }
    }
}
