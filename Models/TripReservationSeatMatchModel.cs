using System;
using Microsoft.Data.SqlClient;

namespace xiaotasi.Models
{
    public class TripReservationSeatMatchModel
    {
        public int seatId { get; set; }
        public string seatName { get; set; }
        public int seatStatus { get; set; }
        public static explicit operator TripReservationSeatMatchModel(SqlDataReader v)
        {
            throw new NotImplementedException();
        }
    }
}
