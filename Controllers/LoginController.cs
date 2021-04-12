using System; 
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;
using xiaotasi.Service;

namespace xiaotasi.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration configuration;

        public LoginController(ILogger<LoginController> logger, IConfiguration config)
        {
            _logger = logger;
            configuration = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MemberInfo()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //登入
        [HttpPost]
        public IActionResult login(string username, string password)
        {
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            if ((username == null || username.Length == 0) || (password == null || password.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            SqlCommand select = new SqlCommand("select account_id, username, password, member_code as memberCode, status, phone from account_list WHERE username = @username", connection);
            select.Parameters.AddWithValue("@username", username);
            // 開啟資料庫連線
            connection.Open();
            LoginModel loginData = new LoginModel();
            SqlDataReader reader = select.ExecuteReader();
            int readStatus = 0;
            while (reader.Read())
            {
                loginData.id = (int)reader[0];
                if (reader.IsDBNull(2) || !reader[2].ToString().Equals(password))
                {
                    return Json(new ApiError(1002, "password error!", "此帐户密码有误，请重新输入！"));
                }
                loginData.username = (string)reader[1];
                loginData.memberCode = (string)reader[3];
                loginData.status = (int)reader[4];
                loginData.phone = (string)reader[5];
                readStatus++;
            }
            if (readStatus == 0)
            {
                return Json(new ApiError(1003, "No such user info in database!", "此账户不存在，请先註冊帳戶！"));
            }
            IAuthContainerModel model = GetJWTContainerModel(loginData.phone, loginData.memberCode);
            IAuthService authService = new JWTService(model.secretKey);
            string token = authService.generateToken(model);
            loginData.token = token;
            return Json(new ApiResult<object>(loginData));
        }

        //登出
        [HttpPost]
        public IActionResult logout(string accToken)
        {
            // SQL Command
            if ((accToken == null || accToken.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            
            return Json(new ApiResult<object>("Logout Success"));
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
