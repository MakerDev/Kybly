using AutoHotkeyRemaster.Models;

namespace AutoHotkeyRemaster.WPF.Events
{
    public class KeySelectedEvent
    {
        public bool IsNew { get; set; }
        public Hotkey Hotkey { get; set; }
    }
}
