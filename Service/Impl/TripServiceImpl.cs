using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Models;
using xiaotasi.Pojo;

namespace xiaotasi.Service.Impl
{
    public class TripServiceImpl : TripService
    {
        private readonly IConfiguration _config;

        public TripServiceImpl(IConfiguration config)
        {
            _config = config;
        }

        // 取得旅遊資訊
        public async Task<List<TripPojo>> getTravelList(string date = "")
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);

            // SQL Command
            string travelSql = "SELECT " +
                               "travel_id as travelId, " +
                               "travel_code as travelCode, " +
                               "travel_name as travelTraditionalTitle, " +
                               "travel_cost as costs, " +
                               "travel_type as travelType, " +
                               "travel_pic_path as travelPicPath, " +
                               "travel_url as travelUrl, " +
                               "travel_subject as travelSubject, " +
                               "travel_content as travelContent FROM travel_list Where convert(DATETIME, travel_s_time, 23) >= @searchDate";
            SqlCommand select = new SqlCommand(travelSql, connection);
            select.Parameters.AddWithValue("@searchDate", !date.Equals("") ? date : DateTime.Now.Date.ToString());

            //// 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();
            List<TripPojo> tripPojos = new List<TripPojo>();

            while (await reader.ReadAsync())
            {
                TripPojo tripPojo = new TripPojo
                {
                    travelId = (int)reader[0],
                    travelCode = (string)reader[1],
                    travelTitle = reader.IsDBNull(2) ? "" : (string)reader[2],
                    cost = (int)reader[3],
                    travelType = (int)reader[4],
                    travelPicPath = reader.IsDBNull(5) ? "" : (string)reader[5],
                    travelUrl = reader.IsDBNull(6) ? "" : (string)reader[6],
                    travelSubject = reader.IsDBNull(7) ? "" : (string)reader[7],
                    travelContent = reader.IsDBNull(8) ? "" : (string)reader[8]
                };
                tripPojos.Add(tripPojo);
            }
            connection.Close();
            return tripPojos;
        }

        public async Task<TripPojo> getTravelInfo(string travelCode)
        {
            var connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var travelSql = "SELECT " +
                            "travel_id as travelId, " +
                            "travel_code as travelCode, " +
                            "travel_name as travelTraditionalTitle, " +
                            "travel_cost as costs, " +
                            "travel_type as travelType, " +
                            "travel_pic_path as travelPicPath, " +
                            "travel_url as travelUrl, " +
                            "travel_subject as travelSubject, " +
                            "travel_content as travelContent FROM travel_list WHERE travel_code = @travelCode";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand select = new SqlCommand(travelSql, connection);
            select.Parameters.AddWithValue("@travelCode", travelCode);

            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();
            TripPojo tripPojo = new TripPojo();
            while (await reader.ReadAsync())
            {
                tripPojo.travelId = (int)reader[0];
                tripPojo.travelCode = (string)reader[1];
                tripPojo.travelTitle = reader.IsDBNull(2) ? "" : (string)reader[2];
                tripPojo.cost = (int)reader[3];
                tripPojo.travelType = (int)reader[4];
                tripPojo.travelPicPath = reader.IsDBNull(5) ? "" : (string)reader[5];
                tripPojo.travelUrl = reader.IsDBNull(6) ? "" : (string)reader[6];
                tripPojo.travelSubject = reader.IsDBNull(7) ? "" : (string)reader[7];
                tripPojo.travelContent = reader.IsDBNull(8) ? "" : (string)reader[8];
            }
            connection.Close();

            return tripPojo;
        }

        // 取得旅遊資訊
        public async Task<List<TripPojo>> getTravelStepList()
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);

            // SQL Command
            string travelSql = "select travel_s_time as startDate, travel_e_time as endDate, travel_step_code as travelStep, travel_num as travelNum, travel_cost as travelCost, sell_seat_num as sellSeatNum, remain_seat_num as remainSeatNum from travel_step_list where travel_id = @travelId and status = 1";
            SqlCommand select = new SqlCommand(travelSql, connection);

            //// 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();
            List<TripPojo> tripPojos = new List<TripPojo>();

            while (await reader.ReadAsync())
            {
                TripPojo tripPojo = new TripPojo
                {
                    travelId = (int)reader[0],
                    travelCode = (string)reader[1],
                    travelTitle = reader.IsDBNull(2) ? "" : (string)reader[2],
                    cost = (int)reader[3],
                    travelType = (int)reader[4],
                    travelPicPath = reader.IsDBNull(5) ? "" : (string)reader[5],
                    travelUrl = reader.IsDBNull(6) ? "" : (string)reader[6],
                    travelSubject = reader.IsDBNull(7) ? "" : (string)reader[7],
                    travelContent = reader.IsDBNull(8) ? "" : (string)reader[8]
                };
                tripPojos.Add(tripPojo);
            }
            connection.Close();
            return tripPojos;
        }

        //取得旅遊各個月梯次統列表資訊
        public async Task<List<TripStatisticListModel>> getTravelMonStatisticList(int travelId, string travelStepCode)
        {
            var travelStatistics = await _getTravelStatisticList(travelId);
            var travelStatisticSellSeats = await _getTravelStatisticSeatList();

            bool minTotalDayFlag = false;
            bool travelStepCodeFlag = false;
            if (!string.IsNullOrEmpty(travelStepCode))
            {
                travelStepCodeFlag = true;
            }

            var tripMonStatisticList = travelStatistics
                .GroupBy(group => group.startMonth)
                .OrderBy(order => order.Key)
                .Select(tss => new TripStatisticListModel
                {
                    travelMon = tss.Key,
                    travelStatisticList = tss
                        .OrderBy(order => order.startDate)
                        .Select(sel => sel)
                        .ToList()
                }).ToList();

            foreach (var tripMonStatistic in tripMonStatisticList)
            {
                var tripStatistics = tripMonStatistic.travelStatisticList;
                foreach (var tripStatistic in tripStatistics)
                {
                    DateTime startDate = DateTime.Parse(tripStatistic.startDate + " 00:00:00");
                    TimeSpan t = startDate - DateTime.Now.AddHours(-8);
                    var nrOfDays = t.TotalDays;
                    var nearTravelFlag = 0;

                    if (travelStepCodeFlag && tripStatistic.travelStepCode.Equals(travelStepCode))
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
                    tripStatistic.travelStepSelectFlag = nearTravelFlag;
                    tripStatistic.remainSeatNum = tripStatistic.travelNum - travelStatisticSellSeats
                        .Where(where => where.travelStepId == tripStatistic.travelStepId)
                        .Select(sel => sel.sellSeatNum)
                        .FirstOrDefault();
                }
            }
            return tripMonStatisticList;
        }

        //取得旅遊各梯次資訊
        private async Task<IEnumerable<TripStatisticModel>> _getTravelStatisticList(int travelId)
        {
            var connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var sql = "select travel_step_id as travelStepId, travel_step_code as travelStepCode, travel_s_time as startDate, travel_e_time as endDate, travel_num as travelNum, travel_cost as travelCost, sell_seat_num as sellSeatNum, remain_seat_num as remainSeatNum from travel_step_list where travel_id = @travelId and convert(DATETIME, travel_s_time, 23) >= @nowDate and status = 1";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand select = new SqlCommand(sql, connection);

            var format = "yyyy-MM-dd";
            var filterDate = DateTime.Now.AddHours(-8).ToString(format);
            select.Parameters.AddWithValue("@nowDate", filterDate);
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();

            var tripStatisticList = new List<TripStatisticModel>();
            while (await reader.ReadAsync())
            {
                var travelStepInfo = new TripStatisticModel();
                var monFormat = "yyyy-MM";
                var nearTravelFlag = 0;
                travelStepInfo.travelStepId = (int)reader[0];
                travelStepInfo.travelStepCode = (string)reader[1];
                DateTime startDate = (DateTime)reader[2];
                DateTime endDate = (DateTime)reader[3];
                travelStepInfo.startDate = startDate.ToString(format);
                travelStepInfo.endDate = endDate.ToString(format);
                travelStepInfo.startMonth = startDate.ToString(monFormat);
                travelStepInfo.travelNum = (int)reader[4];
                travelStepInfo.travelCost = (int)reader[5];
                travelStepInfo.sellSeatNum = (int)reader[6];
                travelStepInfo.remainSeatNum = (int)reader[7];
                travelStepInfo.dest = string.Empty;
                travelStepInfo.dayNum = (int)endDate.Subtract(startDate).TotalDays + 1;
                travelStepInfo.travelStepSelectFlag = nearTravelFlag;
                tripStatisticList.Add(travelStepInfo);
            }
            connection.Close();
            return tripStatisticList;
        }

        //取得旅遊各梯次已賣出座位數量資訊
        private async Task<IEnumerable<TripStatisticSeatModel>> _getTravelStatisticSeatList()
        {
            var connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var sql = "select travel_step_id, Count(*) from seat_travel_match_list where status > -1 group by travel_step_id";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand select = new SqlCommand(sql, connection);

            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();

            var statisticSeats = new List<TripStatisticSeatModel>();
            while (await reader.ReadAsync())
            {
                var statisticSeat = new TripStatisticSeatModel
                {
                    travelStepId = (int)reader[0],
                    sellSeatNum = (int)reader[1]
                };
                statisticSeats.Add(statisticSeat);
            }
            connection.Close();
            return statisticSeats;
        }
    }
}
