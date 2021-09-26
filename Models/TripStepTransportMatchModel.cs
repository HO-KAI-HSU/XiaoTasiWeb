using System;
using Microsoft.Data.SqlClient;

namespace xiaotasi.Models
{
    public class TripStepTransportMatchModel
    {
        public int travelStepId { get; set; }
        public string travelStepCode { get; set; }
        public int travelCost { get; set; }
        public int travelNum { get; set; }
        public string travelStime { get; set; }
        public string travelEtime { get; set; }
        public int travelId { get; set; }
        public int sellSeatNum { get; set; }
        public int remainSeatNum { get; set; }
        public string transportationIds { get; set; }
        public static explicit operator TripStepTransportMatchModel(SqlDataReader v)
        {
            throw new NotImplementedException();
        }
    }
}
