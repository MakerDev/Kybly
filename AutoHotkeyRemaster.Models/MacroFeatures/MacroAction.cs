using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.Models.MacroFeatures
{
    public class MacroAction
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual void Run()
        {

        }
    }
}
