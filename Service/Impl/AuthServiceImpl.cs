using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace xiaotasi.Service.Impl
{
    public class AuthServiceImpl : AuthService
    {
        private readonly IConfiguration _config;

        public AuthServiceImpl(IConfiguration config)
        {
            _config = config;
        }

        public int apiAuth(string token, int featureAccessLevel, int paramsAuthStatus)
        {
            int errorCode = 0;

            // 確認必傳參數
            errorCode = this.checkRequiredInput(paramsAuthStatus);
            if (errorCode > 0)
            {
                return errorCode;
            }
            if (token != null && token != "")
            {
                //if (featureAccessLevel > 1)
                //{
                //    // 是否為已開通帳戶
                //    errorCode = this.isAvailableAccount(token);
                //    if (errorCode > 0)
                //    {
                //        return errorCode;
                //    }

                //    // 是否為正在使用設備
                //    errorCode = this.isUseDevice(token);
                //    if (errorCode > 0)
                //    {
                //        return errorCode;
                //    }
                //}

                // 訪問令牌時效是否失效
                errorCode = this.isTokenExp(token);
                if (errorCode > 0)
                {
                    return errorCode;
                }
            }
            return errorCode;
        }

        // 訪問令牌時效是否失效
        private int isTokenExp(string token)
        {
            Console.WriteLine("----isTokenExp----");
            int errorCode = 90010;
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);

            // 取得當前時間
            int timestamp = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            string time = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine("time : {0}", time);

            // SQL Command
            SqlCommand select = new SqlCommand("select member_code as memberCode from account_list WHERE token_exp > @time and token = @token", connection);
            select.Parameters.AddWithValue("@time", time);
            select.Parameters.AddWithValue("@token", token);

            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("memberCode : {0}", (string)reader[0]);
                errorCode = 0;
            }
            Console.WriteLine("errorCode : {0}", errorCode);
            return errorCode;
        }

        // 確認必傳參數
        private int checkRequiredInput(int paramsAuthStatus)
        {
            int errorCode = 0;
            if (paramsAuthStatus == 1)
            {
                errorCode = 90011;
            }
            return errorCode;
        }

        //// 是否為已開通帳戶
        //private int isAvailableAccount(string token)
        //{
        //    AccountDb accountDb = new AccountDb();
        //    accountDb._db = _db;
        //    AccountPojo accountPojo = accountDb.GetAccountInfo("", token);
        //    int errorCode = accountPojo.accountStatus == 0 ? 90013 : 0;
        //    return errorCode;
        //}
    }
}
