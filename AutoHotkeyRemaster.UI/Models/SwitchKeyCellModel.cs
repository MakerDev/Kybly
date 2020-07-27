using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.WPF.Helpers;
using AutoHotkeyRemaster.WPF.Views.CustomControls;
using Caliburn.Micro;
using System;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Models
{
    public class SwitchKeyCellModel : PropertyChangedBase
    {
        private readonly ProfileSwitchKeyTableManager _profileSwitchKeyTableManager;
        private readonly int _fromIdx;
        private readonly int _toIdx;

        //Used to represent diagonal cells
        public bool CanEdit
        {
            get
            {
                return _fromIdx != _toIdx;
            }
        }

        public string SwitchKey
        {
            get
            {
                //TODO : change all conversion codes to use this converter
                return VirtualKeycodeToWpfKeyConverter.ConverFrom(_profileSwitchKeyTableManager.SwitchKeyTable[_fromIdx][_toIdx]);
            }
        }

        public SwitchKeyCellModel(ProfileSwitchKeyTableManager profileSwitchKeyTableManager, int from, int to)
        {
            _profileSwitchKeyTableManager = profileSwitchKeyTableManager;
            _fromIdx = from - 1;
            _toIdx = to - 1;
        }


        public async void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            Key key = e.ImeProcessedKey == Key.None ? e.Key : e.ImeProcessedKey;

            int vkCode = KeyInterop.VirtualKeyFromKey(key);

            int result = _profileSwitchKeyTableManager.SetSwitchKeyByIndex(_fromIdx, _toIdx, vkCode);

            CustomMessageDialog customMessageDialog;

            switch (result)
            {
                case -2:
                    customMessageDialog
                        = new CustomMessageDialog($"{key} is already reserved for activation key.");

                    customMessageDialog.ShowDialog();
                    break;

                case -1:
                    customMessageDialog
                        = new CustomMessageDialog($"{key} is already used for other switch key.");

                    customMessageDialog.ShowDialog();

                    break;

                case 0:
                    customMessageDialog
                        = new CustomMessageDialog($"{key} is already registerd as a hotkey of this profile");

                    customMessageDialog.ShowDialog();

                    break;

                default:
                    await _profileSwitchKeyTableManager.SaveTableAsync();
                    NotifyOfPropertyChange(() => SwitchKey);

                    break;
            }
        }
    }
}
