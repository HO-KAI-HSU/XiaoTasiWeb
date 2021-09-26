using System;
using System.Collections.Generic;
using xiaotasi.Pojo;

namespace xiaotasi.Vo
{
    public class TravelReservationBoardingListVo
    {
        public int success { get; set; }

        public List<TripBoardingMatchPojo> travelReservationBoardingList { get; set; }
    }
}
