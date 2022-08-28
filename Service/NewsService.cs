using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using xiaotasi.Pojo;

namespace xiaotasi.Service
{
    public interface NewsService
    {
        Task<List<NewsPojo>> getLatestNewsList();

        Task<List<MediaNewsPojo>> getMediaNewsList();
    }
}
