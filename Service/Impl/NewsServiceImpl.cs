using System;
using System.Collections.Generic;
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

        public List<NewsPojo> getLatestNewsList()
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select latest_news_id, latest_news_title, latest_news_en_title, latest_news_content, latest_news_en_content, latest_news_url, latest_news_pic_path, f_date from latest_news_list", connection);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<NewsPojo> newsList = new List<NewsPojo>();
            while (reader.Read())
            {
                NewsPojo newsData = new NewsPojo();
                newsData.newsId = reader.GetInt32(0);
                newsData.newsTraditionalTitle = reader.IsDBNull(1) ? "" : reader.GetSqlString(1).ToString();
                newsData.newsTraditionalContent = reader.IsDBNull(3) ? "" : reader.GetSqlString(3).ToString();
                if (reader.IsDBNull(2))
                {
                    newsData.newsEnTitle = "";
                }
                else
                {
                    newsData.newsEnTitle = (string)reader[2];
                }
                if (reader.IsDBNull(4))
                {
                    newsData.newsEnContent = "";
                }
                else
                {
                    newsData.newsEnContent = (string)reader[4];
                }
                if (reader.IsDBNull(6))
                {
                    newsData.newsPicPath = "";
                }
                else
                {
                    newsData.newsPicPath = "~/Scripts/img/latestNews/" + (string)reader[6];
                }
                string format = "yyyy-MM-dd";
                newsData.date = ((DateTime)reader[7]).ToString(format);
                newsData.newsUrl = "";
                newsList.Add(newsData);
            }
            connection.Close();
            return newsList;
        }

        public List<MediaNewsPojo> getMediaNewsList()
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select latest_media_news_id, latest_media_news_title, latest_media_news_en_title, latest_media_news_content, latest_media_news_en_content, latest_media_news_url, latest_media_news_pic_path, f_date from latest_media_news_list", connection);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<MediaNewsPojo> mediaNewsList = new List<MediaNewsPojo>();
            while (reader.Read())
            {
                MediaNewsPojo newsData = new MediaNewsPojo();
                newsData.mediaNewsId = reader.GetInt32(0);
                newsData.mediaNewsTraditionalTitle = reader.IsDBNull(1) ? "" : (string)reader[1];
                newsData.mediaNewsTraditionalContent = reader.IsDBNull(3) ? "" : (string)reader[3];
                if (reader.IsDBNull(2))
                {
                    newsData.mediaNewsEnTitle = "";
                }
                else
                {
                    newsData.mediaNewsEnTitle = (string)reader[2];
                }
                if (reader.IsDBNull(4))
                {
                    newsData.mediaNewsEnContent = "";
                }
                else
                {
                    newsData.mediaNewsEnContent = (string)reader[4];
                }
                if (reader.IsDBNull(6))
                {
                    newsData.latestMediaNewsPicPath = "";
                }
                else
                {
                    newsData.latestMediaNewsPicPath = "~/Scripts/img/latestMediaNews/" + (string)reader[6];
                }
                string format = "yyyy-MM-dd";
                newsData.date = ((DateTime)reader[7]).ToString(format);
                newsData.mediaNewsMovieUrl = "";
                mediaNewsList.Add(newsData);
            }
            connection.Close();
            return mediaNewsList;
        }
    }
}
