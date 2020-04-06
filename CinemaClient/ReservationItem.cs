using CinemaClient.CinemaService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient
{
    public class ReservationItem
    {
        public Reservation Reservation { get; set; }
        public Showing Showing { get; set; }
        public ReservationItem Self
        {
            get { return this; }
        }

        public ReservationItem(Reservation reservation, Showing showing)
        {
            Reservation = reservation;
            Showing = showing;
        }
    }
}
