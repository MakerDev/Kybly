using AutoHotkeyRemaster.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace AutoHotkeyRemaster.Services
{
    public class Options
    {
        public const string SAVE_NAME = "options";
        
        private const int VK_ESC = 27;

        private bool _saveLastInfoWindowPosition = true;
        public bool SaveLastInfoWindowPosition
        {
            get => _saveLastInfoWindowPosition;
            set
            {
                _saveLastInfoWindowPosition = value;
                JsonFileManager.Save(this, SAVE_NAME);
            }
        }

        private bool _minimizeOnStartup = false;
        public bool MinimizeOnStartUp
        {
            get { return _minimizeOnStartup; }
            set { 
                _minimizeOnStartup = value;
                JsonFileManager.Save(this, SAVE_NAME);
            }
        }

        private int _activationKey = VK_ESC;
        public int ActivationKey
        {
            get => _activationKey; 
            set
            {
                _activationKey = value;
                JsonFileManager.Save(this, SAVE_NAME);
            }
        }
    }
}
