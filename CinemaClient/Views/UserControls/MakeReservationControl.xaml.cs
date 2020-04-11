using CinemaClient.CinemaService;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace CinemaClient
{
    public partial class MakeReservationControl : UserControl
    {
        public ReservationWindow Context { get; set; }
        public CinemaServiceClient Service { get; set; }
        public Authentication Authentication { get; set; }
        public ReservationItem ReservationItem { get; set; }

        private List<CheckBox> selectedCheckBoxes;

        public MakeReservationControl()
        {
            InitializeComponent();
            selectedCheckBoxes = new List<CheckBox>();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSeatsData();
        }

        private void StartLoading()
        {
            Context.StartLoading();
            if (MakeReservationButton.IsEnabled)
                MakeReservationButton.IsEnabled = false;
        }

        private void StopLoading()
        {
            Context.StopLoading();
            if (selectedCheckBoxes.Count > 0)
                MakeReservationButton.IsEnabled = true;
        }

        private async void LoadSeatsData()
        {
            StartLoading();
            try
            {
                GetTakenSeatsResponse response = await Service.GetTakenSeatsAsync(ReservationItem.Showing.ID);
                Seat[] seats = response.@return;
                PopulateSeatGrid(seats);
            }
            catch (FaultException e)
            {
                Utils.ShowError(Context, e.Message);
            }
            StopLoading();
        }

        private GroupBox CreateSeatControl(int number)
        {
            GroupBox seatControl = new GroupBox();
            seatControl.Header = number.ToString();
            seatControl.Margin = new Thickness(5);
            seatControl.Padding = new Thickness(5);
            seatControl.HorizontalContentAlignment = HorizontalAlignment.Center;
            seatControl.VerticalContentAlignment = VerticalAlignment.Center;
            seatControl.Style = FindResource("GroupBoxStyle") as Style;

            CheckBox checkBox = new CheckBox();
            checkBox.Checked += CheckBox_Checked;
            checkBox.Unchecked += CheckBox_Unchecked;
            checkBox.Style = FindResource("CheckBoxStyle") as Style;
            seatControl.Content = checkBox;
            return seatControl;
        }

        private void PopulateSeatGrid(Seat[] seats)
        {
            int rows = ReservationItem.Showing.SeatsRowNumber;
            int columns = ReservationItem.Showing.SeatsColumnNumber;
            GroupBox[,] seatControls = new GroupBox[columns, rows];

            SeatsGrid.Visibility = Visibility.Collapsed;

            for (int i = 0; i < columns; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = GridLength.Auto;
                SeatsGrid.ColumnDefinitions.Add(columnDefinition);
            }

            for (int j = 0; j < rows; j++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                SeatsGrid.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    GroupBox seatControl = CreateSeatControl(i + 1);
                    seatControls[i, j] = seatControl;
                    SeatsGrid.Children.Add(seatControl);
                    Grid.SetRow(seatControl, j);
                    Grid.SetColumn(seatControl, i);
                }
            }

            foreach (Seat seat in seats)
                seatControls[seat.Column, seat.Row].IsEnabled = false;

            if (Context.Mode == ReservationWindow.ReservationMode.Edit)
                foreach (Seat seat in ReservationItem.Reservation.Seats)
                {
                    seatControls[seat.Column, seat.Row].IsEnabled = true;
                    CheckBox checkBox = seatControls[seat.Column, seat.Row].Content as CheckBox;
                    checkBox.IsChecked = true;
                }

            SeatsGrid.Visibility = Visibility.Visible;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            selectedCheckBoxes.Add(checkBox);
            if (!MakeReservationButton.IsEnabled)
                MakeReservationButton.IsEnabled = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            selectedCheckBoxes.Remove(checkBox);
            if (selectedCheckBoxes.Count == 0)
                MakeReservationButton.IsEnabled = false;
        }

        private void MakeReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.Ask(Context, "Czy na pewno chcesz złożyć rezerwację?"))
            {
                Reservation reservation = new Reservation();
                List<Seat> seats = new List<Seat>();
                foreach (CheckBox selectedCheckBox in selectedCheckBoxes)
                {
                    Seat seat = new Seat();
                    seat.Row = Grid.GetRow(selectedCheckBox.Parent as GroupBox);
                    seat.Column = Grid.GetColumn(selectedCheckBox.Parent as GroupBox);
                    seats.Add(seat);
                }
                reservation.ShowingID = ReservationItem.Showing.ID;
                reservation.Seats = seats.ToArray();
                if (Context.Mode == ReservationWindow.ReservationMode.Create)
                    MakeReservation(reservation);
                else if (Context.Mode == ReservationWindow.ReservationMode.Edit)
                    UpdateReservation(reservation);
            }
        }

        private async void MakeReservation(Reservation newReservation)
        {
            StartLoading();
            try
            {
                MakeReservationResponse response = await Service.MakeReservationAsync(Authentication, newReservation);
                string reservationCode = response.@return;
                newReservation.Code = reservationCode;
                ReservationItem.Reservation = newReservation;
                Utils.ShowInfo(Context, "Rezerwacja została złożona pomyślnie.");
                Context.ReservationItem = ReservationItem;
                Context.Mode = ReservationWindow.ReservationMode.Summary;
                Context.UpdateView();
            }
            catch (FaultException e)
            {
                Utils.ShowError(Context, e.Message);
                LoadSeatsData();
            }
            StopLoading();
        }

        private async void UpdateReservation(Reservation newReservation)
        {
            StartLoading();
            try
            {
                await Service.UpdateReservationAsync(Authentication, ReservationItem.Reservation.ID, newReservation);
                newReservation.ID = ReservationItem.Reservation.ID;
                newReservation.Code = ReservationItem.Reservation.Code;
                ReservationItem.Reservation = newReservation;
                Utils.ShowInfo(Context, "Rezerwacja została zaktualizowana pomyślnie.");
                Context.ReservationItem = ReservationItem;
                Context.Mode = ReservationWindow.ReservationMode.Summary;
                Context.UpdateView();
            }
            catch (FaultException e)
            {
                Utils.ShowError(Context, e.Message);
                LoadSeatsData();
            }
            StopLoading();
        }
    }
}
