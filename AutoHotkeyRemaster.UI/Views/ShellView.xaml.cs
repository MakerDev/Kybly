using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoHotkeyRemaster.UI.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        private Storyboard _openProfilePanelStoryboard = new Storyboard();

        public ShellView()
        {
            InitializeComponent();

            DoubleAnimation openProfilePanelAnimation = new DoubleAnimation();
            openProfilePanelAnimation.To = 230 + 250;
            openProfilePanelAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(400));

            Storyboard.SetTargetName(openProfilePanelAnimation, NavPanel.Name);
            Storyboard.SetTargetProperty(openProfilePanelAnimation, new PropertyPath(Grid.WidthProperty));

            _openProfilePanelStoryboard.Children.Add(openProfilePanelAnimation);

            //TODO : delete this
            NavPanel.Width = 65;
        }

        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            //TODO : 리스트뷰 선택된거 있으면 색깔들 다 원상복귀->초기화 루틴   
            MenuList.SelectedItem = null;
            BackgroundImage.Opacity = 1;
        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            BackgroundImage.Opacity = 0.3;
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleButton.IsChecked = false;
        }

        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //TODO : sender 정보 이용해서 누구한테 색칠해줘야할지 파악.
            //내용 바인딩은 ViewModel에서
            ToggleButton.IsChecked = true;
            _openProfilePanelStoryboard.Begin(NavPanel);
        }

        private void PreventKeyboardNavigation(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;
            TextBlock txtBlock = listViewItem.Content as TextBlock;

            ProfileEditor profileEditor = new ProfileEditor(txtBlock.Text);
            profileEditor.ShowDialog();

            txtBlock.Text = profileEditor.ChangedName;
        }
    }
}
