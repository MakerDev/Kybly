namespace AutoHotkeyRemaster.Models
{
    public interface IVirtualKeycodeToStringConverter
    {
        string Convert(int keycode);
    }
}