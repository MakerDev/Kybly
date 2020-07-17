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
        private readonly Storyboard _openProfilePanelStoryboard = new Storyboard();

        public ShellView()
        {
            InitializeComponent();

            DoubleAnimation openProfilePanelAnimation = new DoubleAnimation
            {
                To = 230 + 250,
                Duration = new Duration(TimeSpan.FromMilliseconds(400))
            };

            Storyboard.SetTargetName(openProfilePanelAnimation, NavPanel.Name);
            Storyboard.SetTargetProperty(openProfilePanelAnimation, new PropertyPath(Grid.WidthProperty));

            _openProfilePanelStoryboard.Children.Add(openProfilePanelAnimation);

            NavPanel.Width = 65;
        }

        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            if(ProfilesPanel.SelectedItem == null && MenuList.SelectedItem != null)
            {
                MenuList.SelectedItem = null;
            }

            ContentMask.Visibility = Visibility.Collapsed;
        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            ContentMask.Visibility = Visibility.Visible;
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleButton.IsChecked = false;
        }

        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
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
