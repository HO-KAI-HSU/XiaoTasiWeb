using System;
using System.Collections.Generic;
using System.Linq;

namespace xiaotasi.Models
{
    public class GetTravelListResponse
    {
        public int success { get; set; }
        public int count { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public List<TripViewModel> travelList { get; set; }
    }
}
