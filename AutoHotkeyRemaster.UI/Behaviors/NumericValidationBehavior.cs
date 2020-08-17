using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Behaviors
{
    public static class NumericValidationBehavior
    {
        public static bool GetOnlyAllowNumeric(DependencyObject obj)
        {
            return (bool)obj.GetValue(OnlyAllowNumericProperty);
        }

        public static void SetOnlyAllowNumeric(DependencyObject obj, bool value)
        {
            obj.SetValue(OnlyAllowNumericProperty, value);
        }

        // Using a DependencyProperty as the backing store for OnlyAllowNumeric.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnlyAllowNumericProperty =
            DependencyProperty.RegisterAttached("OnlyAllowNumeric",
                typeof(bool),
                typeof(NumericValidationBehavior),
                new PropertyMetadata(false, propertyChangedCallback: OnAllowNumericChanged));

        private static void OnAllowNumericChanged(DependencyObject view, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = view as TextBox;

            if (textBox == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textBox.PreviewTextInput += OnPreviewTextInput;
            }
            else
            {
                textBox.PreviewTextInput -= OnPreviewTextInput;
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isNumeric = IsTextAllowed(e.Text);

            e.Handled = !isNumeric;
        }
    }
}
