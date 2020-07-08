using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.Models
{
    public class Hotkey
    {
        public string Explanation { get; set; } = "";

        public KeyInfo Trigger { get; set; }
        public KeyInfo Action { get; set; }
        public KeyInfo EndingAction { get; set; }

        public Hotkey(KeyInfo trigger, KeyInfo action, KeyInfo endingAction = null)
        {
            Trigger = trigger;
            Action = action;
            EndingAction = endingAction;
        }
    }
}
