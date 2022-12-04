using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;
using xiaotasi.Service;

namespace xiaotasi.Controllers
{

    public class TripController : Controller
    {
        private readonly ILogger<TripController> _logger;
        private readonly IConfiguration _config;
        private readonly TripService _tripService;

        public TripController(ILogger<TripController> logger, IConfiguration config, TripService tripService)
        {
            _logger = logger;
            _config = config;
            _tripService = tripService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult SingledayTrip()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetTravelListForMember(int page, int limit, int travelType, string searchDate, string searchString)
        {
            var connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var travelSql = "WITH travelSql AS ( SELECT " +
                            "tl.travel_id as travelId, " +
                            "tl.travel_code as travelCode, " +
                            "tl.travel_name as travelTraditionalTitle, " +
                            "tl.travel_cost as costs, " +
                            "tl.travel_type as travelType, " +
                            "tl.travel_pic_path as travelPicPath, " +
                            "tl.travel_url as travelUrl, " +
                            "tl.f_date as travelFdate, ROW_NUMBER() OVER(ORDER BY tl.travel_id) AS rowId FROM (select travel_id from travel_step_list WHERE status = 1 and convert(DATETIME, travel_s_time, 23) >= @searchDate GROUP BY travel_id) as tsln inner join travel_list tl ON tl.travel_id = tsln.travel_id WHERE tl.travel_type = @travelType and tl.travel_name like @searchStr )";
            travelSql += " select * from travelSql";

            var travelShowDatas = new List<TripViewModel>();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand(travelSql, connection);
            sqlCommand.Parameters.AddWithValue("@travelType", travelType);
            sqlCommand.Parameters.AddWithValue("@searchStr", "%" + searchString + "%");
            sqlCommand.Parameters.AddWithValue("@searchDate", string.IsNullOrEmpty(searchDate) ? string.Empty : searchDate);
            await connection.OpenAsync();
            SqlDataReader reader = sqlCommand.ExecuteReader();

            PageControl<TripViewModel> pageControl = new PageControl<TripViewModel>();
            while (reader.Read())
            {
                var format = "yyyy-MM-dd";
                TripViewModel travelShowData = new TripViewModel();
                travelShowData.travelId = (int)reader[0];
                travelShowData.travelCode = (string)reader[1];
                travelShowData.travelTraditionalTitle = reader.IsDBNull(2) ? string.Empty : (string)reader[2];
                travelShowData.cost = (int)reader[3];
                travelShowData.travelType = (int)reader[4];
                travelShowData.startDate = _getTravelStepStartDateInfo((int)reader[0]);
                travelShowData.travelPicPath = reader[5].ToString();
                travelShowData.travelUrl = reader.IsDBNull(6) ? string.Empty : (string)reader[6];
                travelShowData.travelFdate = reader.IsDBNull(7) ? string.Empty : ((DateTime)reader[7]).ToString(format);
                travelShowDatas.Add(travelShowData);
            }
            connection.Close();
            List<TripViewModel> travelShowDatasNew = pageControl.pageControl(page, limit, travelShowDatas);
            GetTravelListResponse getTravelListResponse = new GetTravelListResponse();
            getTravelListResponse.success = 1;
            getTravelListResponse.count = pageControl.size;
            getTravelListResponse.page = page;
            getTravelListResponse.limit = limit;
            getTravelListResponse.travelList = travelShowDatasNew;

            return Json(getTravelListResponse);
        }

        [HttpPost]
        public async Task<ActionResult> GetTravelInfoForMember(string travelCode, string travelStepCode)
        {
            var travel = await _tripService.getTravelInfo(travelCode);
            var travelStatisticList = await _tripService.getTravelMonStatisticList(travel.travelId, travelStepCode);
            var dateTravelPicList = await _getDateTravelPicList(travel.travelId, 2);
            var travelCostInfo = await _getTravelCostInfo(travel.travelId);

            Dictionary<string, object> travelInfo = new Dictionary<string, object>();
            travelInfo.Add("travelCode", travel.travelCode);
            travelInfo.Add("travelTitle", travel.travelTitle);
            travelInfo.Add("travelSubject", travel.travelSubject);
            travelInfo.Add("travelContent", travel.travelContent);
            travelInfo.Add("travelMoviePath", string.Empty);

            GetTravelInfoResponse getTravelInfoResponse = new GetTravelInfoResponse();
            getTravelInfoResponse.success = 1;
            getTravelInfoResponse.travelInfo = travelInfo;
            getTravelInfoResponse.travelStatisticList = travelStatisticList;
            getTravelInfoResponse.dateTravelPicList = dateTravelPicList;
            getTravelInfoResponse.costInfo = travelCostInfo;
            getTravelInfoResponse.announcementsList = new string[0];
            getTravelInfoResponse.nonIncludeCostList = new string[0];
            return Json(getTravelInfoResponse);
        }

        [HttpPost]
        public async Task<ActionResult> GetTravelStatisticsInfoForMember(string travelCode, string travelStepCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string travelSql = "SELECT " +
                               "travel_id as travelId, " +
                               "travel_code as travelCode," +
                               "travel_name as travelTraditionalTitle, " +
                               "travel_cost as costs, travel_type as " +
                               "travelType, travel_pic_path as travelPicPath, " +
                               "travel_url as travelUrl, " +
                               "travel_subject as travelSubject, " +
                               "travel_content as travelContent FROM travel_list WHERE travel_code = @travelCode";
            SqlCommand select = new SqlCommand(travelSql, connection);
            //// 開啟資料庫連線
            select.Parameters.AddWithValue("@travelCode", travelCode);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<TripStatisticListModel> travelStatisticList = null;

            Dictionary<string, object> travelInfo = new Dictionary<string, object>();
            while (reader.Read())
            {
                travelInfo.Add("travelCode", (string)reader[1]);
                travelInfo.Add("travelTitle", reader.IsDBNull(2) ? string.Empty : (string)reader[2]);
                travelInfo.Add("travelSubject", reader.IsDBNull(6) ? string.Empty : (string)reader[6]);
                travelInfo.Add("travelContent", reader.IsDBNull(7) ? string.Empty : (string)reader[7]);
                travelInfo.Add("travelMoviePath", string.Empty);
                travelStatisticList = await _tripService.getTravelMonStatisticList((int)reader[0], travelStepCode);
            }
            connection.Close();
            GetTravelInfoResponse getTravelInfoResponse = new GetTravelInfoResponse();
            getTravelInfoResponse.success = 1;
            getTravelInfoResponse.travelInfo = travelInfo;
            getTravelInfoResponse.travelStatisticList = travelStatisticList;
            return Json(getTravelInfoResponse);
        }

        [HttpPost]
        public async Task<ActionResult> GetSingleTravelListForMember(int page, int limit, string searchDate, string searchString)
        {
            var connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            var sql = "select " +
                      "tl.travel_id as travelId, " +
                      "tl.travel_code as travelCode, " +
                      "tl.travel_name as travelTraditionalTitle, " +
                      "tl.travel_cost as costs, " +
                      "tl.travel_type as travelType, " +
                      "tl.travel_pic_path as travelPicPath, " +
                      "tl.travel_url as travelUrl, " +
                      "stl.travel_s_time as travelStime, " +
                      "tl.travel_subject as travelSubject, " +
                      "tl.travel_content as travelContent, " +
                      "stl.travel_step_code as travelStepCode, " +
                      "tl.travel_viewpoint_info as travelViewpointInfo from travel_step_list stl LEFT JOIN travel_list tl ON tl.travel_id = stl.travel_id WHERE stl.status = 1 and tl.travel_type = 1 and convert(DATETIME, stl.travel_s_time, 23) = @searchDate and tl.travel_name like @searchStr";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            sqlCommand.Parameters.AddWithValue("@searchStr", "%" + searchString + "%");
            sqlCommand.Parameters.AddWithValue("@searchDate", string.IsNullOrEmpty(searchDate) ? string.Empty : searchDate);
            await connection.OpenAsync();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            List<TripViewModel> travelShowDatas = new List<TripViewModel>();
            PageControl<TripViewModel> pageControl = new PageControl<TripViewModel>();
            while (reader.Read())
            {
                var format = "yyyy-MM-dd";
                TripViewModel travelShowData = new TripViewModel();
                travelShowData.travelId = (int)reader[0];
                travelShowData.travelCode = (string)reader[1];
                travelShowData.travelTraditionalTitle = reader.IsDBNull(2) ? string.Empty : (string)reader[2];
                travelShowData.cost = (int)reader[3];
                travelShowData.travelType = (int)reader[4];
                travelShowData.startDate = _getTravelStepStartDateInfo((int)reader[0]);
                travelShowData.dateTravelPicList = await this._getDateTravelPicList((int)reader[0], 1);
                travelShowData.travelPicPath = reader.IsDBNull(5) ? string.Empty : domainUrl + "/images/trip/" + (string)reader[5].ToString();
                travelShowData.travelUrl = reader.IsDBNull(6) ? string.Empty : (string)reader[6];
                travelShowData.travelFdate = reader.IsDBNull(7) ? string.Empty : ((DateTime)reader[7]).ToString(format);
                travelShowData.travelContent = reader.IsDBNull(9) ? string.Empty : (string)reader[9];
                travelShowData.travelStepCode = reader.IsDBNull(10) ? string.Empty : (string)reader[10];
                travelShowData.travelViewpointInfo = reader.IsDBNull(11) ? string.Empty : (string)reader[11];
                travelShowDatas.Add(travelShowData);
            }
            connection.Close();
            List<TripViewModel> travelShowDatasNew = pageControl.pageControl(page, limit, travelShowDatas);
            GetTravelListResponse getTravelListResponse = new GetTravelListResponse();
            getTravelListResponse.success = 1;
            getTravelListResponse.count = pageControl.size;
            getTravelListResponse.page = page;
            getTravelListResponse.limit = limit;
            getTravelListResponse.travelList = travelShowDatasNew;

            return Json(getTravelListResponse);
        }


        //取得旅遊各梯次開始時間
        private string _getTravelStepStartDateInfo(int travelId)
        {
            var connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var sql = "select travel_s_time as startDate from travel_step_list where travel_id = @travelId and status = 1";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand select = new SqlCommand(sql, connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<string> startDateList = new List<string>();
            while (reader.Read())
            {
                var format = "MM/dd";
                if (!reader.IsDBNull(0))
                {
                    startDateList.Add(((DateTime)reader[0]).ToString(format));
                }
            }

            var startDates = string.Join(",", startDateList);

            connection.Close();
            return startDates;
        }

        //每日旅遊介紹列表
        private async Task<List<DateTripPicModel>> _getDateTravelPicList(int travelId, int multiDayType)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select travel_detail_id as travelDetailId, day, travel_id as travelId from travel_detail_list where travel_id = @travelId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            TripHotelModel travelHotelInfo;
            List<DateTripPicModel> dateTravelPicLists = new List<DateTripPicModel>();
            while (reader.Read())
            {
                DateTripPicModel dateTravelPic = new DateTripPicModel();
                if (multiDayType == 2)
                {
                    travelHotelInfo = await _getTravelHotelInfo((int)reader[0]);
                }
                else
                {
                    travelHotelInfo = null;
                }
                TripMealModel travelMealInfo = await this._getTravelMealInfo((int)reader[0]);
                List<TripPicIntroModel> getTravelPicIntroList = await this._getTravelPicIntroList((int)reader[0]);
                dateTravelPic.travelPicList = getTravelPicIntroList;
                dateTravelPic.hotel = (travelHotelInfo == null || travelHotelInfo.hotel == null) ? string.Empty : travelHotelInfo.hotel;
                dateTravelPic.mealInfo = travelMealInfo;
                dateTravelPic.date = string.Empty;
                dateTravelPicLists.Add(dateTravelPic);
            }
            connection.Close();
            return dateTravelPicLists;
        }


        //取得旅遊花費資訊
        private async Task<TripCostModel> _getTravelCostInfo(int travelId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select transportation_info as transportationInfo, eat_info as eatInfo, live_info as liveInfo, action_info as actionInfo, insurance_info as insuranceInfo, near_info as nearInfo from travel_cost_list where travel_id = @travelId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            TripCostModel travelCostInfoTest = new TripCostModel();
            while (reader.Read())
            {
                travelCostInfoTest.travelDetailCode = "";
                travelCostInfoTest.transportationInfo = reader.IsDBNull(0) ? "" : (string)reader[0]; ;
                travelCostInfoTest.eatInfo = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelCostInfoTest.liveInfo = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelCostInfoTest.actionInfo = reader.IsDBNull(3) ? "" : (string)reader[3];
                travelCostInfoTest.insuranceInfo = reader.IsDBNull(4) ? "" : (string)reader[4];
                travelCostInfoTest.nearInfo = reader.IsDBNull(5) ? "" : (string)reader[5];
            }
            connection.Close();
            return travelCostInfoTest;
        }

        //取得旅遊住宿資訊
        private async Task<TripHotelModel> _getTravelHotelInfo(int travelDetailId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select hl.hotel_name as hotel, thl.travel_detail_id as travelDetailId from travel_hotel_list thl inner join hotel_list hl ON thl.hotel_id = hl.hotel_id where thl.travel_detail_id = @travelDetailId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            TripHotelModel travelHotelInfoTest = new TripHotelModel();
            while (reader.Read())
            {
                travelHotelInfoTest.travelDetailCode = "";
                travelHotelInfoTest.hotel = reader.IsDBNull(0) ? "" : (string)reader[0];
            }
            connection.Close();
            return travelHotelInfoTest;
        }

        //取得旅遊餐點資訊
        private async Task<TripMealModel> _getTravelMealInfo(int travelDetailId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select breakfast, lunch, dinner, travel_detail_id as travelDetailId from travel_meal_list where travel_detail_id = @travelDetailId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            TripMealModel travelMealInfoTest = new TripMealModel();
            while (reader.Read())
            {
                travelMealInfoTest.travelDetailCode = "";
                travelMealInfoTest.breakfast = reader.IsDBNull(0) ? "" : (string)reader[0];
                travelMealInfoTest.lunch = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelMealInfoTest.dinner = reader.IsDBNull(2) ? "" : (string)reader[2];
            }
            connection.Close();
            return travelMealInfoTest;
        }

        //取得旅遊圖片介紹資訊
        private async Task<List<TripPicIntroModel>> _getTravelPicIntroList(int travelDetailId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select vl.viewpoint_title as travelPicTraditionalTitle, vl.viewpoint_title as travelPicEnTitle, vl.viewpoint_content as travelPicTraditionalIntro, vl.viewpoint_content as travelPicEnIntro, vl.viewpoint_pic_path as travelPicPath, tpil.travel_detail_id as travelDetailId from travel_pic_intro_list tpil inner join viewpoint_list vl ON tpil.viewpoint_id = vl.viewpoint_id where tpil.travel_detail_id = @travelDetailId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<TripPicIntroModel> travelPicListTests = new List<TripPicIntroModel>();
            while (reader.Read())
            {
                TripPicIntroModel travelPicListTest = new TripPicIntroModel();
                travelPicListTest.travelPicTraditionalTitle = reader.IsDBNull(0) ? "" : (string)reader[0];
                travelPicListTest.travelPicEnTitle = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelPicListTest.travelPicTraditionalIntro = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelPicListTest.travelPicEnIntro = reader.IsDBNull(3) ? "" : (string)reader[3];
                travelPicListTest.travelPicPath = reader.IsDBNull(4) ? "" : (string)reader[4];
                travelPicListTest.travelDetailCode = "";
                travelPicListTests.Add(travelPicListTest);
            }
            connection.Close();
            return travelPicListTests;
        }

        public IActionResult MultipledayTrip()
        {
            return View();
        }

        [Route("Trip/MultipledayTripInfo/{traveCode}")]
        public IActionResult MultipledayTripInfo(string traveCode)
        {
            return View();
        }

        [Route("Trip/IslandTripInfo/{traveCode}")]
        public IActionResult IslandTripInfo(string traveCode)
        {
            return View();
        }

        public IActionResult IslandTrip()
        {
            return View();
        }

        public IActionResult CarTrip()
        {
            return View();
        }

        [Route("Trip/CarTripInfo/{traveCode}")]
        public IActionResult CarTripInfo(string traveCode)
        {
            return View();
        }

        public IActionResult ForeignTrip()
        {
            return View();
        }

        [Route("Trip/ForeignTripInfo/{traveCode}")]
        public IActionResult ForeignTripInfo(string traveCode)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
