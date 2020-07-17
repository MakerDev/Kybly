using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoHotkeyRemaster.UI.Views.CustomControls
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
            ProfileEditor profileEditor = new ProfileEditor(txtBlock.Text);
            profileEditor.ShowDialog();

            txtBlock.Text = profileEditor.ChangedName;
        }
    }
}
