using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using xiaotasi.Bo;
using xiaotasi.Service;
using xiaotasi.Vo;
using static xiaotasi.Vo.ApiResult;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace xiaotasi.Controllers
{
    public class FileController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApiResultService _apiResultService;
        private readonly FileService _fileService;

        public FileController(ApiResultService apiResultService, FileService fileService, IConfiguration config)
        {
            _apiResultService = apiResultService;
            _fileService = fileService;
            _config = config;
        }

        // POST UploadPic
        [HttpPost]
        public async Task<IActionResult> UploadPic([FromForm] UploadPicBo uploadPicBo)
        {
            Console.WriteLine("----------UploadPic-----------");
            Console.WriteLine("UploadPicBo : {0},{1}", uploadPicBo.token, uploadPicBo.picType);
            Console.WriteLine("----------UploadPic-----------");
            string path = "";
            string picPath = "";
            string picName = "";
            int maxPicSize = 0;
            string picFormatStr = "";
            List<string> picFormat = new List<string>();
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));
            if (uploadPicBo.picType == 1)
            {
                string extension = Path.GetExtension(uploadPicBo.file.FileName);
                picFormatStr = ".png/.jpeg/.jpg";
                picFormat = picFormatStr.Split(new char[] { '/' }).ToList();
                path = "wwwroot/images/reservationCheck/";
                picName = "pic" + Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                picPath = domainUrl + "/images/reservationCheck/" + picName + extension;
                maxPicSize = 819200;
            }

            int errorCode = await _fileService.uploadFile(uploadPicBo.file, picName, path, maxPicSize, picFormat);
            if (errorCode > 0)
            {
                string extraStr = "";
                if (errorCode == 90044)
                {
                    extraStr = maxPicSize.ToString();
                }
                else
                {
                    extraStr = picFormatStr;
                }
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 0, errorCode, extraStr);
                return Json(apiError);
            }
            ApiResult1<string> apiRes = _apiResultService.apiResult("zh-tw", 0, "ULPS001");
            UploadPicVo uploadPicVo = new UploadPicVo();
            uploadPicVo.success = 1;
            uploadPicVo.message = apiRes.message;
            uploadPicVo.reason = apiRes.reason;
            uploadPicVo.picPath = picPath;
            return Json(uploadPicVo);
        }

        // POST UploadPic
        [HttpPost]
        public async Task<IActionResult> UploadPicFromMgt([FromForm] UploadPicBo uploadPicBo)
        {
            Console.WriteLine("----------UploadPicFromMgt-----------");
            Console.WriteLine("UploadPicBo : {0}",uploadPicBo.picType);
            Console.WriteLine("----------UploadPicFromMgt-----------");
            string path = "";
            string picPath = "";
            string picName = "";
            int maxPicSize = 0;
            string picFormatStr = "";
            List<string> picFormat = new List<string>();
            var domainUrl = string.Format(_config.GetValue<string>("Domain"));

            
            if (uploadPicBo.picType == 2)
            {
                // 行程圖
                string extension = Path.GetExtension(uploadPicBo.file.FileName);
                picFormatStr = ".png/.jpeg/.jpg";
                picFormat = picFormatStr.Split(new char[] { '/' }).ToList();
                path = "wwwroot/images/trip/";
                picName = "trip" + Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                picPath = domainUrl + "/images/trip/" + picName + extension;
                maxPicSize = 819200;
            }
            else if (uploadPicBo.picType == 3)
            {
                // 消息圖
                string extension = Path.GetExtension(uploadPicBo.file.FileName);
                picFormatStr = ".png/.jpeg/.jpg";
                picFormat = picFormatStr.Split(new char[] { '/' }).ToList();
                path = "wwwroot/images/news/";
                picName = "news" + Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                picPath = domainUrl + "/images/news/" + picName + extension;
                maxPicSize = 819200;
            }
            else if (uploadPicBo.picType == 4)
            {
                // 媒體報導圖
                string extension = Path.GetExtension(uploadPicBo.file.FileName);
                picFormatStr = ".png/.jpeg/.jpg";
                picFormat = picFormatStr.Split(new char[] { '/' }).ToList();
                path = "wwwroot/images/mediaNews/";
                picName = "media" + Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                picPath = domainUrl + "/images/latestMediaNews/" + picName + extension;
                maxPicSize = 819200;
            }
            else if (uploadPicBo.picType == 5)
            {
                // 首頁banner圖
                string extension = Path.GetExtension(uploadPicBo.file.FileName);
                picFormatStr = ".png/.jpeg/.jpg";
                picFormat = picFormatStr.Split(new char[] { '/' }).ToList();
                path = "wwwroot/images/index/";
                picName = "ban" + Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                picPath = domainUrl + "/images/index/" + picName + extension;
                maxPicSize = 819200;
            }

            int errorCode = await _fileService.uploadFile(uploadPicBo.file, picName, path, maxPicSize, picFormat);
            if (errorCode > 0)
            {
                string extraStr = "";
                if (errorCode == 90044)
                {
                    extraStr = maxPicSize.ToString();
                }
                else
                {
                    extraStr = picFormatStr;
                }
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 0, errorCode, extraStr);
                return Json(apiError);
            }
            ApiResult1<string> apiRes = _apiResultService.apiResult("zh-tw", 0, "ULPS001");
            UploadPicVo uploadPicVo = new UploadPicVo();
            uploadPicVo.success = 1;
            uploadPicVo.message = apiRes.message;
            uploadPicVo.reason = apiRes.reason;
            uploadPicVo.picName = picName;
            uploadPicVo.picPath = picPath;
            return Json(uploadPicVo);
        }
    }
}
