using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace xiaotasi.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {
        #region Public Methods
        public string secretKey { get; set; } = "eGlhb1Rhc2kxMjM0NTY3OA==";
        public string securityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int expireMinutes { get; set; } = 10800;
        public Claim[] claims { get; set; }
        #endregion
    }
}
