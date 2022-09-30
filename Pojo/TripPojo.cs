using System;
namespace xiaotasi.Pojo
{
    public class TripPojo
    {
        public int travelId { get; set; }
        public string travelCode { get; set; }
        public string travelTitle { get; set; }
        public string travelSubject { get; set; }
        public string travelContent { get; set; }
        public int cost { get; set; }
        public int travelType { get; set; }
        public string startDate { get; set; }
        public string travelPicPath { get; set; }
        public string travelUrl { get; set; }
        public string travelFdate { get; set; }
        public string travelEdate { get; set; }
        public TripPojo()
        {
        }
    }
}
