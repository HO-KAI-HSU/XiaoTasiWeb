using System;
using static xiaotasi.Vo.ApiResult;

namespace xiaotasi.Service
{
    public interface ApiResultService
    {
        ApiError1 apiAuth(string token, string lang, int osType, int featureAccessLevel, int paramsAuthStatus);

        ApiResult1<string> apiResult(string lang, int osType, string returnCode);

        ApiError1 apiAFailResult(string lang, int osType, int returnCode, string extraStr);
    }
}
