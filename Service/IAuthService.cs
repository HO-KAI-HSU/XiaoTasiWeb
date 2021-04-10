using System;
using System.Collections.Generic;
using System.Security.Claims;
using xiaotasi.Models;

namespace xiaotasi.Service
{
    public interface IAuthService
    {
        // 密鑰
        string secretKey { get; set; }

        // 驗證token是否有效
        bool isTokenVaild(string token);

        // 產生token
        string generateToken(IAuthContainerModel model);

        // 取得 token
        IEnumerable<Claim> getTokenClaims(string token);
    }
}
