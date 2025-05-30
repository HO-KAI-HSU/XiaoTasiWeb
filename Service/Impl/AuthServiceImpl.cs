using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public async Task<int> apiAuth(string token, int featureAccessLevel, int paramsAuthStatus)
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
                errorCode = await this.isTokenExp(token);
                if (errorCode > 0)
                {
                    return errorCode;
                }
            }
            return errorCode;
        }

        public int isValidIDorRCNumber(string id)
        {
            int Esum = 0;
            int Nsum = 0;
            int count = 0;
            int errorCode = 0;
            if (string.IsNullOrWhiteSpace(id))
            {
                errorCode = 0010;
                return errorCode;
            }
            var pattern = @"^[A-Z]{1}[1-2]{1}[0-9]{8}$";
            bool flag = Regex.IsMatch(id, pattern);//先判定是否符合一個大寫字母+1或2開頭的1個數字+8個數字                                                             //先用正規運算式判斷是否符合格式
            var idCode = id.ToUpper();//把字母轉成大寫
            if (flag)//如果符合第一層格式
            {   //宣告一個陣列放入A~Z相對應數字的順序 
                string[] country = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
                for (int index = 0; index < country.Length; index++)
                {
                    if (idCode.Substring(0, 1) == country[index])
                    {
                        index += 10;//A是從10開始編碼,每個英文的碼都跟index差異10,先加回來
                        Esum = (((index % 10) * 9) + (index / 10));
                        //英文轉成的數字, 個位數(把數字/10取餘數)乘９再加上十位數
                        //加上十位數(數字/10,因為是int,後面會直接捨去)
                        break;
                    }
                }
                for (int i = 1; i < 9; i++)
                {//從第二個數字開始跑,每個數字*相對應權重
                    Nsum += (Convert.ToInt32(idCode[i].ToString())) * (9 - i);
                }
                count = 10 - ((Esum + Nsum) % 10);//把上述的總和加起來,取餘數後,10-該餘數為檢查碼,要等於最後一個數字
                if (count == Convert.ToInt32(idCode[9].ToString()))//判斷計算總和是不是等於檢查碼
                {
                    errorCode = 0;
                }
                else
                {
                    errorCode = 0012;
                }
            }
            else
            {
                errorCode = 0011;
            }

            return errorCode;
        }

        // 電話驗證
        public int isValidPhoneFormat(string phone)
        {
            int errorCode = 0;
            string pattern = @"^09[0-9]{8}$";
            bool flag = Regex.IsMatch(phone, pattern);
            if (!flag)
            {
                errorCode = 90015;
            }
            return errorCode;
        }

        // 訪問令牌時效是否失效
        private async Task<int> isTokenExp(string token)
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
            await connection.OpenAsync();
            SqlDataReader reader = await select.ExecuteReaderAsync();
            while (await reader.ReadAsync())
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

        //// 是否為已開通帳戶
        //public bool isCorrectPhoneFormat(string token)
        //{
        //    AccountDb accountDb = new AccountDb();
        //    accountDb._db = _db;
        //    AccountPojo accountPojo = accountDb.GetAccountInfo("", token);
        //    int errorCode = accountPojo.accountStatus == 0 ? 90013 : 0;
        //    return errorCode;
        //}


    }
}
