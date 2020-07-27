using System.Windows.Controls;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for ProfileListViewItem.xaml
    /// </summary>
    public partial class ProfileListViewItem : UserControl
    {
        //TODO : 다이얼로그 말고 더블 클릭시 Textblock -> Textbox로 변경해서 바로 바꿀 수 있게하자...
        public ProfileListViewItem()
        {
            InitializeComponent();
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProfileEditor profileEditor
                = new ProfileEditor(txtBlock.Text);
            profileEditor.ShowDialog();

            txtBlock.Text = profileEditor.ChangedName;
        }
    }
}
