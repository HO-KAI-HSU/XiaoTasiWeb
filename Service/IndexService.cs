using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using xiaotasi.Pojo;

namespace xiaotasi.Service
{
    public interface IndexService
    {
        Task<List<IndexBannerPojo>> getIndexBannerList();
    }
}
