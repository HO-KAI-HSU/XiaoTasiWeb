using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace xiaotasi.Service.Impl
{
    public class RegisterServiceImpl : RegisterService
    {
        private readonly IConfiguration _config;

        public RegisterServiceImpl(IConfiguration config)
        {
            _config = config;
        }

        public void resetPassword(string cellphone, string password)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE account_list SET password = @password WHERE phone = @cellphone and status = 1", connection);
            select.Parameters.AddWithValue("@cellphone", cellphone);
            select.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
        }

        public int isRegisterStatusByPhone(string cellphone, string verificationType)
        {
            int errorCode = 0;
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // 取得帳號資訊
            SqlCommand getMemberSelect = new SqlCommand("select * from account_list WHERE phone = @cellphone and status = @status", connection);
            getMemberSelect.Parameters.AddWithValue("@cellphone", cellphone);
            getMemberSelect.Parameters.AddWithValue("@status", 1);
            connection.Open();
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            while (getMemberReader.Read())
            {
                if (verificationType == "1")
                {
                    if (!getMemberReader.IsDBNull(0))
                    {
                        errorCode = 90016;
                    }
                }
                else if (verificationType == "2")
                {
                    if (getMemberReader.IsDBNull(0))
                    {
                        errorCode = 90017;
                    }
                }
            }
            return errorCode;
        }
    }
}
