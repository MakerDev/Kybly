using AutoHotkeyRemaster.Models;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsInput;
using WindowsInput.Native;

namespace AutoHotkeyRemaster.Services.Helpers
{
    public static class InputSimlationHelper
    {
        private static InputSimulator _inputSimulator = new InputSimulator();


        public static void DownMouse(int mouseKey)
        {
            switch ((VirtualKeyCode)mouseKey)
            {
                case VirtualKeyCode.LBUTTON:
                    _inputSimulator.Mouse.LeftButtonDown();
                    break;

                case VirtualKeyCode.RBUTTON:
                    _inputSimulator.Mouse.RightButtonDown();
                    break;

                case VirtualKeyCode.MBUTTON:
                    _inputSimulator.Mouse.MiddleButtonDown();
                    break;

                default:
                    break;
            }
        }

        public static void UpMouse(int mouseKey)
        {
            switch ((VirtualKeyCode)mouseKey)
            {
                case VirtualKeyCode.LBUTTON:
                    _inputSimulator.Mouse.LeftButtonUp();
                    break;

                case VirtualKeyCode.RBUTTON:
                    _inputSimulator.Mouse.RightButtonUp();
                    break;

                case VirtualKeyCode.MBUTTON:
                    _inputSimulator.Mouse.MiddleButtonUp();
                    break;

                default:
                    break;
            }
        }

        public static void ClickMouse(int mouseKey)
        {
            switch ((VirtualKeyCode)mouseKey)
            {
                case VirtualKeyCode.LBUTTON:
                    _inputSimulator.Mouse.LeftButtonClick();
                    break;

                case VirtualKeyCode.RBUTTON:
                    _inputSimulator.Mouse.RightButtonClick();
                    break;

                case VirtualKeyCode.MBUTTON:
                    _inputSimulator.Mouse.MiddleButtonClick();
                    break;

                default:
                    break;
            }
        }

        public static void DoubleClickMouse(int mouseKey)
        {
            switch ((VirtualKeyCode)mouseKey)
            {
                case VirtualKeyCode.LBUTTON:
                    _inputSimulator.Mouse.LeftButtonDoubleClick();
                    break;

                case VirtualKeyCode.RBUTTON:
                    _inputSimulator.Mouse.RightButtonDoubleClick();
                    break;

                case VirtualKeyCode.MBUTTON:
                    _inputSimulator.Mouse.MiddleButtonDoubleClick();
                    break;

                default:
                    break;
            }
        }

        public static void DownKey(KeyInfo key)
        {
            var modifiers = GetModifierList(key.Modifier);

            DownModifiers(modifiers);

            switch (key.MouseEvent)
            {
                case EMouseEvents.None:
                    _inputSimulator.Keyboard.KeyDown((VirtualKeyCode)key.Key);
                    break;

                case EMouseEvents.Click:
                    ClickMouse(key.Key);
                    break;

                case EMouseEvents.DoubleClick:
                    DoubleClickMouse(key.Key);
                    break;

                case EMouseEvents.Down:
                    DownMouse(key.Key);
                    break;

                default:
                    break;
            }
        }

        public static void UpKey(KeyInfo key)
        {
            var modifiers = GetModifierList(key.Modifier);

            switch (key.MouseEvent)
            {
                case EMouseEvents.None:
                    _inputSimulator.Keyboard.KeyUp((VirtualKeyCode)key.Key);
                    break;

                case EMouseEvents.Down:
                    UpMouse(key.Key);
                    break;

                default:
                    break;
            }

            UpModifiers(modifiers);
        }

        public static void PressKey(KeyInfo key)
        {
            var modifiers = GetModifierList(key.Modifier);

            _inputSimulator.Keyboard.ModifiedKeyStroke(modifiers, (VirtualKeyCode)key.Key);
        }

        public static void DownModifiers(List<VirtualKeyCode> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                _inputSimulator.Keyboard.KeyDown(modifier);
            }
        }

        public static void UpAllModifiers()
        {
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.MENU);
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LWIN);
        }

        public static void UpModifiers(List<VirtualKeyCode> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                _inputSimulator.Keyboard.KeyUp(modifier);
            }
        }

        public static List<VirtualKeyCode> GetModifierList(int modifier)
        {
            List<VirtualKeyCode> modifiers = new List<VirtualKeyCode>();

            if ((modifier & Modifiers.Ctrl) != 0) { modifiers.Add(VirtualKeyCode.CONTROL); }
            if ((modifier & Modifiers.Shift) != 0) { modifiers.Add(VirtualKeyCode.SHIFT); }
            if ((modifier & Modifiers.Alt) != 0) { modifiers.Add(VirtualKeyCode.MENU); }
            if ((modifier & Modifiers.Win) != 0) { modifiers.Add(VirtualKeyCode.LWIN); }

            return modifiers;
        }
    }
}
