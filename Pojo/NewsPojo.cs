using System;
namespace xiaotasi.Pojo
{
    public class NewsPojo
    {
        public int newsId { get; set; }
        public string newsTraditionalTitle { get; set; }
        public string newsEnTitle { get; set; }
        public string newsTraditionalContent { get; set; }
        public string newsEnContent { get; set; }
        public string date { get; set; }
        public string newsPicPath { get; set; }
        public string newsUrl { get; set; }
    }

    public class MediaNewsPojo
    {
        public int mediaNewsId { get; set; }
        public string mediaNewsTraditionalTitle { get; set; }
        public string mediaNewsEnTitle { get; set; }
        public string mediaNewsTraditionalContent { get; set; }
        public string mediaNewsEnContent { get; set; }
        public string date { get; set; }
        public string mediaNewsMovieUrl { get; set; }
        public string latestMediaNewsPicPath { get; set; }
    }
}
