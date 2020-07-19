using AutoHotkeyRemaster.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.Services.Events
{
    public class HookStateChangeEvent
    {
        public HookState HookState { get; set; }
        public HotkeyProfile HotkeyProfile { get; set; } = null;
    }
}
