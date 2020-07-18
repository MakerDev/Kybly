using AutoHotkeyRemaster.Models.Helpers;
using AutoHotkeyRemaster.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.Models
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

        public Options Options { get; set; }

        private ApplicationState _applicationState;
        public ApplicationState ApplicationState
        {
            get { return _applicationState; }
            set {
                _applicationState = value;
                ApplicationStateChange(value);
            }
        }

        public ApplicationModel()
        {
            Options = JsonFileManager.Load<Options>(Options.SAVE_NAME) ?? new Options();
        }        
    }
}
