﻿using System;
using System.Data;
using System.Threading.Tasks;
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

        public async Task resetPassword(string cellphone, string password)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE account_list SET password = @password WHERE phone = @cellphone and status = 1", connection);
            select.Parameters.AddWithValue("@cellphone", cellphone);
            select.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
            await connection.OpenAsync();
            await select.ExecuteNonQueryAsync();
            connection.Close();
        }

        public async Task<int> isRegisterStatusByPhone(string cellphone, string verificationType)
        {
            int errorCode = 0;
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            string username = "";
            SqlConnection connection = new SqlConnection(connectionString);
            // 取得帳號資訊
            SqlCommand getMemberSelect = new SqlCommand("select username from account_list WHERE phone = @cellphone and status = @status", connection);
            getMemberSelect.Parameters.AddWithValue("@cellphone", cellphone);
            getMemberSelect.Parameters.AddWithValue("@status", 1);
            await connection.OpenAsync();
            SqlDataReader getMemberReader = await getMemberSelect.ExecuteReaderAsync();
            while (await getMemberReader.ReadAsync())
            {
                username = getMemberReader.IsDBNull(0) ? "" : (string)getMemberReader[0];
            }
            if (verificationType == "1" && username != "")
            {
                errorCode = 90016;
            }
            else if (verificationType == "2" && username == "")
            {
                errorCode = 90017;
            }
            return errorCode;
        }
    }
}
