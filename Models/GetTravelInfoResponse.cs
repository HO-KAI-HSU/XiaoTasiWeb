using System;
using System.Collections.Generic;
using System.Linq;

namespace xiaotasi.Models
{
    public class GetTravelInfoResponse
    {
        internal int count;
        public int success { get; set; }
        public List<TripStatisticListModel> travelStatisticList { get; set; }
        public Dictionary<string, object> travelInfo { get; set; }
        public List<DateTripPicModel> dateTravelPicList { get; set; }
        public TripCostModel costInfo { get; set; }
        public String[] nonIncludeCostList { get; set; }
        public String[] announcementsList { get; set; }
    }
}
