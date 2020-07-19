using AutoHotkeyRemaster.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Helpers
{
    public class VirtualKeycodeToWpfKeyConverter : IVirtualKeycodeToStringConverter
    {
        public static string ConverFrom(int keycode)
        {
            if (keycode == -1)
                return "";

            return KeyInterop.KeyFromVirtualKey(keycode).ToString();
        }

        public string Convert(int keycode)
        {
            if (keycode == -1)
                return "";

            return KeyInterop.KeyFromVirtualKey(keycode).ToString();
        }


    }
}
