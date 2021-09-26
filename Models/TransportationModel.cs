using System;
using Microsoft.Data.SqlClient;

namespace xiaotasi.Models
{
    public class TransportationModel
    {
        public int transportationId { get; set; }
        public int transportationStep { get; set; }
        public string transportationCode { get; set; }
        public string remainSeatNum { get; set; }
        public static explicit operator TransportationModel(SqlDataReader v)
        {
            throw new NotImplementedException();
        }
    }
}
