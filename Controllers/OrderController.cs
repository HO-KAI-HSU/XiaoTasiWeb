using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace xiaotasi.Controllers
{
    public class OrderController : Controller
    {
        //// 建立訂單
        //public Dictionary<string, string> createOrder(string orderCode, int orderNum, int payAmt)
        //{
        //    SqlConnection connection = new SqlConnection("Server = localhost; User ID = sa; Password = reallyStrongPwd123; Database = tasiTravel");
        //    // SQL Command
        //    // member_code 會員唯一編碼
        //    string memberCode = "mem0000001";
        //    string tradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//宣告一個目前的時間
        //    string sqlCommand = "INSERT INTO order_list (order_code, member_code, reservation_num, trade_amt, trade_date) VALUES ";
        //    sqlCommand = sqlCommand + " (" + "'" + orderCode + "', '" + memberCode + "', " + orderNum + ", " + payAmt + ",'" + tradeDate + "')";
        //    System.Diagnostics.Debug.Print(sqlCommand);
        //    SqlCommand select = new SqlCommand(sqlCommand, connection);

        //    //開啟資料庫連線
        //    connection.Open();
        //    select.ExecuteNonQuery();
        //    connection.Close();
        //    EcPay ecpay = new EcPay();
        //    Dictionary<string, string> keyValuePairs = ecpay.sendEcPay(orderCode, "ALL");
        //    return keyValuePairs;
        //}
    }
}
