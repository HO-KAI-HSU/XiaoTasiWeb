using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Models;
using xiaotasi.Service;

namespace xiaotasi.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration configuration;
        JWTContainerModel model;
        IAuthService authService;

        public AuthController()
        {
        }

        public AuthController(IConfiguration config)
        {
            configuration = config;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //API驗證
        public bool _apiAuth(string token, string memberCode, string phone)
        {
            model = GetJWTContainerModel(phone, memberCode);
            authService = new JWTService(model.secretKey);
            if (!authService.isTokenVaild(token))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        // 令牌是否過期
        public async Task<bool> isTokenExp(string token = "")
        {
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);

            bool flag = true;

            // 取得當前時間
            int timestamp = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            string time = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine("time : {0}", time);

            // SQL Command
            SqlCommand select = new SqlCommand("select member_code as memberCode from account_list WHERE token_exp > @time and token = @token", connection);
            select.Parameters.AddWithValue("@time", time);
            select.Parameters.AddWithValue("@token", token);

            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine("memberCode : {0}", (string)reader[0]);
                flag = false;
            }
            return flag;
        }

        #region Private Methods
        private static JWTContainerModel GetJWTContainerModel(string phone, string memberCode)
        {
            return new JWTContainerModel()
            {
                claims = new Claim[]
                {
                    new Claim(ClaimTypes.MobilePhone, phone),
                    new Claim(ClaimTypes.SerialNumber, memberCode)
                }
            };
        }
        #endregion
    }
}
