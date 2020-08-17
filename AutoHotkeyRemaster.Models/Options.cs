namespace AutoHotkeyRemaster.Services
{
    public class Options
    {
        private const int VK_ESC = 27;

        public int LeftPosition { get; set; } = 50;
        public int TopPosition { get; set; } = 50;
        public bool SaveLastInfoWindowPosition { get; set; } = true;
        public bool MinimizeOnStartUp { get; set; } = false;
        public int ActivationKey { get; set; } = VK_ESC;
        public int MouseUpDelayMiliseconds { get; set; } = 50;
        public int MouseDownDelayMiliseconds { get; set; } = 50;
    }
}
