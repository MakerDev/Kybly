using AutoHotkeyRemaster.WPF.Models;
using AutoHotkeyRemaster.WPF.ViewModels;
using MahApps.Metro.Controls;
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

namespace AutoHotkeyRemaster.WPF.Views
{
    public partial class ProfileEditor : MetroWindow
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
