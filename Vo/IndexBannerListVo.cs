using System;
using System.Collections.Generic;
using xiaotasi.Pojo;

namespace xiaotasi.Vo
{
    public class IndexBannerListVo
    {
        public int success { get; set; }

        public int count { get; set; }

        public int page { get; set; }

        public int limit { get; set; }

        public List<IndexBannerPojo> indexBannerList { get; set; }
    }
}