using AutoHotkeyRemaster.Models.MacroFeatures;
using System.Collections.Generic;

namespace AutoHotkeyRemaster.Models
{
    public class Hotkey
    {
        public string Explanation { get; set; } = "";

        public HotkeyActionType HotkeyActionType { get; set; } = HotkeyActionType.Hotkey;
        public MacroAction MacroAction { get; set; } 

        public KeyInfo Trigger { get; set; }
        public KeyInfo Action { get; set; }
        public KeyInfo EndingAction { get; set; }

        /// <summary>
        /// This is just for json.net to work. Don't use this.
        /// </summary>
        public Hotkey()
        {
            Trigger = null;
            Action = null;
            EndingAction = null;
        }

        public Hotkey(KeyInfo trigger, KeyInfo action, KeyInfo endingAction = null)
        {
            Trigger = trigger;
            Action = action;
            EndingAction = endingAction;
        }
    }
}
