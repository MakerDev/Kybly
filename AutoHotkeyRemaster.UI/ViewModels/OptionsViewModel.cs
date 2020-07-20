﻿using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Events;
using AutoHotkeyRemaster.WPF.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.ViewModels
{
    //INFO : As this viewmodel is manually set, name binding doesn't work.
    //Use explicit binding syntax
    public class OptionsViewModel : Screen
    {
        private readonly Options _options;
        private readonly ApplicationModel _applicationModel;

        public int ActivationKey
        {
            get { return _options.ActivationKey; }            
        }

        public bool SaveInfoWindowPosition
        {
            get
            {
                return _options.SaveLastInfoWindowPosition;
            }
            set
            {
                _options.SaveLastInfoWindowPosition = value;
                NotifyOfPropertyChange(() => SaveInfoWindowPosition);
            }
        }

        public bool MinimizeOnStartUp
        {
            get
            {
                return _options.MinimizeOnStartUp;
            }
            set
            {
                _options.MinimizeOnStartUp = value;
                NotifyOfPropertyChange(() => MinimizeOnStartUp);
            }
        }
        public OptionsViewModel(ApplicationModel applicationModel)
        {
            _options = applicationModel.Options;
            _applicationModel = applicationModel;
        }

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            Key key = e.ImeProcessedKey == Key.None ? e.Key : e.ImeProcessedKey;
            int activationKey = KeyInterop.VirtualKeyFromKey(key);

            if(_applicationModel.SetActivationKey(activationKey))
            {
                NotifyOfPropertyChange(() => ActivationKey);

                return;
            }

            CustomMessageDialog dialog = new CustomMessageDialog("This key is already registered in switchkey table!");
        }
    }
}
