using MahApps.Metro.Controls;

namespace AutoHotkeyRemaster.WPF.Views
{
    public partial class ProfileEditor : MetroWindow
    {
        public string ChangedName
        {
            get
            {
                return ProfileName.Text;
            }
            set { }
        }

        public ProfileEditor(string nameBefore)
        {
            InitializeComponent();
            ProfileName.Text = nameBefore;
        }
    }
}
