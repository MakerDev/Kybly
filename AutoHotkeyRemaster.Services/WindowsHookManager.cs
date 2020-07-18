using AutoHotkeyRemaster.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoHotkeyRemaster.Services
{
    public class WindowsHookManager
    {
        #region Hook Constants
        public const int HOTKEY_ID = 9000;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;

        #endregion

        //Assume that trigger is not a combination but only a single key
        private Dictionary<int, Hotkey> _triggerHotkeyPairs { get; set; } = new Dictionary<int, Hotkey>();
        private Dictionary<int, bool> _isAlreadyPressed { get; set; } = new Dictionary<int, bool>();

        private readonly ProfileSwitchKeyTable _switchKeyTable;
        private readonly ProfileManager _profileManager;
        private readonly ApplicationModel _applicationModel;

        public WindowsHookManager(ProfileSwitchKeyTable switchKeyTable, ApplicationModel applicationModel, ProfileManager profileManager)
        {
            _switchKeyTable = switchKeyTable;
            _profileManager = profileManager;
            _applicationModel = applicationModel;

            _proc = HookCallback;
        }

        public void RegisterProfile(HotkeyProfile hotkeyProfile)
        {
            int[] currentSwitchKeys = _switchKeyTable[hotkeyProfile.ProfileNum];
        }

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

        public void StartHookKeyboard()
        {
            _hookId = SetHook(_proc);
        }

        public void StopHookKeyboard()
        {
            UnhookWindowsHookEx(_hookId);
        }


        //TODO : rename this
        public void ClearKeys()
        {
            _triggerHotkeyPairs.Clear();
            _isAlreadyPressed.Clear();
        }

        private void AddTriggerHotkeyPair(int input, Hotkey hotkey)
        {
            _triggerHotkeyPairs.Add(input, hotkey);
            _isAlreadyPressed.Add(input, false);
        }

        private void OnKeyPressed(int vkCode)
        {

        }

        private void OnKeyUp(int vkCode)
        {

        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (_triggerHotkeyPairs.Count == 0)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (!_triggerHotkeyPairs.ContainsKey(vkCode))
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);

                if (_isAlreadyPressed[vkCode] && IsMouseEvent(_triggerHotkeyPairs[vkCode].Action.Key))
                    return new IntPtr(5);

                OnKeyPressed(vkCode);

                return new IntPtr(5);
            }
            else if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                vkCode = ConvertVkcodeToInternalValue(vkCode);

                if (!_triggerHotkeyPairs.ContainsKey(vkCode))
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);

                OnKeyUp(vkCode);

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
