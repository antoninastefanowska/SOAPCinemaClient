using CinemaClient.CinemaService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace CinemaClient
{
    public partial class MainWindow : Window
    {
        public Authentication Authentication { get; set; }
        public CinemaServiceClient Service { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadShowingsData();
        }

        private void StartLoading()
        {
            ProgressBar.Visibility = Visibility.Visible;
        }

        private void StopLoading()
        {
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ShowShowingButton_Click(object sender, RoutedEventArgs e)
        {
            Showing showing = ((FrameworkElement)sender).DataContext as Showing;
            FilmWindow filmWindow = new FilmWindow();
            filmWindow.Owner = this;
            filmWindow.Showing = showing;
            filmWindow.Authentication = Authentication;
            filmWindow.Service = Service;
            filmWindow.Show();
        }

        private void ShowReservationButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationItem item = ((FrameworkElement)sender).DataContext as ReservationItem;
            ReservationWindow reservationWindow = new ReservationWindow();
            reservationWindow.Owner = this;
            reservationWindow.ReservationItem = item;
            reservationWindow.Mode = ReservationWindow.ReservationMode.Summary;
            reservationWindow.Show();
        }

        private void UpdateReservation_Click(object sender, RoutedEventArgs e)
        {
            ReservationItem item = ((FrameworkElement)sender).DataContext as ReservationItem;
            ReservationWindow reservationWindow = new ReservationWindow();
            reservationWindow.Owner = this;
            reservationWindow.ReservationItem = item;
            reservationWindow.Service = Service;
            reservationWindow.Authentication = Authentication;
            reservationWindow.Mode = ReservationWindow.ReservationMode.Edit;
            reservationWindow.Show();
        }

        private void CancelReservation_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.Ask(this, "Czy na pewno chcesz anulować wybraną rezerwację?"))
            {
                ReservationItem item = ((FrameworkElement)sender).DataContext as ReservationItem;
                CancelReservation(item.Reservation);
            }
        }

        private void Tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && ReservationsTab.IsSelected)
                LoadReservationsData();
        }
        
        private async void LoadShowingsData()
        {
            StartLoading();
            GetShowingsResponse response = await Service.GetShowingsAsync();
            Showing[] data = response.@return;
            ShowingsDataGrid.ItemsSource = data;
            StopLoading();
        }

        private async void LoadReservationsData()
        {
            StartLoading();
            try
            {
                GetReservationsResponse response = await Service.GetReservationsAsync(Authentication);
                Reservation[] reservations = response.@return;
                List<ReservationItem> items = new List<ReservationItem>();

                if (reservations != null && reservations.Length > 0)
                {
                    List<Showing> showings = (ShowingsDataGrid.ItemsSource as Showing[]).ToList();
                    foreach (Reservation reservation in reservations)
                    {
                        Showing foundShowing = showings.Find(showing => showing.ID == reservation.ShowingID);
                        items.Add(new ReservationItem(reservation, foundShowing));
                    }
                }
                ReservationsDataGrid.ItemsSource = items;
            }
            catch (FaultException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }

        private async void CancelReservation(Reservation reservation)
        {
            StartLoading();
            try
            {
                await Service.CancelReservationAsync(Authentication, reservation.ID);
                Utils.ShowInfo(this, "Rezerwacja została pomyślnie anulowana.");
                LoadReservationsData();
            }
            catch (FaultException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }
    }
}
