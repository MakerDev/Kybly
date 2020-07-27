using System;
using System.Globalization;
using System.Windows.Data;

namespace AutoHotkeyRemaster.WPF.Converters
{
    public class InstanceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "" : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
