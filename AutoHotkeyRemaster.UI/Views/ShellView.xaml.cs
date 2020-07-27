using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace AutoHotkeyRemaster.WPF.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {
        private readonly Storyboard _openProfilePanelStoryboard = new Storyboard();
        private System.Windows.Forms.NotifyIcon _notiIcon = new System.Windows.Forms.NotifyIcon();

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
            OptionsPanel.Width = 0;
        }

        protected override void OnActivated(EventArgs e)
        {
            //Set tray icon.
            _notiIcon.Icon = Properties.Resources.KyblyIcon;
            _notiIcon.Visible = true;
            _notiIcon.DoubleClick += (s, e) => OpenWindow();

            _notiIcon.BalloonTipClosed += (sender, e) =>
            {
                var icon = (System.Windows.Forms.NotifyIcon)sender;
                icon.Visible = false;
                icon.Dispose();
            };

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();

            contextMenu.Items.Add("Setting", null, (s, e) => OpenWindow());
            contextMenu.Items.Add("Exit", null, (s, e) => Close());
            _notiIcon.ContextMenuStrip = contextMenu;

            base.OnActivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _notiIcon.Visible = false;
            _notiIcon.Dispose();

            base.OnClosing(e);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    Show();
                    break;

                case WindowState.Minimized:
                    Hide();
                    break;

                //Never happens
                case WindowState.Maximized:
                    break;

                default:
                    break;
            }

            base.OnStateChanged(e);
        }

        private void OpenWindow()
        {
            Show();
            this.WindowState = WindowState.Normal;
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;
            TextBlock txtBlock = listViewItem.Content as TextBlock;

            ProfileEditor profileEditor = new ProfileEditor(txtBlock.Text);
            profileEditor.ShowDialog();

            txtBlock.Text = profileEditor.ChangedName;
        }

        #region UI EVENTS
        private void ToggleMenuBtnUnchecked(object sender, RoutedEventArgs e)
        {
            if (ProfilesPanel.SelectedItem == null && MenuList.SelectedItem != null)
            {
                MenuList.SelectedItem = null;
            }

            ContentMask.Visibility = Visibility.Collapsed;
        }

        private void ToggleMenuBtnChecked(object sender, RoutedEventArgs e)
        {
            ContentMask.Visibility = Visibility.Visible;
        }

        private void OnBackgroundPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleButton.IsChecked = false;
            OptionButton.IsChecked = false;
        }

        private void ListViewItemPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleButton.IsChecked = true;
            _openProfilePanelStoryboard.Begin(NavPanel);
        }

        private void PreventKeyboardNavigation(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

    }
}
