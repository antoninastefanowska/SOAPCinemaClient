using CinemaClient.CinemaService;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace CinemaClient
{
    public partial class FilmWindow : Window
    {
        public Showing Showing { get; set; }
        public Authentication Authentication { get; set; }
        public CinemaServiceClient Service { get; set; }

        public FilmWindow()
        {
            InitializeComponent();
            Service = new CinemaServiceClient();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFilmData();
        }

        private void StartLoading()
        {
            ProgressBar.Visibility = Visibility.Visible;
            MakeReservationButton.IsEnabled = false;
        }

        private void StopLoading()
        {
            ProgressBar.Visibility = Visibility.Collapsed;
            MakeReservationButton.IsEnabled = true;
        }

        private async void LoadFilmData()
        {
            StartLoading();
            try
            {
                GetFilmResponse response = await Service.GetFilmAsync(Showing.FilmID);
                Film film = response.@return;
                FilmGrid.DataContext = film;
            }
            catch (FaultException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }

        private void MakeReservationButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationWindow reservationWindow = new ReservationWindow();
            reservationWindow.Owner = this;
            reservationWindow.ReservationItem = new ReservationItem(null, Showing);
            reservationWindow.Authentication = Authentication;
            reservationWindow.Service = Service;
            reservationWindow.Mode = ReservationWindow.ReservationMode.Create;
            reservationWindow.Show();
        }
    }
}
