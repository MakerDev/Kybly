using AutoHotkeyRemaster.Models.Helpers;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Helpers
{
    public class VirtualKeycodeToWpfKeyConverter : IVirtualKeycodeToStringConverter
    {
        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        public static char GetCharFromKey(int vkCode)
        {
            char ch = ' ';

            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)vkCode, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)vkCode, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }

            return ch;
        }

        public static string ConverFrom(int keycode)
        {
            if (keycode == -1)
                return "";

            string result = KeyInterop.KeyFromVirtualKey(keycode).ToString();

            if(result.StartsWith("Oem"))
            {
                result = GetCharFromKey(keycode).ToString();
            }

            return result;
        }

        public string Convert(int keycode)
        {
            return ConverFrom(keycode);
        }
    }
}
