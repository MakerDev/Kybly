using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace AutoHotkeyRemaster.Services
{
    public enum HookState
    {
        Hooking,   //A profile is being hooked
        UnHooking, //Only activation key is enabled
    }

    /// <summary>
    /// Register keys to WindowsHookhandler. 
    /// Process hotkey is triggerd by WindowsHookhandler
    /// </summary>
    public class WindowsHookManager
    {
        public HookState HookState { get; set; } = HookState.UnHooking;

        private readonly IEventAggregator _eventAggregator;
        private readonly ApplicationModel _applicationModel;
        private readonly ProfileManager _profileManager;
        private readonly ProfileSwitchKeyTable _profileSwitchKeyTable;
        private readonly WindowsKeyboardHooker _keyboardHooker = new WindowsKeyboardHooker();
        private readonly InputSimulator _inputSimulator = new InputSimulator();
        
        private int _activationKey => _applicationModel.Options.ActivationKey;
        private readonly Dictionary<int, int> _swtichKeys = new Dictionary<int, int>(); //Key : switch keycode, Value : profile to switch
        private readonly Dictionary<int, Hotkey> _profileHotkeys = new Dictionary<int, Hotkey>();  //Key : trigger, Value : Hotkey

        private HotkeyProfile _currentHookingProfile = null;
        private int _lastHookedProfileNum = -1;

        public WindowsHookManager(IEventAggregator eventAggregator,
            ApplicationModel applicationModel, ProfileManager profileManager, ProfileSwitchKeyTable profileSwitchKeyTable)
        {
            _eventAggregator = eventAggregator;
            _applicationModel = applicationModel;
            _profileManager = profileManager;
            _profileSwitchKeyTable = profileSwitchKeyTable;

            _applicationModel.ActivationKeyChange += OnActivationKeyChanged;
            _keyboardHooker.KeyHooked += HandleHookedEvent;

            _keyboardHooker.StartHook(_activationKey, null);
        }

        private void OnActivationKeyChanged()
        {
            //Changing activation key means that current state is UnHooking
            _keyboardHooker.StopHook();
            _keyboardHooker.StartHook(_activationKey, null);
        }

        //Used to clean all key strokes. Useful in case of unexpected bugs
        public void Shutdown()
        {
            _keyboardHooker.StopHook();

            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.MENU);
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LWIN);
        }

        private void ChangeHookState()
        {
            if (HookState == HookState.UnHooking)
            {
                HookState = HookState.Hooking;

                Activate(_lastHookedProfileNum);

                return;
            }

            HookState = HookState.UnHooking;

            Deactivate();

            return;
        }

        private void Deactivate()
        {
            _keyboardHooker.StopHook();

            _lastHookedProfileNum = _currentHookingProfile.ProfileNum;

            //Delete cache for case where user modified profile hotkeys while deactivated
            _profileHotkeys.Clear();
            _swtichKeys.Clear();
            _currentHookingProfile = null;

            //Only enables activation key            
            _keyboardHooker.StartHook(_activationKey, null);

            _eventAggregator.PublishOnUIThreadAsync(new HookStateChangeEvent
            {
                HookState = HookState.UnHooking,
            });
        }

        /// <summary>
        /// On the first activation, activates default profile. (profile1)
        /// </summary>
        /// <returns>Activated profile</returns>
        private void Activate(int profileNum)
        {
            _currentHookingProfile = _profileManager.FindProfileOrDefault(profileNum);

            var switchKeys = _profileSwitchKeyTable[_currentHookingProfile.ProfileNum-1];

            for (int i = 1; i <= _profileManager.ProfileCount; i++)
            {
                if (i == _currentHookingProfile.ProfileNum)
                    continue;

                _swtichKeys.Add(switchKeys[i-1], i);
            }

            foreach (var hotkey in _currentHookingProfile.Hotkeys)
            {
                _profileHotkeys.Add(hotkey.Trigger.Key, hotkey);
            }

            var registeredKeys = CreateRegisteredKeyDictionary();

            _keyboardHooker.StartHook(registeredKeys);

            _eventAggregator.PublishOnUIThreadAsync(new HookStateChangeEvent
            {
                HookState = HookState.Hooking,
                HotkeyProfile = _currentHookingProfile
            });
        }

        private Dictionary<int, KeyInfo> CreateRegisteredKeyDictionary()
        {
            Dictionary<int, KeyInfo> registeredKeys = new Dictionary<int, KeyInfo>();

            //Registraion order : activation key -> switichkey -> profile hotkeys
            registeredKeys.Add(_applicationModel.Options.ActivationKey, null);

            foreach (var key in _swtichKeys.Keys)
            {
                if (!registeredKeys.ContainsKey(key)) registeredKeys.Add(key, null);
            }

            foreach (var trigger in _profileHotkeys.Keys)
            {
                if (!registeredKeys.ContainsKey(trigger)) registeredKeys.Add(trigger, _profileHotkeys[trigger].Action);
            }

            return registeredKeys;
        }

        private void SwtichProfile(int toProfileNum)
        {
            Deactivate();
            Activate(toProfileNum);
        }

        private void ProcessHotkeyAction(Hotkey hotkey, bool isPressed)
        {
            var modifiers = GetModifierList(hotkey.Action.Modifier);

            _keyboardHooker.StopHookKeyboard();

            if (isPressed)
            {
                foreach (var modifier in modifiers)
                {
                    _inputSimulator.Keyboard.KeyDown(modifier);
                }

                _inputSimulator.Keyboard.KeyDown((VirtualKeyCode)hotkey.Action.Key);

                return;
            }

            foreach (var modifier in modifiers)
            {
                _inputSimulator.Keyboard.KeyUp(modifier);
            }

            _inputSimulator.Keyboard.KeyUp((VirtualKeyCode)hotkey.Action.Key);

            if (hotkey.EndingAction != null)
            {
                modifiers = GetModifierList(hotkey.EndingAction.Modifier);

                _inputSimulator.Keyboard.ModifiedKeyStroke(modifiers, (VirtualKeyCode)hotkey.EndingAction.Key);
            }

            _keyboardHooker.StartHookKeyboard();
        }
        private List<VirtualKeyCode> GetModifierList(int modifier)
        {
            List<VirtualKeyCode> modifiers = new List<VirtualKeyCode>();

            if ((modifier & Modifiers.Ctrl) != 0) { modifiers.Add(VirtualKeyCode.CONTROL); }
            if ((modifier & Modifiers.Shift) != 0) { modifiers.Add(VirtualKeyCode.SHIFT); }
            if ((modifier & Modifiers.Alt) != 0) { modifiers.Add(VirtualKeyCode.MENU); }
            if ((modifier & Modifiers.Win) != 0) { modifiers.Add(VirtualKeyCode.LWIN); }

            return modifiers;
        }
        private void HandleHookedEvent(KeyHookedArgs args)
        {
            int keycode = args.VkCode;

            if (args.IsPressed)
            {
                if (keycode == _activationKey)
                {
                    ChangeHookState();

                    return;
                }

                if (_swtichKeys.ContainsKey(keycode))
                {
                    SwtichProfile(_swtichKeys[keycode]);

                    return;
                }

                ProcessHotkeyAction(_profileHotkeys[keycode], true);

                return;
            }

            if (keycode == _activationKey || _swtichKeys.ContainsKey(keycode))
                return;

            ProcessHotkeyAction(_profileHotkeys[keycode], false);
        }
    }
}
