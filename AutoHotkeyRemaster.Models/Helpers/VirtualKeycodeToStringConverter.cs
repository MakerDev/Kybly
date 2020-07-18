using System;

namespace AutoHotkeyRemaster.Models.Helpers
{
    internal class VirtualKeycodeToStringConverter : IVirtualKeycodeToStringConverter
    {
        public string Convert(int keycode)
        {
            return ((ConsoleKey)keycode).ToString();
        }
    }
}
