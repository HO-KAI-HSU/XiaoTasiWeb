using System;

namespace xiaotasi.Models
{
    public class MemberReservationModel
    {
        public string travelReservationCode { get; set; }
        public string travelCode { get; set; }
        public int travelType { get; set; }
        public int travelStep { get; set; }
        public string travelTraditionalTitle { get; set; }
        public string travelEnTitle { get; set; }
        public string travelTraditionalContent { get; set; }
        public int dayNum { get; set; }
        public int cost { get; set; }
        public string startDate { get; set; }
        public string travelStepCode { get; set; }
        public int payStatus { get; set; }
        public string orderName { get; set; }
        public string memberCode { get; set; }
        public string travelStepDate { get; set; }
        public string travelReservationDate { get; set; }
    }

}
