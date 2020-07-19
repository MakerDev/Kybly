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
        private const int VK_ESC = 27;

        public const string SAVE_NAME = "options";
        private bool _saveLastInfoWindowPosition = true;
        private int _activationKey = VK_ESC;

        public bool SaveLastInfoWindowPosition
        {
            get => _saveLastInfoWindowPosition;
            set
            {
                _saveLastInfoWindowPosition = value;
                JsonFileManager.Save(this, SAVE_NAME);
            }
        }

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
