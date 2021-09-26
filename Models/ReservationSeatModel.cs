using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace xiaotasi.Models
{
    public class ReservationSeatModel
    {
        public int transportationId { get; set; }
        public int transportationStep { get; set; }
        public int useStatus { get; set; }
        public List<TripReservationSeatMatchModel> reservationSeatList { get; set; }
        public static explicit operator ReservationSeatModel(SqlDataReader v)
        {
            throw new NotImplementedException();
        }
    }
}
