using AutoHotkeyRemaster.WPF.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Converters
{
    public class KeycodeToStringConverter : IValueConverter
    {
        private KeyConverter _converter = new KeyConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int keycode = (int)value;

            //If this is mouse event
            if (keycode == 1) { return "LButton"; }
            if (keycode == 2) { return "RButton"; }
            if (keycode == 4) { return "MButton"; }

            return VirtualKeycodeToWpfKeyConverter.ConverFrom((int)value);
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
