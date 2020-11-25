using AutoHotkeyRemaster.Models.MacroFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoHotkeyRemaster.Models.Helpers
{
    public static class GlobalCommands
    {
        //HACK
        public static Func<string, Task<MacroAction>> OpenActionEditorAsync { get; set; }
    }
}
