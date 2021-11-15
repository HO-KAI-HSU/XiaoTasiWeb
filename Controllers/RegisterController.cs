using System; 
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;
using xiaotasi.Service;
using static xiaotasi.Vo.ApiResult;

namespace xiaotasi.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration configuration;
        private readonly ApiResultService _apiResultService;
        private readonly RegisterService _registerService;

        public RegisterController(ILogger<LoginController> logger, IConfiguration config, ApiResultService apiResultService, RegisterService registerService)
        {
            _logger = logger;
            configuration = config;
            _registerService = registerService;
            _apiResultService = apiResultService;
        }


        [HttpPost]
        public ActionResult RegisterByPhone(string username, string password, string name, string email, string address, string birthday, string telephone, string cellphone, string emerContactName, string emerContactPhone, int checkFlag = 1)
        {
            if ((cellphone == null || cellphone.Length == 0) || (username == null || username.Length == 0) || (password == null || password.Length == 0) || (name == null || name.Length == 0) || (birthday == null || birthday.Length == 0))
            {
                string errorStr = "";
                if (cellphone == null || (cellphone.Length == 0 || cellphone.Length != 10))
                {
                    errorStr = "請輸入手機號碼或輸入完整的手機號碼！";
                }
                else if (username == null || username.Length == 0)
                {
                    errorStr = "請輸入身分證號！";
                }
                else if (password == null || password.Length == 0)
                {
                    errorStr = "請輸入密碼！";
                }
                else if (name == null || name.Length == 0)
                {
                    errorStr = "請輸入姓名！";
                }
                else if (birthday == null || birthday.Length == 0)
                {
                    errorStr = "請輸入生日！";
                }
                if (errorStr != "") {
                    return Json(new ApiError(1001, "Required field(s) is missing!", errorStr));
                }
            }
            int timpStamp = this.getTimestamp();
            string memberCode = "mem" + timpStamp.ToString().Substring(2, 8);
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // 開啟資料庫連線

            // 取得帳號資訊
            SqlCommand getMemberSelect = new SqlCommand("select * from account_list WHERE username = @username", connection);
            getMemberSelect.Parameters.AddWithValue("@username", username);
            connection.Open();
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            while (getMemberReader.Read())
            {
                if (!getMemberReader.IsDBNull(0))
                {
                    return Json(new ApiError(0003, "Username used!", "此身分證號已被使用，請重新輸入！"));
                }
            }
            connection.Close();

            // checkFlag 0 表示已驗證手機並新增帳戶
            if (checkFlag == 0)
            {
                // 新增會員資訊
                SqlCommand addMember = new SqlCommand("INSERT INTO member_list (member_code, email, name, address, birthday, telephone, emer_contact_name, emer_contact_phone)" +
                "VALUES ('" + memberCode + "', '" + email + "',  @name, @address, @birthday, @telephone, @emerContactName, @emerContactPhone)", connection);
                addMember.Parameters.Add("@birthday", SqlDbType.NVarChar).Value = birthday == null ? "" : birthday;
                addMember.Parameters.Add("@telephone", SqlDbType.NVarChar).Value = telephone == null ? "" : telephone;
                addMember.Parameters.Add("@emerContactName", SqlDbType.NVarChar).Value = emerContactName == null ? "" : emerContactName;
                addMember.Parameters.Add("@emerContactPhone", SqlDbType.NVarChar).Value = emerContactPhone == null ? "" : emerContactPhone;
                addMember.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                addMember.Parameters.Add("@address", SqlDbType.NVarChar).Value = address == null ? "" : address;
                connection.Open();
                addMember.ExecuteNonQuery();
                connection.Close();

                // 新增帳號資訊
                SqlCommand addAccount = new SqlCommand("INSERT INTO account_list (username, password, phone, member_code, status)" +
                "VALUES ('" + username + "', '" + password + "',  '" + cellphone + "',  '" + memberCode + "',  " + 1 + ")", connection);
                connection.Open();
                addAccount.ExecuteNonQuery();
                connection.Close();
                return Json(new ApiResult<string>("Auth Success", "開通成功，請重新登入"));
            }
            else
            {
                return Json(new ApiResult<string>("Send Success", "已發送驗證碼"));
            }
        }

        [HttpPost]
        // 傳送手機驗證碼
        public ActionResult GetPhoneCaptcha(string cellphone, string verificationType)
        {
            int paramsAuthStatus = 0;
            if (cellphone == null || cellphone.Length == 0)
            {
                paramsAuthStatus = 1;
            }
            // 初始驗證
            ApiError1 apiAuth = _apiResultService.apiAuth("", "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // 取得帳號資訊
            int errorCode = _registerService.isRegisterStatusByPhone(cellphone, verificationType);

            if (errorCode > 0)
            {
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 2, errorCode, "");
                return Json(apiError);
            }

            connection.Close();
            int rand = this.getTimestamp();
            string captcha = rand.ToString().Substring(4, 6);
            Every8d every8 = new Every8d();
            string smsRes = every8.sendSMS("您的驗證碼為" + captcha, cellphone);
            HttpContext.Session.SetString("phone", cellphone);
            HttpContext.Session.SetInt32("captcha_time", rand);
            HttpContext.Session.SetString("captcha", captcha);
            return Json(new ApiResult<string>("Send Success", "已發送驗證碼"));
        }


        [HttpPost]
        // 驗證手機驗證碼
        public ActionResult verifyPhoneCaptcha(string cellphone, string captcha)
        {
            if ((cellphone == null || cellphone.Length == 0) || (captcha == null || captcha.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需參數缺失！"));
            }
            if (HttpContext.Session.GetString("phone") == null || !HttpContext.Session.GetString("phone").Equals(cellphone))
            {
                return Json(new ApiError(1010, "Error phone!", "電話輸入錯誤或尚未取得驗證碼，請重新輸入或重新取得驗證碼！"));
            }
            if (HttpContext.Session.GetString("captcha") == null || !HttpContext.Session.GetString("captcha").Equals(captcha))
            {
                return Json(new ApiError(1011, "Error captcha!", "驗證碼輸入錯誤或尚未取得驗證碼，請重新輸入或重新取得驗證碼！"));
            }
            int timpStamp = this.getTimestamp();
            string phoneSess = HttpContext.Session.GetString("phone").ToString();
            string captchaSess = HttpContext.Session.GetString("captcha").ToString();
            int timpStampSess = (int)HttpContext.Session.GetInt32("captcha_time");
            int diff = timpStamp - timpStampSess;
            if (diff > 55)
            {
                return Json(new ApiError(1012, "Captcha Expired!", "驗證碼已過期，請重新取得驗證碼！"));
            }
            else
            {
                return Json(new ApiResult<string>("Auth Success", "手機驗證成功"));
            }
        }

        [HttpPost]
        // 重置密碼
        public ActionResult resetPassword(string cellphone, string newPassword, string reKeyinPassword)
        {
            int paramsAuthStatus = 0;
            if ((cellphone == null || cellphone.Length == 0) || (newPassword == null || newPassword.Length == 0) || (reKeyinPassword == null || reKeyinPassword.Length == 0))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = _apiResultService.apiAuth("", "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            // 新密碼匹配是否正確
            if (newPassword != reKeyinPassword)
            {
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 2, 90018, "");
                return Json(apiError);
            }

            // 重置密碼
            _registerService.resetPassword(cellphone, newPassword);

            return Json(new ApiResult<string>("Reset Password Success", "密碼設定成功，請重新登入"));
        }

        //產生亂數（使用時間戳）
        private int getTimestamp()
        {
            DateTime gtm = new DateTime(1970, 1, 1);//宣告一個GTM時間出來
            DateTime utc = DateTime.UtcNow.AddHours(8);//宣告一個目前的時間
            int timeStamp = Convert.ToInt32(((TimeSpan)utc.Subtract(gtm)).TotalSeconds);
            return timeStamp;
        }

        //更新註冊狀態
        private bool _updateAccountRegisterStatus(string cellphone)
        {
            string updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//宣告一個目前的時間
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand select = new SqlCommand("UPDATE account_list SET  = " + 1 + ", e_date = '" + updateDate + "'  WHERE phone = @cellphone", connection);
            select.Parameters.AddWithValue("@cellphone", cellphone);
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return true;
        }

    }
}
