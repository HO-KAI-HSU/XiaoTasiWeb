using System;
using Microsoft.Extensions.Configuration;
using static xiaotasi.Vo.ApiResult;

namespace xiaotasi.Service.Impl
{
    public class ApiResultServiceImpl : ApiResultService
    {
        private readonly AuthService _authService;
        private readonly LangService _langService;
        private readonly IConfiguration _config;

        public ApiResultServiceImpl(IConfiguration config, AuthService authService, LangService langService)
        {
            _config = config;
            _authService = authService;
            _langService = langService;
        }

        public ApiError1 apiAuth(string token, string lang, int osType, int featureAccessLevel, int paramsAuthStatus = 2)
        {
            int errorcode = _authService.apiAuth(token, featureAccessLevel, paramsAuthStatus);
            int langIndex = _langService.langMatch(lang);
            if (errorcode > 0)
            {
                var errorCodeMsg = _config.GetValue<string>("ErrorCodes:" + errorcode.ToString() + ":" + 0);
                var errorCodeReas = _config.GetValue<string>("ErrorCodes:" + errorcode.ToString() + ":" + langIndex);
                return new ApiError1(errorcode, errorCodeMsg, errorCodeReas);
            }
            else
            {
                return new ApiError1(0, "", "");
            }
        }

        public ApiError1 apiAFailResult(string lang, int osType, int returnCode, string extraStr = "")
        {
            int langIndex = _langService.langMatch(lang);
            var errorCodeMsg = "";
            var errorCodeReas = "";
            if (extraStr != "")
            {
                errorCodeMsg = string.Format(_config.GetValue<string>("ErrorCodes:" + returnCode.ToString() + ":" + 0), extraStr);
                errorCodeReas = string.Format(_config.GetValue<string>("ErrorCodes:" + returnCode.ToString() + ":" + langIndex), extraStr);
            }
            else
            {
                errorCodeMsg = _config.GetValue<string>("ErrorCodes:" + returnCode.ToString() + ":" + 0);
                errorCodeReas = _config.GetValue<string>("ErrorCodes:" + returnCode.ToString() + ":" + langIndex);
            }

            return new ApiError1(returnCode, errorCodeMsg, errorCodeReas);
        }

        public ApiResult1<string> apiResult(string lang, int osType, string returnCode)
        {
            int langIndex = _langService.langMatch(lang);
            var returnCodeMsg = _config.GetValue<string>("ReturnCodes:" + returnCode + ":" + 0);
            var returnCodeReas = _config.GetValue<string>("ReturnCodes:" + returnCode + ":" + langIndex);
            return new ApiResult1<string>(returnCodeMsg, returnCodeReas);
        }
    }
}
