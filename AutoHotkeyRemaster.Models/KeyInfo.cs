using AutoHotkeyRemaster.Models.Helpers;
using System.Text.Json.Serialization;

namespace AutoHotkeyRemaster.Models
{
    public class KeyInfo
    {
        //Replace this with respect to WPF or UWP
        [JsonIgnore]
        public static IVirtualKeycodeToStringConverter VirtualKeycodeToStringConverter { get; set; }
            = new VirtualKeycodeToStringConverter();

        //만약 key==0이라면 Z=Shift처럼 특수키 매핑인 것으로 간주함
        public int Key { get; set; }    //윈도우 virtualKeyCode값
        public int Modifier { get; set; } //Modifiers 값

        public KeyInfo()
        {
            Key = -1;
            Modifier = 0;
        }

        public KeyInfo(int key, int modifier)
        {
            Key = key;
            Modifier = modifier;
        }

        public override string ToString()
        {
            string info = "";

            //마우스 이벤트일 경우
            if (Key >= 1 && Key <= 4)
            {
                int mouseMod = Modifier / 100;
                int mods = Modifier - mouseMod * 100;

                info += GetModifiersInfo(mods);
                info += GetMouseEventExplanation(Key, mouseMod);
            }
            else
            {
                info += GetModifiersInfo(Modifier);
                //INFO : .netstandard를 위해 이렇게 바꿨다. 호출자쪽에서 캐스팅해라.
                info += VirtualKeycodeToStringConverter.Convert(Key);
            }

            return info;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is KeyInfo))
                return false;

            KeyInfo info = obj as KeyInfo;

            if (Key == info.Key && Modifier == info.Modifier)
                return true;
            else
                return false;
        }

        //INFO : 왜 해시코드도 같이 오버라이드 하며, 이런식으로 오버라이드 가능한 지 공부
        public override int GetHashCode()
        {
            return Key ^ Modifier;
        }

        private static string GetMouseEventExplanation(int button, int mouseEvent)
        {
            string explanation = "";

            if (button == 1) { explanation = "LeftMouseButton"; }
            if (button == 2) { explanation = "RightMouseButton"; }
            if (button == 4) { explanation = "MiddleMouseButton"; }

            if (mouseEvent == MouseEvents.Click) { explanation += "\nClick"; }
            if (mouseEvent == MouseEvents.DoubleClick) { explanation += "\nDouble Click"; }
            if (mouseEvent == MouseEvents.Down) { explanation += "\nDown"; }

            return explanation;
        }

        private string GetModifiersInfo(int modifier)
        {
            string info = "";


            if ((modifier & Modifiers.Ctrl) == Modifiers.Ctrl)
            {
                info += "Ctrl + ";
            }
            if ((modifier & Modifiers.Alt) == Modifiers.Alt)
            {
                info += "Alt + ";
            }
            if ((modifier & Modifiers.Shift) == Modifiers.Shift)
            {
                info += "Shift + ";
            }
            if ((modifier & Modifiers.Win) == Modifiers.Win)
            {
                info += "Win + ";
            }

            return info;
        }

    }
}
