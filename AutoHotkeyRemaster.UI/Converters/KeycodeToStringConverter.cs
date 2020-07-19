using AutoHotkeyRemaster.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Converters
{
    public class KeycodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return VirtualKeycodeToWpfKeyConverter.ConverFrom((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
