using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using xiaotasi.Helpers;

namespace xiaotasi.Service.Impl
{
    public class FileServiceImpl : FileService
    {
        IConfiguration _config; 

        public FileServiceImpl(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> uploadFile(IFormFile file, string name, string path, int maxPicSize, List<string> picFormat)
        {
            try
            {
                int errorCode = 0;
                //1 check if the file length is greater than 0 bytes 
                if (file.Length > 0)
                {
                    string fileName = file.FileName;

                    //2 Get the extension of the file
                    string extension = Path.GetExtension(fileName);
                    string fileNameNew = name + extension;
                    if (file.Length > maxPicSize)
                    {
                        errorCode = 90044;
                    }
                    else if (extension == null)
                    {
                        errorCode = 90045;
                    }
                    else if (!picFormat.Contains(extension))
                    {
                        errorCode = 90046;
                    }
                    else
                    {
                        //4 set the path where file will be copied
                        string filePath = Path.GetFullPath(
                            Path.Combine(Directory.GetCurrentDirectory(),
                                                        path));
                        //5 copy the file to the path
                        using (var fileStream = new FileStream(
                            Path.Combine(filePath, fileNameNew),
                                            FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            errorCode = 0;
                        }
                    }
                }
                else
                {
                    errorCode = 90048;
                }
                return errorCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> uploadFileToStorage(IFormFile file, string containerName, string groupName)
        {
            try
            {
                Console.WriteLine(containerName);
                Console.WriteLine(groupName);
                Console.WriteLine(file);
                string picPathUrl = "";
                AzureStorageHelper azureStorageHelper = new AzureStorageHelper(_config);

                using (Stream stream = file.OpenReadStream())
                {
                    picPathUrl = await azureStorageHelper.UploadFileToStorage(stream, containerName, groupName, this.createPicName() + ".jpeg");
                }
                return picPathUrl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // 產生圖片名稱
        private string createPicName()
        {
            string n = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            return "bankAccountRecord" + n;
        }
    }
}
