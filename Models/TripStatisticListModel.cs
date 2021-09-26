using System;
using System.Collections.Generic;

namespace xiaotasi.Models
{
    //旅遊統計資訊顯示物件
    public class TripStatisticListModel
    {
        public string travelMon { get; set; }
        public List<TripStatisticModel> travelStatisticList { get; set; }
    }
}
