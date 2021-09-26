using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using xiaotasi.Pojo;

namespace xiaotasi.Service.Impl
{
    public class NewsServiceImpl : NewsService
    {

        public List<NewsPojo> getLatestNewsList()
        {
            SqlConnection connection = new SqlConnection("Server = localhost; User ID = sa; Password = reallyStrongPwd123; Database = tasiTravel");
            // SQL Command
            SqlCommand select = new SqlCommand("select * from latest_news_list", connection);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<NewsPojo> newsList = new List<NewsPojo>();
            while (reader.Read())
            {
                NewsPojo newsData = new NewsPojo();
                newsData.newsId = (int)reader[0];
                newsData.newsTraditionalTitle = (string)reader[2];
                newsData.newsTraditionalContent = (string)reader[3];
                if (reader.IsDBNull(10))
                {
                    newsData.newsEnTitle = "";
                }
                else
                {
                    newsData.newsEnTitle = (string)reader[10];
                }
                if (reader.IsDBNull(11))
                {
                    newsData.newsEnContent = "";
                }
                else
                {
                    newsData.newsEnContent = (string)reader[11];
                }
                if (reader.IsDBNull(5))
                {
                    newsData.newsPicPath = "";
                }
                else
                {
                    newsData.newsPicPath = "~/Scripts/img/latestNews/" + (string)reader[5];
                }
                string format = "yyyy-MM-dd";
                newsData.date = ((DateTime)reader[8]).ToString(format);
                newsData.newsUrl = "";
                newsList.Add(newsData);
            }
            connection.Close();
            return newsList;
        }

        public List<MediaNewsPojo> getMediaNewsList()
        {
            SqlConnection connection = new SqlConnection("Server = localhost; User ID = sa; Password = reallyStrongPwd123; Database = tasiTravel");
            // SQL Command
            SqlCommand select = new SqlCommand("select * from latest_media_news_list", connection);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<MediaNewsPojo> mediaNewsList = new List<MediaNewsPojo>();
            while (reader.Read())
            {
                MediaNewsPojo newsData = new MediaNewsPojo();
                newsData.mediaNewsId = (int)reader[0];
                newsData.mediaNewsTraditionalTitle = (string)reader[2];
                newsData.mediaNewsTraditionalContent = (string)reader[3];
                if (reader.IsDBNull(10))
                {
                    newsData.mediaNewsEnTitle = "";
                }
                else
                {
                    newsData.mediaNewsEnTitle = (string)reader[10];
                }
                if (reader.IsDBNull(11))
                {
                    newsData.mediaNewsEnContent = "";
                }
                else
                {
                    newsData.mediaNewsEnContent = (string)reader[11];
                }
                if (reader.IsDBNull(5))
                {
                    newsData.latestMediaNewsPicPath = "";
                }
                else
                {
                    newsData.latestMediaNewsPicPath = "~/Scripts/img/latestMediaNews/" + (string)reader[5];
                }
                string format = "yyyy-MM-dd";
                newsData.date = ((DateTime)reader[8]).ToString(format);
                newsData.mediaNewsMovieUrl = "";
                mediaNewsList.Add(newsData);
            }
            connection.Close();
            return mediaNewsList;
        }
    }
}
