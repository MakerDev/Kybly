using AutoHotkeyRemaster.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AutoHotkeyRemaster.UI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private BindableCollection<Button> _profileBtns = new BindableCollection<Button>();
        private readonly ProfileManager _profileManager;

        public BindableCollection<Button> ProfileBtns
        {
            get { return _profileBtns; }
            set
            {
                _profileBtns = value;
                NotifyOfPropertyChange(() => ProfileBtns);
            }
        }

        public ShellViewModel(ProfileManager profileManager)
        {
            _profileManager = profileManager;
            
            for (int i = 0; i < 5; i++)
            {
                ProfileBtns.Add(createProfileButton(i));
            }
        }



        private Button createProfileButton(int profileNum)
        {
            Button profileBtn = new Button();

            profileBtn.Content = "Profile " + profileNum.ToString();
            profileBtn.Margin = new Thickness(0, 0, 0, 5);
            profileBtn.Width = 330;
            profileBtn.Height = 35;
            profileBtn.Click += OnProfileClicked;

            return profileBtn;
        }

        private void OnProfileClicked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            
        }
    }
}
