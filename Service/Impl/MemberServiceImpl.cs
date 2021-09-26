using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Models;

namespace xiaotasi.Service.Impl
{
    public class MemberServiceImpl : MemberService
    {
        private readonly IConfiguration _config;

        public MemberServiceImpl(IConfiguration config)
        {
            _config = config;
        }

        public MemberInfoModel getMemberInfo(string memberCode, string token)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand select;
            // SQL Command
            if (memberCode != null && memberCode != "")
            {
                select = new SqlCommand("select ml.member_code, al.username, al.password, ml.name, ml.email, ml.address, ml.telephone as phone, al.phone as cellphone, al.status, ml.birthday, ml.emer_contact_name as emername, ml.emer_contact_phone as emerphone from member_list AS ml JOIN account_list al ON ml.member_code = al.member_code WHERE ml.member_code = @memberCode", connection);
                select.Parameters.AddWithValue("@memberCode", memberCode);
            }
            else
            {
                select = new SqlCommand("select ml.member_code, al.username, al.password, ml.name, ml.email, ml.address, ml.telephone as phone, al.phone as cellphone, al.status, ml.birthday, ml.emer_contact_name as emername, ml.emer_contact_phone as emerphone from member_list AS ml JOIN account_list al ON ml.member_code = al.member_code WHERE al.token = @token", connection);
                select.Parameters.AddWithValue("@token", token);
            }
            // 開啟資料庫連線
            connection.Open();
            MemberInfoModel memberData = new MemberInfoModel();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                memberData.memberCode = (string)reader[0];
                memberData.username = (string)reader[1];
                memberData.password = (string)reader[2];
                memberData.name = reader.IsDBNull(3) ? "" : reader[3].ToString();
                memberData.email = reader.IsDBNull(4) ? "" : reader[4].ToString();
                memberData.address = reader.IsDBNull(5) ? "" : reader[5].ToString();
                memberData.phone = reader.IsDBNull(6) ? "" : reader[6].ToString();
                memberData.cellphone = reader.IsDBNull(7) ? "" : reader[7].ToString();
                memberData.status = (int)reader[8];
                string formatDate = "yyyy-MM-dd";
                memberData.birthday = reader.IsDBNull(9) ? "" : ((DateTime)reader[9]).ToString(formatDate);
                memberData.emerContactName = reader.IsDBNull(10) ? "" : reader[10].ToString();
                memberData.emerContactPhone = reader.IsDBNull(11) ? "" : reader[11].ToString();
            }
            connection.Close();
            return memberData;
        }

        public List<MemberReservationModel> getMembrReservationList(string memberCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "select rl.reservation_code as travelReservationCode, tl.travel_code as travelCode, stl.travel_step_code as travelStepCode, rl.reservation_cost as cost, stl.travel_s_time as startDate, tl.travel_name as travelTraditionalTitle, rl.status as payStatus, stl.travel_s_time as travelStepSdate, stl.travel_e_time as travelStepEdate, rl.f_date as reservationFdate from reservation_list AS rl INNER JOIN travel_step_list stl ON stl.travel_step_id = rl.travel_step_id INNER JOIN travel_list tl ON tl.travel_id = stl.travel_id  WHERE rl.member_code = @memberCode";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<MemberReservationModel> travelReservationInfoDatas = new List<MemberReservationModel>();
            while (reader.Read())
            {
                MemberReservationModel travelReservationInfo = new MemberReservationModel();
                travelReservationInfo.travelReservationCode = reader.IsDBNull(0) ? "" : (string)reader[0];
                travelReservationInfo.travelCode = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelReservationInfo.travelStepCode = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelReservationInfo.cost = reader.IsDBNull(3) ? 0 : (int)reader[3];
                string format = "yyyy-MM-dd HH:mm:ss";
                string formatDate = "yyyy-MM-dd";
                travelReservationInfo.startDate = ((DateTime)reader[4]).ToString(format);
                travelReservationInfo.travelTraditionalTitle = reader.IsDBNull(5) ? "" : (string)reader[5];
                travelReservationInfo.payStatus = reader.IsDBNull(6) ? 0 : (int)reader[6];
                travelReservationInfo.orderName = "";
                travelReservationInfo.memberCode = memberCode;
                travelReservationInfo.travelStepDate = reader.IsDBNull(7) == true || reader.IsDBNull(8) == true ? "" : ((DateTime)reader[7]).ToString(formatDate) + " ~ " + ((DateTime)reader[8]).ToString(formatDate);
                travelReservationInfo.travelReservationDate = reader.IsDBNull(9) ? "" : ((DateTime)reader[9]).ToString(formatDate);
                travelReservationInfoDatas.Add(travelReservationInfo);
            }
            connection.Close();
            return travelReservationInfoDatas;
        }
    }
}
