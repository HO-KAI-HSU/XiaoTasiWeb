using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace xiaotasi.Service
{
    public interface FileService
    {
        Task<int> uploadFile(IFormFile file, string name, string path, int maxPicSize, List<string> picFormat);

        Task<string> uploadFileToStorage(IFormFile file, string containerName, string groupName);
    }
}
