﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
        public KeyboardView()
        {
            InitializeComponent();
        }

        //TODO : remove this and relpace with Command
        private void OnKeyClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}