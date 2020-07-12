using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.UI.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AutoHotkeyRemaster.UI.ViewModels
{
    public class KeyboardViewModel : Screen, IHandle<ProfileChangedEvent>
    {
        private readonly IEventAggregator _eventAggregator;

        private HotkeyProfile _profile;

        public HotkeyProfile Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                NotifyOfPropertyChange(() => Profile);
            }
        }

        //Key 눌렀을 때 이벤트를 발생시키기 위해
        public KeyboardViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        public Task HandleAsync(ProfileChangedEvent message, CancellationToken cancellationToken)
        {
            Profile = message.Profile;

            return Task.CompletedTask;
        }

        public void OnKeyClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
