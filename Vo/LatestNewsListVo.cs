using System;
using System.Collections.Generic;
using xiaotasi.Pojo;

namespace xiaotasi.Vo
{
    public class LatestNewsListVo
    {
        public int success { get; set; }

        public int count { get; set; }

        public int page { get; set; }

        public int limit { get; set; }

        public List<NewsPojo> lateNewsList { get; set; }
    }
}