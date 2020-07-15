using AutoHotkeyRemaster.UI.Helpers;
using AutoHotkeyRemaster.UI.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoHotkeyRemaster.UI.Views
{
    /// <summary>
    /// Interaction logic for KeyboardPage.xaml
    /// </summary>
    public partial class KeyboardView : UserControl
    {
        private bool _subscibedEvent = false;

        public KeyboardView()
        {
            InitializeComponent();


        }

        private bool _isDictInitialized = false;
        private Dictionary<int, RadioButton> _keyButtonPairs = new Dictionary<int, RadioButton>();

        private void OnProfileChanged(object sender, EventArgs e)
        {
            if (!_isDictInitialized)
                RegisterButtonsToDict();

            var keyHotkeyPair = ((KeyboardViewModel)sender).TriggerHotkeyPairs;

            foreach (var keycode in _keyButtonPairs.Keys)
            {
                if (keyHotkeyPair.ContainsKey(keycode))
                {
                    _keyButtonPairs[keycode].Tag = "True";
                }
                else
                {
                    _keyButtonPairs[keycode].Tag = "False";
                }

                _keyButtonPairs[keycode].IsChecked = false;
            }
        }

        private void RegisterButtonsToDict()
        {
            var buttonGrid = (Grid)FindName("xGridKeyButtons");
            var children = buttonGrid.Children;

            //TODO : 여기서 태그와 바인딩을 세팅하는 방법도 고려가능            
            foreach (var item in children)
            {
                RadioButton button = item as RadioButton;
                int keycode = KeyConversionHelper.ExtractFromElementName((button.Name));

                _keyButtonPairs.Add(keycode, button);
            }

            _isDictInitialized = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!_subscibedEvent)
            {
                ((KeyboardViewModel)DataContext).PropertyChanged += OnProfileChanged;
                _subscibedEvent = true;
            }
        }
    }
}
