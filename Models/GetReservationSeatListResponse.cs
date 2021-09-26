using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace xiaotasi.Models
{
    public class GetReservationSeatListResponse
    {
        public int success { get; set; }
        public string travelCode { get; set; }
        public string travelStep { get; set; }
        public string startDate { get; set; }
        public List<ReservationSeatModel> reservationSeatList { get; set; }
        public List<TransportationModel> transportationList { get; set; }
        public int status { get; set; }
        public static explicit operator GetReservationSeatListResponse(SqlDataReader v)
        {
            throw new NotImplementedException();
        }
    }
}
