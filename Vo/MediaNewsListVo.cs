using System;
using System.Collections.Generic;
using xiaotasi.Pojo;

namespace xiaotasi.Vo
{
    public class MediaNewsListVo
    {
        public int success { get; set; }

        public int count { get; set; }

        public int page { get; set; }

        public int limit { get; set; }

        public List<MediaNewsPojo> mediaNewsList { get; set; }
    }
}