using System;

namespace AutoHotkeyRemaster.WPF.Helpers
{
    public static class KeyConversionHelper
    {
        public static int ExtractFromElementName(string name)
        {
            return Convert.ToInt32(name.Substring(4));
        }
    }
}
