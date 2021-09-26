using System; 
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;
 
namespace xiaotasi.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration configuration;

        public RegisterController(ILogger<LoginController> logger, IConfiguration config)
        {
            _logger = logger;
            configuration = config;
        }


        [HttpPost]
        public ActionResult RegisterByPhone(string username, string password, string name, string email, string address, string birthday, string telephone, string cellphone, string emerContactName, string emerContactPhone)
        {
            if ((cellphone == null || cellphone.Length == 0) || (username == null || username.Length == 0) || (password == null || password.Length == 0) || (name == null || name.Length == 0) || (email == null || email.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
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

            // 新增會員資訊
            SqlCommand addMember = new SqlCommand("INSERT INTO member_list (member_code, email, name, address, birthday, telephone, emer_contact_name, emer_contact_phone )" +
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
            "VALUES ('" + username + "', '" + password + "',  '" + cellphone + "',  '" + memberCode + "',  " + 0 + ")", connection);
            connection.Open();
            addAccount.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Register Success, please auth by phone", "註冊成功，請完成手機驗證開通程序"));
        }

        [HttpPost]
        // 傳送手機驗證碼
        public ActionResult GetPhoneCaptcha(string cellphone)
        {
            if (cellphone == null || cellphone.Length == 0)
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // 取得帳號資訊
            SqlCommand getMemberSelect = new SqlCommand("select * from account_list WHERE phone = @cellphone and status = @status", connection);
            getMemberSelect.Parameters.AddWithValue("@cellphone", cellphone);
            getMemberSelect.Parameters.AddWithValue("@status", 1);
            connection.Open();
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            while (getMemberReader.Read())
            {
                if (!getMemberReader.IsDBNull(0))
                {
                    return Json(new ApiError(1014, "Phone used!", "電話已被註冊，請重新輸入！！"));
                }
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
        public ActionResult VerifyPhoneCaptcha(string cellphone, string captcha)
        {
            if ((cellphone == null || cellphone.Length == 0) || (captcha == null || captcha.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
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
            this._updateAccountRegisterStatus(cellphone);
            return Json(new ApiResult<string>("Auth Success", "開通成功，請重新登入"));
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
            SqlCommand select = new SqlCommand("UPDATE account_list SET status = " + 1 + ", e_date = '" + updateDate + "'  WHERE phone = @cellphone", connection);
            select.Parameters.AddWithValue("@cellphone", cellphone);
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return true;
        }

    }
}
