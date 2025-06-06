using System;
using System.Collections.Generic;

namespace xiaotasi.Models
{
    public class TripViewModel
    {
        public int travelId { get; set; }
        public string travelStepCode { get; set; }
        public string travelCode { get; set; }
        public string travelTraditionalTitle { get; set; }
        public string travelEnTitle { get; set; }
        public string startDate { get; set; }
        public int cost { get; set; }
        public int travelType { get; set; }
        public string travelPicPath { get; set; }
        public string travelUrl { get; set; }
        public string travelFdate { get; set; }
        public string travelContent { get; set; }
        public string travelViewpointInfo { get; set; }
        public List<DateTripPicModel> dateTravelPicList { get; set; }
    }
}
