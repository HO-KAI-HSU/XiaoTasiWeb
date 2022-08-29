using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        public TripController(ILogger<TripController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
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
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            SqlConnection connection = new SqlConnection(connectionString);
            string travelSql = "WITH travelSql AS ( SELECT tl.travel_id as travelId, tl.travel_code as travelCode, tl.travel_name as travelTraditionalTitle, tl.travel_en_name as travelEnTitle, tl.travel_cost as costs, tl.travel_type as travelType, tl.travel_pic_path as travelPicPath, tl.travel_url as travelUrl, tl.f_date as travelFdate, ROW_NUMBER() OVER(ORDER BY tl.travel_id) AS rowId FROM (select travel_id from travel_step_list WHERE convert(DATETIME, travel_s_time, 23) >= @searchDate GROUP BY travel_id) as tsln inner join travel_list tl ON tl.travel_id = tsln.travel_id WHERE tl.travel_type = @travelType and tl.travel_name like @searchStr )";
            //string travelSql = "WITH travelSql AS ( SELECT travel_id as travelId, travel_code as travelCode, travel_name as travelTraditionalTitle, travel_en_name as travelEnTitle, travel_cost as costs, travel_type as travelType, travel_pic_path as travelPicPath, travel_url as travelUrl, f_date as travelFdate, ROW_NUMBER() OVER(ORDER BY travel_id) AS rowId FROM travel_list WHERE travel_type = @travelType and convert(DATETIME, travel_s_time, 23) = @searchDate and tl.travel_name like @searchStr )";
            travelSql += " select * from travelSql";
            SqlCommand sqlCommand = new SqlCommand(travelSql, connection);
            sqlCommand.Parameters.AddWithValue("@travelType", travelType);
            sqlCommand.Parameters.AddWithValue("@searchStr", "%" + searchString + "%");
            sqlCommand.Parameters.AddWithValue("@searchDate", searchDate == null ? "" : searchDate);
            await connection.OpenAsync();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            List<TripViewModel> travelShowDatas = new List<TripViewModel>();
            PageControl<TripViewModel> pageControl = new PageControl<TripViewModel>();
            while (reader.Read())
            {
                TripViewModel travelShowData = new TripViewModel();
                travelShowData.travelId = (int)reader[0];
                travelShowData.travelCode = (string)reader[1];
                travelShowData.travelTraditionalTitle = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelShowData.travelEnTitle = reader.IsDBNull(3) ? "" : (string)reader[3];
                travelShowData.cost = (int)reader[4];
                travelShowData.travelType = (int)reader[5];
                travelShowData.startDate = this._getTravelStepStartDateInfo((int)reader[0]) == null ? "" : this._getTravelStepStartDateInfo((int)reader[0]);
                travelShowData.travelPicPath = (string)reader[6].ToString();
                travelShowData.travelUrl = reader.IsDBNull(7) ? "" : (string)reader[7];
                string format = "yyyy-MM-dd";
                travelShowData.travelFdate = reader.IsDBNull(8) ? "" : ((DateTime)reader[8]).ToString(format);
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
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string travelSql = "SELECT travel_id as travelId, travel_code as travelCode, travel_name as travelTraditionalTitle, travel_en_name as travelEnTitle, travel_cost as costs, travel_type as travelType, travel_pic_path as travelPicPath, travel_url as travelUrl, travel_subject as travelSubject, travel_content as travelContent FROM travel_list WHERE travel_code = @travelCode";
            SqlCommand select = new SqlCommand(travelSql, connection);
            //// 開啟資料庫連線
            select.Parameters.AddWithValue("@travelCode", travelCode);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<TripViewModel> travelShowDatas = new List<TripViewModel>();
            List<TripStatisticListModel> travelStatisticList = null;
            List<DateTripPicModel> dateTravelPicList = null;
            TripInfoModel travelInfoData = null;
            TripCostModel travelCostInfo = null;
            Dictionary<string, object> travelInfo = new Dictionary<string, object>();
            while (reader.Read())
            {
                travelInfoData = new TripInfoModel();
                travelInfo.Add("travelCode", (string)reader[1]);
                travelInfo.Add("travelTitle", reader.IsDBNull(2) ? "" : (string)reader[2]);
                travelInfo.Add("travelSubject", reader.IsDBNull(8) ? "" : (string)reader[8]);
                travelInfo.Add("travelContent", reader.IsDBNull(9) ? "" : (string)reader[9]);
                travelInfo.Add("travelMoviePath", "");
                travelStatisticList = await this._getTravelStatisticListTest((int)reader[0], travelStepCode);
                dateTravelPicList = await this._getDateTravelPicList((int)reader[0], 2);
                travelCostInfo = await this._getTravelCostInfo((int)reader[0]);
            }
            connection.Close();
            GetTravelInfoResponse getTravelInfoResponse = new GetTravelInfoResponse();
            getTravelInfoResponse.success = 1;
            getTravelInfoResponse.travelStatisticList = travelStatisticList;
            getTravelInfoResponse.travelInfo = travelInfo;
            getTravelInfoResponse.dateTravelPicList = dateTravelPicList;
            getTravelInfoResponse.costInfo = travelCostInfo;
            getTravelInfoResponse.announcementsList = new String[0];
            getTravelInfoResponse.nonIncludeCostList = new String[0];
            return Json(getTravelInfoResponse);
        }

        [HttpPost]
        public async Task<ActionResult> GetTravelStatisticsInfoForMember(string travelCode, string travelStepCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string travelSql = "SELECT travel_id as travelId, travel_code as travelCode, travel_name as travelTraditionalTitle, travel_en_name as travelEnTitle, travel_cost as costs, travel_type as travelType, travel_pic_path as travelPicPath, travel_url as travelUrl, travel_subject as travelSubject, travel_content as travelContent FROM travel_list WHERE travel_code = @travelCode";
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
                travelInfo.Add("travelTitle", reader.IsDBNull(2) ? "" : (string)reader[2]);
                travelInfo.Add("travelSubject", reader.IsDBNull(8) ? "" : (string)reader[8]);
                travelInfo.Add("travelContent", reader.IsDBNull(9) ? "" : (string)reader[9]);
                travelInfo.Add("travelMoviePath", "");
                travelStatisticList = await this._getTravelStatisticListTest((int)reader[0], travelStepCode);
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
            CultureInfo provider = CultureInfo.InvariantCulture;
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string travelSql = "select tl.travel_id as travelId, tl.travel_code as travelCode, tl.travel_name as travelTraditionalTitle, tl.travel_en_name as travelEnTitle, tl.travel_cost as costs, tl.travel_type as travelType, tl.travel_pic_path as travelPicPath, tl.travel_url as travelUrl, stl.travel_s_time as travelStime, tl.travel_subject as travelSubject, tl.travel_content as travelContent, stl.travel_step_code as travelStepCode, tl.travel_viewpoint_info as travelViewpointInfo from travel_step_list stl LEFT JOIN travel_list tl ON tl.travel_id = stl.travel_id WHERE tl.travel_type = 1 and convert(DATETIME, stl.travel_s_time, 23) = @searchDate and tl.travel_name like @searchStr";
            SqlCommand sqlCommand = new SqlCommand(travelSql, connection);
            sqlCommand.Parameters.AddWithValue("@searchStr", "%" + searchString + "%");
            sqlCommand.Parameters.AddWithValue("@searchDate", searchDate == null ? "" : searchDate);
            await connection.OpenAsync();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            List<TripViewModel> travelShowDatas = new List<TripViewModel>();
            PageControl<TripViewModel> pageControl = new PageControl<TripViewModel>();
            while (reader.Read())
            {
                TripViewModel travelShowData = new TripViewModel();
                travelShowData.travelId = (int)reader[0];
                travelShowData.travelCode = (string)reader[1];
                travelShowData.travelTraditionalTitle = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelShowData.travelEnTitle = reader.IsDBNull(3) ? "" : (string)reader[3];
                travelShowData.cost = (int)reader[4];
                travelShowData.travelType = (int)reader[5];
                travelShowData.startDate = this._getTravelStepStartDateInfo((int)reader[0]) == null ? "" : this._getTravelStepStartDateInfo((int)reader[0]);
                travelShowData.dateTravelPicList = await this._getDateTravelPicList((int)reader[0], 1);
                travelShowData.travelPicPath = reader.IsDBNull(6) ? "" : domainUrl + "/images/trip/" + (string)reader[6].ToString();
                travelShowData.travelUrl = reader.IsDBNull(7) ? "" : (string)reader[7];
                string format = "yyyy-MM-dd";
                travelShowData.travelFdate = reader.IsDBNull(8) ? "" : ((DateTime)reader[8]).ToString(format);
                travelShowData.travelContent = reader.IsDBNull(10) ? "" : (string)reader[10];
                travelShowData.travelStepCode = reader.IsDBNull(11) ? "" : (string)reader[11];
                travelShowData.travelViewpointInfo = reader.IsDBNull(12) ? "" : (string)reader[12];
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
        private String _getTravelStepStartDateInfo(int travelId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand select = new SqlCommand("select travel_s_time as startDate from travel_step_list where travel_id = @travelId and status = 1", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            string startDates = null;
            while (reader.Read())
            {
                string format = "MM/dd";
                if (!reader.IsDBNull(0))
                {
                    startDates += ((DateTime)reader[0]).ToString(format);
                    startDates += "、";

                }
            }
            connection.Close();
            return startDates;
        }

        //取得旅遊月份資訊
        private async Task<List<String>> _getTravelStatisticMonthList(int travelId, string travelStepCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select travel_s_time from travel_step_list where travel_id = @travelId and convert(DATETIME, travel_s_time, 23) > @searchDate and status = 1", connection);
            // 開啟資料庫連線
            string format = "yyyy-MM-dd";
            string monFormat = "yyyy-MM";
            string filterDate = DateTime.Now.ToString(format);
            //string filterDate = "2020-11-11";
            select.Parameters.AddWithValue("@searchDate", filterDate);
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<String> travelMonArr = new List<String>();
            while (reader.Read())
            {
                DateTime date = ((DateTime)reader[0]);
                String mon = date.ToString(monFormat);
                if (!travelMonArr.Exists(x => x == mon))
                {
                    travelMonArr.Add(mon);
                }
            }
            connection.Close();
            return travelMonArr;
        }


        //取得旅遊各梯次資訊
        private async Task<List<TripStatisticModel>> _getTravelStatisticList(int travelId, string travelStepCode)
        {
            List<String> travelMonArr = await this._getTravelStatisticMonthList(travelId, travelStepCode);
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select travel_s_time as startDate, travel_e_time as endDate, travel_step_code as travelStep, travel_num as travelNum, travel_cost as travelCost, sell_seat_num as sellSeatNum, remain_seat_num as remainSeatNum from travel_step_list where travel_id = @travelId and status = 1", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<TripStatisticModel> travelStatisticLists = new List<TripStatisticModel>();
            Boolean minTotalDayFlag = false;
            Boolean travelStepCodeFlag = false;
            if (travelStepCode != null)
            {
                travelStepCodeFlag = true;
            }
            while (reader.Read())
            {
                TripStatisticModel travelStepInfo = new TripStatisticModel();
                string format = "yyyy-MM-dd";
                int nearTravelFlag = 0;
                DateTime startDate = ((DateTime)reader[0]);
                DateTime endDate = ((DateTime)reader[1]);
                TimeSpan t = startDate - DateTime.Now;
                double nrOfDays = t.TotalDays;
                if (travelStepCodeFlag && (string)reader[2] == travelStepCode)
                {
                    nearTravelFlag = 1;
                }
                else if (!travelStepCodeFlag)
                {
                    if (nrOfDays > 0 && !minTotalDayFlag)
                    {
                        nearTravelFlag = 1;
                        minTotalDayFlag = true;
                    }
                }

                travelStepInfo.startDate = startDate.ToString(format);
                travelStepInfo.endDate = endDate.ToString(format);
                travelStepInfo.travelStep = (string)reader[2];
                travelStepInfo.travelNum = (int)reader[3];
                travelStepInfo.travelCost = (int)reader[4];
                travelStepInfo.sellSeatNum = (int)reader[5];
                travelStepInfo.remainSeatNum = (int)reader[6];
                travelStepInfo.dest = "";
                travelStepInfo.dayNum = (int)endDate.Subtract(startDate).TotalDays + 1;
                travelStepInfo.travelStepSelectFlag = nearTravelFlag;
                travelStatisticLists.Add(travelStepInfo);
            }
            connection.Close();
            return travelStatisticLists;
        }

        //取得旅遊各梯次資訊
        private async Task<List<TripStatisticListModel>> _getTravelStatisticListTest(int travelId, string travelStepCode)
        {
            // 取得旅遊月份資訊
            List<String> travelMonArr = await this._getTravelStatisticMonthList(travelId, travelStepCode);

            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select travel_s_time as startDate, travel_e_time as endDate, travel_step_code as travelStep, travel_num as travelNum, travel_cost as travelCost, sell_seat_num as sellSeatNum, remain_seat_num as remainSeatNum from travel_step_list where travel_id = @travelId and status = 1", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();

            List<TripStatisticListModel> travelMonStatisticList = new List<TripStatisticListModel>();
            foreach (String mon in travelMonArr)
            {
                TripStatisticListModel tripStatisticList = new TripStatisticListModel();
                tripStatisticList.travelMon = mon;
                tripStatisticList.travelStatisticList = new List<TripStatisticModel>();
                travelMonStatisticList.Add(tripStatisticList);
            }

            Boolean minTotalDayFlag = false;
            Boolean travelStepCodeFlag = false;
            if (travelStepCode != null)
            {
                travelStepCodeFlag = true;
            }
            while (reader.Read())
            {
                TripStatisticModel travelStepInfo = new TripStatisticModel();
                string format = "yyyy-MM-dd";
                string monFormat = "yyyy-MM";
                int nearTravelFlag = 0;
                DateTime startDate = ((DateTime)reader[0]);
                DateTime endDate = ((DateTime)reader[1]);
                TimeSpan t = startDate - DateTime.Now;

                // 取得行程月份索引鍵
                int monIndex = travelMonArr.IndexOf(startDate.ToString(monFormat));
                if (monIndex > -1)
                {
                    double nrOfDays = t.TotalDays;
                    if (travelStepCodeFlag && (string)reader[2] == travelStepCode)
                    {
                        nearTravelFlag = 1;
                    }
                    else if (!travelStepCodeFlag)
                    {
                        if (nrOfDays > 0 && !minTotalDayFlag)
                        {
                            nearTravelFlag = 1;
                            minTotalDayFlag = true;
                        }
                    }

                    travelStepInfo.startDate = startDate.ToString(format);
                    travelStepInfo.endDate = endDate.ToString(format);
                    travelStepInfo.travelStep = (string)reader[2];
                    travelStepInfo.travelNum = (int)reader[3];
                    travelStepInfo.travelCost = (int)reader[4];
                    travelStepInfo.sellSeatNum = (int)reader[5];
                    travelStepInfo.remainSeatNum = (int)reader[6];
                    travelStepInfo.dest = "";
                    travelStepInfo.dayNum = (int)endDate.Subtract(startDate).TotalDays + 1;
                    travelStepInfo.travelStepSelectFlag = nearTravelFlag;
                    travelMonStatisticList[monIndex].travelStatisticList.Add(travelStepInfo);
                }

            }
            connection.Close();
            return travelMonStatisticList;
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
                    travelHotelInfo = await this._getTravelHotelInfo((int)reader[0]);
                }
                else
                {
                    travelHotelInfo = null;
                }
                TripMealModel travelMealInfo = await this._getTravelMealInfo((int)reader[0]);
                List<TripPicIntroModel> getTravelPicIntroList = await this._getTravelPicIntroList((int)reader[0]);
                dateTravelPic.travelPicList = getTravelPicIntroList;
                dateTravelPic.hotel = (travelHotelInfo == null || travelHotelInfo.hotel == null) ? "" : travelHotelInfo.hotel;
                dateTravelPic.mealInfo = travelMealInfo;
                dateTravelPic.date = "";
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
            SqlCommand select = new SqlCommand("select transportation_info as transportationInfo, eat_info as eatInfo, live_info as liveInfo, action_info as actionInfo, insurance_info as insuranceInfo, near_info as nearInfo, travel_detail_id as travelDetailId from travel_cost_list where travel_id = @travelId", connection);
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

        public IActionResult MultipledayTripInfo()
        {
            return View();
        }

        public IActionResult IslandTripInfo()
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

        public IActionResult CarTripInfo()
        {
            return View();
        }

        public IActionResult ForeignTrip()
        {
            return View();
        }

        public IActionResult ForeignTripInfo()
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
