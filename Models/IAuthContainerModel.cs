using System;
using System.Security.Claims;

namespace xiaotasi.Models
{
    public interface IAuthContainerModel
    {
        #region Members
        // 密鑰
        string secretKey { get; set; }

        // 安全演算法
        string securityAlgorithm { get; set; }

        // token 過期時間
        int expireMinutes { get; set; }

        //
        Claim[] claims { get; set; }
        #endregion
    }
}
