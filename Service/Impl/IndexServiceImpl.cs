using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Pojo;

namespace xiaotasi.Service.Impl
{
    public class IndexServiceImpl : IndexService
    {
        private readonly IConfiguration _config;

        public IndexServiceImpl(IConfiguration config)
        {
            _config = config;
        }

        public List<IndexBannerPojo> getIndexBannerList()
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            string timeNow = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds)).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select index_banner_id, index_banner_url, index_banner_pic_path from index_banner_list where publish_s_time <= @time and publish_e_time >= @time", connection);
            select.Parameters.Add("@time", System.Data.SqlDbType.DateTime).Value = timeNow;
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<IndexBannerPojo> list = new List<IndexBannerPojo>();
            while (reader.Read())
            {
                IndexBannerPojo data = new IndexBannerPojo();
                data.indexBannerId = reader.GetInt32(0);
                data.indexBannerUrl = reader.IsDBNull(1) ? "" : reader.GetSqlString(1).ToString();
                //data.indexBannerPicPath = reader.IsDBNull(2) ? "" : domainUrl + "/images/index/" + reader.GetSqlString(2).ToString();
                data.indexBannerPicPath = reader.IsDBNull(2) ? "" : reader.GetSqlString(2).ToString();
                list.Add(data);
            }
            connection.Close();
            return list;
        }
    }
}
