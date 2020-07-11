using AutoHotkeyRemaster.UI.Models;
using AutoHotkeyRemaster.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoHotkeyRemaster.UI.Views
{
    public partial class ProfileEditor : Window
    {
        public string ChangedName
        {
            get
            {
                return ProfileName.Text;
            }
            set { }
        }

        public ProfileEditor(string nameBefore)
        {
            InitializeComponent();
            ProfileName.Text = nameBefore;
        }
    }
}
