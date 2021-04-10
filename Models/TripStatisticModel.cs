using System;


namespace xiaotasi.Models
{
    //旅遊統計資訊顯示物件
    public class TripStatisticModel
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string travelStep { get; set; }
        public int travelNum { get; set; }
        public int travelCost { get; set; }
        public int sellSeatNum { get; set; }
        public int remainSeatNum { get; set; }
        public string dest { get; set; }
        public int dayNum { get; set; }
        public int cost { get; set; }
        public int travelStepSelectFlag { get; set; }
    }
}
