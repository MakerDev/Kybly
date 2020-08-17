using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoHotkeyRemaster.WPF.Converters
{
    public class BooleanToWindowStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = (bool)value;

            if (isVisible)
            {
                return WindowState.Normal;
            }

            return WindowState.Minimized;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
