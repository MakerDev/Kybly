using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.UI.Helpers
{
    public static class KeyConversionHelper
    {
        public static int ExtractFromElementName(string name)
        {
            return Convert.ToInt32(name.Substring(4));
        }
    }
}
