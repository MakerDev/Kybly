using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.UI.ViewModels
{
    public class KeyboardPageViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;

        //Key 눌렀을 때 이벤트를 발생시키기 위해
        public KeyboardPageViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
    }
}
