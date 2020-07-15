using AutoHotkeyRemaster.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.UI.Events
{
    public class ProfileDeletedEvent
    {
        public HotkeyProfile DeletedProfile { get; set; }
    }
}
