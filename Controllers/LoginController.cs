using System;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Models;
using xiaotasi.Service;

namespace xiaotasi.Controllers
{
    public class LoginController : Controller
    {
        //private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly MemberService _memberService;
        IAuthService authService;
        IAuthContainerModel model;


        public LoginController(IConfiguration config, MemberService memberService)
        {
            _config = config;
            _memberService = memberService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //登入
        [HttpPost]
        public async Task<IActionResult> login(string username, string password)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);

            // SQL Command
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Json(new ApiError(1002, "Account or password error!", "此帳戶或密碼有誤，請重新輸入！"));
            }
            SqlCommand select = new SqlCommand("select account_id, username, password, member_code as memberCode, status, phone from account_list WHERE username = @username and status = 1", connection);
            select.Parameters.AddWithValue("@username", username);
            // 開啟資料庫連線
            await connection.OpenAsync();
            LoginModel loginData = new LoginModel();
            SqlDataReader reader = select.ExecuteReader();
            int readStatus = 0;
            while (reader.Read())
            {
                loginData.id = (int)reader[0];
                if (reader.IsDBNull(2) || !reader[2].ToString().Equals(password))
                {
                    return Json(new ApiError(1002, "Account or password error!", "此帳戶或密碼有誤，請重新輸入！"));
                }
                loginData.username = (string)reader[1];
                loginData.memberCode = (string)reader[3];
                loginData.status = (int)reader[4];
                loginData.phone = (string)reader[5];
                readStatus = (int)reader[4];
            }
            connection.Close();
            if (readStatus == 0)
            {
                return Json(new ApiError(1002, "Account not open!", "帳號未開通！"));
            }

            // 取得會員資訊
            MemberInfoModel member = await _memberService.getMemberInfo(loginData.memberCode, "");
            model = GetJWTContainerModel(loginData.phone, loginData.memberCode);
            authService = new JWTService(model.secretKey);
            string token = authService.generateToken(model);

            // 登入時將token存至數據表內
            await this.addTokenToDb(token, username);

            loginData.token = token;
            loginData.name = member.name;
            return Json(new ApiResult<object>(loginData));
        }

        //登出
        [HttpPost]
        public async Task<IActionResult> logout(string accToken)
        {
            //// SQL Command
            //if ((accToken == null || accToken.Length == 0))
            //{
            //    return Json(new ApiError(1001, "Required field(s) is missing!", "必需參數缺失！"));
            //}
            return Json(new ApiResult<string>("Logout Success", "成功登出系統"));
        }


        // 新增旅遊預定會員資訊
        private async Task addTokenToDb(string token, string username)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string cmdText = "UPDATE account_list set token = @token, token_exp = @tokenExp, token_iat = @tokenIat where username = @username";
            int timestampExp = Convert.ToInt32(DateTime.UtcNow.AddHours(10).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            int timestampIat = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            string tokenExp = DateTimeOffset.FromUnixTimeSeconds(timestampExp).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string tokenIat = DateTimeOffset.FromUnixTimeSeconds(timestampIat).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            SqlCommand select = new SqlCommand(cmdText, connection);
            select.Parameters.AddWithValue("@username", username);
            select.Parameters.Add("@token", SqlDbType.VarChar).Value = token;
            select.Parameters.Add("@tokenExp", SqlDbType.NVarChar).Value = tokenExp;
            select.Parameters.Add("@tokenIat", SqlDbType.NVarChar).Value = tokenIat;
            //開啟資料庫連線
            await connection.OpenAsync();
            select.ExecuteNonQuery();
            connection.Close();
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
