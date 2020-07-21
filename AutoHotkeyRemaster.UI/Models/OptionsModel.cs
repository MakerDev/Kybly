using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.WPF.Models
{
    public class OptionsModel
    {
        private readonly Options _options;
        private readonly IJsonSavefileManager _jsonSavefileManager;

        public bool SaveLastInfoWindowPosition
        {
            get
            {
                return _options.SaveLastInfoWindowPosition;
            }

            set
            {
                _options.SaveLastInfoWindowPosition = value;
                _jsonSavefileManager.Save(_options, "options");
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
                _jsonSavefileManager.Save(_options, "options");
            }
        }

        public int ActivationKey
        {
            get
            {
                return _options.ActivationKey;
            }

            set
            {
                _options.ActivationKey = value;
                _jsonSavefileManager.Save(_options, "options");
            }
        }

        public OptionsModel(Options options, IJsonSavefileManager jsonSavefileManager)
        {
            _options = options;
            _jsonSavefileManager = jsonSavefileManager;
        }
    }
}
