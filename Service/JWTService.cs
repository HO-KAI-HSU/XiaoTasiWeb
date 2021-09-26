using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using xiaotasi.Models;

namespace xiaotasi.Service
{
    public class JWTService : IAuthService
    {
        #region Members
        // 密鑰
        public string secretKey { get; set; }
        #endregion

        #region Constructor
        // 
        public JWTService(string secretKey)
        {
            this.secretKey = secretKey;
        }
        #endregion

        public string generateToken(IAuthContainerModel model)
        {
            if (model == null || model.claims == null || model.claims.Length == 0)
            {
                throw new ArgumentException("Token are not valid");
            }
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(model.claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(model.expireMinutes)),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), model.securityAlgorithm)
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            string token = jwtSecurityTokenHandler.WriteToken(securityToken);
            return token;
        }

        public IEnumerable<Claim> getTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token is Null or Empty");
            }
            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validationToken);
                return tokenValid.Claims;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool isTokenVaild(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token is Null or Empty");
            }
            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Private method
        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(secretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }
        #endregion
    }
}
