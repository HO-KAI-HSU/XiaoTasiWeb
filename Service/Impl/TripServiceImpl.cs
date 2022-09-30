using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Pojo;

namespace xiaotasi.Service.Impl
{
    public class TripServiceImpl
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
            string travelSql = "SELECT travel_id as travelId, travel_code as travelCode, travel_name as travelTraditionalTitle, travel_cost as costs, travel_type as travelType, travel_pic_path as travelPicPath, travel_url as travelUrl, travel_subject as travelSubject, travel_content as travelContent FROM travel_list Where convert(DATETIME, travel_s_time, 23) >= @searchDate";
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

        // 取得旅遊資訊
        public async Task<List<TripPojo>> getTravelStepList()
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);

            // SQL Command
            string travelSql = "SELECT travel_id as travelId, travel_code as travelCode, travel_name as travelTraditionalTitle, travel_cost as costs, travel_type as travelType, travel_pic_path as travelPicPath, travel_url as travelUrl, travel_subject as travelSubject, travel_content as travelContent FROM travel_list";
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
    }
}
