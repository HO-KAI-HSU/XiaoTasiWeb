using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;

namespace xiaotasi.Controllers
{

    public class TripController : Controller
    {
        private readonly ILogger<TripController> _logger;
        private readonly IConfiguration configuration;

        public TripController(ILogger<TripController> logger, IConfiguration config)
        {
            _logger = logger;
            configuration = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SingledayTrip()
        {
            return View();
        }


        [HttpPost]
        public IActionResult GetTravelListForMember(int page, int limit, int travelType)
        {
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            string travelSql = "WITH travelSql AS ( SELECT travel_id as travelId, travel_code as travelCode, travel_name as travelTraditionalTitle, travel_en_name as travelEnTitle, travel_cost as costs, travel_type as travelType, travel_pic_path as travelPicPath, travel_url as travelUrl, f_date as travelFdate, ROW_NUMBER() OVER(ORDER BY travel_id) AS rowId FROM travel_list WHERE travel_type = @travelType ) ";
            travelSql += " select * from travelSql";
            SqlCommand sqlCommand = new SqlCommand(travelSql, connection);
            sqlCommand.Parameters.AddWithValue("@travelType", travelType);
            connection.Open();
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
                travelShowData.travelPicPath = reader.IsDBNull(6) ? "" : "http://localhost:8080//~/Scripts/img/viewpoint/" + (string)reader[6];
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

        //取得旅遊各梯次開始時間
        private String _getTravelStepStartDateInfo(int travelId)
        {
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand select = new SqlCommand("select travel_s_time as startDate from travel_step_list where travel_id = @travelId", connection);
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
                    startDates += ",";

                }
            }
            connection.Close();
            return startDates;
        }



        public IActionResult MultipledayTrip()
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

        public IActionResult ForeignTrip()
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
