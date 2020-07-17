using System;

namespace AutoHotkeyRemaster.Models
{
    internal class VirtualKeycodeToStringConverter : IVirtualKeycodeToStringConverter
    {
        public string Convert(int keycode)
        {
            return ((ConsoleKey)keycode).ToString();
        }
    }
}
