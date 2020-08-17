using AutoHotkeyRemaster.Models;

namespace AutoHotkeyRemaster.Services.Events
{
    public class HookStateChangeEvent
    {
        public HookState HookState { get; set; }
        public HotkeyProfile HotkeyProfile { get; set; } = null;
    }
}
