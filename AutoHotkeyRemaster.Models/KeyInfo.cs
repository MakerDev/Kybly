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

        public int Key { get; set; }    //윈도우 virtualKeyCode값
        public int Modifier { get; set; } //Modifiers 값
        public EMouseEvents MouseEvent { get; set; }

        public KeyInfo()
        {
            Key = -1;
            Modifier = 0;
            MouseEvent = EMouseEvents.None;
        }

        public KeyInfo(int key, int modifier, 
            EMouseEvents mouseEvent = EMouseEvents.None)
        {
            Key = key;
            Modifier = modifier;
            MouseEvent = mouseEvent;
        }

        public override string ToString()
        {
            string info = "";

            info += GetModifiersInfo(Modifier);

            //마우스 이벤트일 경우
            if (MouseEvent != EMouseEvents.None)
            {
                info += GetMouseEventExplanation(Key, MouseEvent);
            }
            else
            {
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
        public override int GetHashCode()
        {
            return Key ^ Modifier;
        }
        private static string GetMouseEventExplanation(int button, EMouseEvents mouseEvent)
        {
            string explanation = "";

            if (button == 1) { explanation = "Left Mouse Button"; }
            if (button == 2) { explanation = "Right Mouse Button"; }
            if (button == 4) { explanation = "Middle Mouse Button"; }

            if (mouseEvent == EMouseEvents.Click) { explanation += "\nClick"; }
            if (mouseEvent == EMouseEvents.DoubleClick) { explanation += "\nDouble Click"; }
            if (mouseEvent == EMouseEvents.Down) { explanation += "\nDown"; }

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
