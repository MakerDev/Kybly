using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoHotkeyRemaster.Services
{
    public class KeyHookedArgs
    {
        public bool IsPressed { get; set; } = true;
        public int VkCode { get; set; } = -1;
    }

    public class WindowsKeyboardHooker
    {
        #region Hook Constants
        public const int HOTKEY_ID = 9000;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;

        #endregion

        #region DllImports
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        #endregion

        #region WINDOWS_HOOK Properties & functions
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        #endregion

        //Assume that trigger is not a combination but only a single key        
        private Dictionary<int, KeyInfo> _registeredKeys { get; set; } = new Dictionary<int, KeyInfo>();
        private Dictionary<int, bool> _isAlreadyPressed { get; set; } = new Dictionary<int, bool>();

        public delegate void HookEventHandler(KeyHookedArgs args);
        public event HookEventHandler KeyHooked;

        public WindowsKeyboardHooker()
        {
            _proc = HookCallback;
        }

        /// <summary>
        /// Start hook with only one key registered. 
        /// </summary>
        /// <param name="key"></param>
        public void StartHook(int key, KeyInfo action)
        {
            _registeredKeys.Add(key, action);
            _isAlreadyPressed.Add(key, false);

            StartHookKeyboard();
        }

        public void StartHook(Dictionary<int, KeyInfo> registeredKeys)
        {
            _registeredKeys = registeredKeys;

            foreach (var trigger in _registeredKeys.Keys)
            {
                _isAlreadyPressed[trigger] = false;
            }

            StartHookKeyboard();
        }

        public void StopHook()
        {
            _registeredKeys.Clear();
            _isAlreadyPressed.Clear();

            StopHookKeyboard();
        }

        //These two are for when processing InputSimulator to make no collision to occur.
        public void StartHookKeyboard() => _hookId = SetHook(_proc);
        public void StopHookKeyboard() => UnhookWindowsHookEx(_hookId);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (_registeredKeys.Count == 0 || nCode < 0)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);

            if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = ConvertVkcodeToInternalValue(Marshal.ReadInt32(lParam));

                if (!_registeredKeys.ContainsKey(vkCode))
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);

                //To avoid infinite mouse click
                if (_isAlreadyPressed[vkCode]
                    && (_registeredKeys[vkCode] != null && IsMouseEvent(_registeredKeys[vkCode].Key)))
                {
                    return new IntPtr(5);
                }

                KeyHooked?.Invoke(new KeyHookedArgs
                {
                    IsPressed = true,
                    VkCode = vkCode
                });

                _isAlreadyPressed[vkCode] = true;

                return new IntPtr(5);
            }
            else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
            {
                int vkCode = ConvertVkcodeToInternalValue(Marshal.ReadInt32(lParam));

                if (!_registeredKeys.ContainsKey(vkCode))
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);

                KeyHooked?.Invoke(new KeyHookedArgs
                {
                    IsPressed = false,
                    VkCode = vkCode
                });

                _isAlreadyPressed[vkCode] = false;

                return new IntPtr(5);
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
        private bool IsMouseEvent(int key) => (key >= 1 && key <= 4);
        private int ConvertVkcodeToInternalValue(int vkCode)
        {
            if (vkCode == 164 || vkCode == 165) { return Modifiers.Alt; }
            if (vkCode == 162 || vkCode == 163) { return Modifiers.Ctrl; }
            if (vkCode == 91 || vkCode == 92) { return Modifiers.Win; }
            if (vkCode == 160 || vkCode == 161) { return Modifiers.Shift; }

            return vkCode;
        }
    }
}
