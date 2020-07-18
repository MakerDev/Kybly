using AutoHotkeyRemaster.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Helpers
{
    public class VirtualKeycodeToWpfKeyConverter : IVirtualKeycodeToStringConverter
    {
        public string Convert(int keycode)
        {
            return KeyInterop.KeyFromVirtualKey(keycode).ToString();
        }
    }
}
