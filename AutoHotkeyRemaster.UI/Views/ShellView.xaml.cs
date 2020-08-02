using AutoHotkeyRemaster.WPF.ViewModels;
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
        private bool _pausingHook = false;
        private System.Windows.Forms.ToolStripMenuItem _pauseMenuItem;
        private ShellViewModel _viewModel;

        //TODO : KeyboardView와 HotkeyEditView를 하나의 컨테이너 안에 두고, Stackpanel로 OptionsView와 Horizaontal로 두면
        //Options만 따로 VM을 바인딩하지 않아도 됨.
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

            SetNotifyIcon();
        }

        private void SetNotifyIcon()
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

            _pauseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            _pauseMenuItem.Text = "Pause Hook";
            _pauseMenuItem.Click += OnPauseItemClicked;

            _notiIcon.ContextMenuStrip = ResetTooltipContextMenu();
        }

        protected override void OnActivated(EventArgs e)
        {
            _viewModel = this.DataContext as ShellViewModel;

            base.OnActivated(e);
        }

        private System.Windows.Forms.ContextMenuStrip ResetTooltipContextMenu()
        {
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();

            contextMenu.Items.Clear();

            contextMenu.Items.Add(_pauseMenuItem);
            contextMenu.Items.Add("Setting", null, (s, e) => OpenWindow());
            contextMenu.Items.Add("Exit", null, (s, e) => Close());

            return contextMenu;
        }

        //HACK : Allow accessing to VM 
        private void OnPauseItemClicked(object sender, EventArgs e)
        {
            if (_pausingHook)
            {
                _pauseMenuItem.Text = "Pause Hook";
                _viewModel.ResumeHook();
            }
            else
            {
                _pauseMenuItem.Text = "Resume Hook";
                _viewModel.PauseHook();
            }

            _notiIcon.ContextMenuStrip = ResetTooltipContextMenu();

            _pausingHook = !_pausingHook;
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
