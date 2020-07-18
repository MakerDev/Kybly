using AutoHotkeyRemaster.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.WPF.Events
{
    public class ProfileChangedEvent
    {
        public HotkeyProfile Profile { get; set; }
    }
}
