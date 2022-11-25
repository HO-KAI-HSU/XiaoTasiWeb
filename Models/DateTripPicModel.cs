using System.Collections.Generic;

namespace xiaotasi.Models
{
    //每日旅遊圖片介紹列表顯示物件
    public class DateTripPicModel
    {
        public string date { get; set; }
        public List<TripPicIntroModel> travelPicList { get; set; }
        public TripMealModel mealInfo { get; set; }
        public string hotel { get; set; }
    }
}
