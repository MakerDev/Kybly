using AutoHotkeyRemaster.WPF.Events;
using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AutoHotkeyRemaster.WPF.Views
{
    /// <summary>
    /// Interaction logic for InfoWindowView.xaml
    /// </summary>
    public partial class InfoWindowView : Window, IHandle<InfoWindowStateChangedEvent>
    {
        private bool _isHidden = false;

        public InfoWindowView()
        {
            InitializeComponent();

            IoC.Get<IEventAggregator>().SubscribeOnUIThread(this);
        }

        public Task HandleAsync(InfoWindowStateChangedEvent message, CancellationToken cancellationToken)
        {
            if(_isHidden)
            {
                Show();
            }
            else
            {
                Hide();
            }

            _isHidden = !_isHidden;

            return Task.CompletedTask;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }       
    }
}
