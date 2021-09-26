using System;
using System.Collections.Generic;
using xiaotasi.Pojo;

namespace xiaotasi.Service
{
    public interface NewsService
    {
        List<NewsPojo> getLatestNewsList();

        List<MediaNewsPojo> getMediaNewsList();
    }
}
