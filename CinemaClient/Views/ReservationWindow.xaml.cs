using CinemaClient.CinemaService;
using System.Windows;
using System.Windows.Controls;

namespace CinemaClient
{
    public partial class ReservationWindow : Window
    {
        public enum ReservationMode
        {
            Create,
            Edit,
            Summary
        }

        public ReservationMode Mode { get; set; }

        public ReservationItem ReservationItem { get; set; }
        public Authentication Authentication { get; set; }
        public CinemaServiceClient Service { get; set; }

        public ReservationWindow()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }

        public void UpdateView()
        {
            if (Mode == ReservationMode.Create || Mode == ReservationMode.Edit)
            {
                MakeReservationControl control = new MakeReservationControl();
                control.Context = this;
                control.ReservationItem = ReservationItem;
                control.Authentication = Authentication;
                control.Service = Service;
                ViewContentControl.Content = control;
            }
            else if (Mode == ReservationMode.Summary)
            {
                ReservationSummaryControl control = new ReservationSummaryControl();
                control.Context = this;
                control.ReservationItem = ReservationItem;
                ViewContentControl.Content = control;
            }
        }

        public void StartLoading()
        {
            ProgressBar.Visibility = Visibility.Visible;
        }

        public void StopLoading()
        {
            Dispatcher.Invoke(() => ProgressBar.Visibility = Visibility.Collapsed);
        }
    }
}
