using AutoHotkeyRemaster.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    //INFO : As this viewmodel is manually set, name binding doesn't work.
    //Use explicit binding syntax
    public class OptionsViewModel : Screen
    {
        private readonly ApplicationModel _applicationModel;

        public OptionsViewModel(ApplicationModel applicationModel)
        {
            _applicationModel = applicationModel;
        }

    }
}
