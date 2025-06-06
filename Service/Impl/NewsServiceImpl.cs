﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Pojo;

namespace xiaotasi.Service.Impl
{
    public class NewsServiceImpl : NewsService
    {
        private readonly IConfiguration _config;

        public NewsServiceImpl(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<NewsPojo>> getLatestNewsList()
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            string timeNow = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds)).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select latest_news_id, latest_news_title, latest_news_content, latest_news_url, latest_news_pic_path, f_date from latest_news_list where publish_s_time <= @time and publish_e_time >= @time", connection);
            select.Parameters.Add("@time", System.Data.SqlDbType.DateTime).Value = timeNow;
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();
            List<NewsPojo> newsList = new List<NewsPojo>();
            while (await reader.ReadAsync())
            {
                NewsPojo newsData = new NewsPojo();
                newsData.newsId = reader.GetInt32(0);
                newsData.newsTraditionalTitle = reader.IsDBNull(1) ? "" : reader.GetSqlString(1).ToString();
                newsData.newsTraditionalContent = reader.IsDBNull(2) ? "" : reader.GetSqlString(2).ToString();
                newsData.newsPicPath = reader.IsDBNull(4) ? "" : (string)reader[4].ToString();
                string format = "yyyy-MM-dd";
                newsData.date = ((DateTime)reader[5]).ToString(format);
                newsData.newsUrl = "";
                newsList.Add(newsData);
            }
            connection.Close();
            return newsList;
        }

        public async Task<List<MediaNewsPojo>> getMediaNewsList()
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            string timeNow = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds)).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select latest_media_news_id, latest_media_news_title, latest_media_news_content, latest_media_news_url, latest_media_news_pic_path, f_date from latest_media_news_list where publish_s_time <= @time and publish_e_time >= @time and latest_media_news_type = 1", connection);
            select.Parameters.Add("@time", System.Data.SqlDbType.DateTime).Value = timeNow;
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();
            List<MediaNewsPojo> mediaNewsList = new List<MediaNewsPojo>();
            while (await reader.ReadAsync())
            {
                MediaNewsPojo newsData = new MediaNewsPojo();
                newsData.mediaNewsId = reader.GetInt32(0);
                newsData.mediaNewsTraditionalTitle = reader.IsDBNull(1) ? "" : (string)reader[1];
                newsData.mediaNewsTraditionalContent = reader.IsDBNull(2) ? "" : (string)reader[2];
                newsData.mediaNewsMovieUrl = reader.IsDBNull(3) ? "" : (string)reader[3];
                newsData.mediaNewsPicPath = reader.IsDBNull(4) ? "" : (string)reader[4];
                string format = "yyyy-MM-dd";
                newsData.date = ((DateTime)reader[5]).ToString(format);
                newsData.mediaNewsMovieUrl = "";
                mediaNewsList.Add(newsData);
            }
            connection.Close();
            return mediaNewsList;
        }
    }
}
