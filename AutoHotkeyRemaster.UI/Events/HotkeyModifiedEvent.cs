using AutoHotkeyRemaster.Models;

namespace AutoHotkeyRemaster.WPF.Events
{
    public enum EHotkeyModifiedEvent
    {
        Added,
        Modified,
        Deleted,
    }

    public class HotkeyModifiedEvent
    {
        public EHotkeyModifiedEvent ModifiedEvent { get; set; }
        public Hotkey Hotkey { get; set; }
    }
}
