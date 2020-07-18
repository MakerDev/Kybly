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

        public bool SaveLastInfoWindowPosition { get; set; } = true;
        public int ActivationKey { get; set; } = VK_ESC;
    }
}
