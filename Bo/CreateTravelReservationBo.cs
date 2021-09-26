using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace xiaotasi.Bo
{
    //建立預定預設資訊
    public class CreateTravelReservationBo
    {
        public string token { get; set; }
        public string travelStepCode { get; set; }
        public List<MemberReservationArrBo> memberReservationArr { get; set; }
        public string note { get; set; }
        public static explicit operator CreateTravelReservationBo(SqlDataReader v)
        {
            throw new NotImplementedException();
        }
    }

    //成員訂位資訊
    public class MemberReservationArrBo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string birthday { get; set; }
        public int mealsType { get; set; }
        public int roomsType { get; set; }
        public int boardingId { get; set; }
        public int transportationId { get; set; }
        public int seatId { get; set; }
        public string note { get; set; }
        public static explicit operator MemberReservationArrBo(SqlDataReader v)
        {
            throw new NotImplementedException();
        }
    }
}
