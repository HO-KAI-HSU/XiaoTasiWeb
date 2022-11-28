using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
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

        public async Task<MemberInfoModel> getMemberInfo(string memberCode, string token)
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
            await connection.OpenAsync();
            MemberInfoModel memberData = new MemberInfoModel();
            SqlDataReader reader = select.ExecuteReader();
            while (await reader.ReadAsync())
            {
                memberData.memberCode = (string)reader[0];
                memberData.username = (string)reader[1];
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

        public async Task<List<MemberReservationModel>> getMemberReservationList(string memberCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "select " +
                                "rl.reservation_code as travelReservationCode, " +
                                "tl.travel_code as travelCode, " +
                                "stl.travel_step_code as travelStepCode, " +
                                "rl.reservation_cost as cost, " +
                                "stl.travel_s_time as startDate, " +
                                "tl.travel_name as travelTraditionalTitle, " +
                                "rl.status as payStatus, " +
                                "stl.travel_s_time as travelStepSdate, " +
                                "stl.travel_e_time as travelStepEdate, " +
                                "rl.f_date as reservationFdate, " +
                                "tcl.transportation_info, " +
                                "tcl.eat_info as eatInfo, " +
                                "tcl.action_info as actionInfo, " +
                                "tcl.insurance_info as insuranceInfo, " +
                                "tcl.near_info as nearInfo, " +
                                "tcl.live_info as liveInfo " +
                                "from reservation_list AS rl INNER JOIN travel_step_list stl ON stl.travel_step_id = rl.travel_step_id INNER JOIN travel_list tl ON tl.travel_id = stl.travel_id INNER JOIN travel_cost_list tcl ON tcl.travel_id = tl.travel_id WHERE rl.member_code = @memberCode and rl.status > -1";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<MemberReservationModel> travelReservationInfoDatas = new List<MemberReservationModel>();
            while (reader.Read())
            {
                MemberReservationModel travelReservationInfo = new MemberReservationModel();
                travelReservationInfo.travelReservationCode = reader.IsDBNull(0) ? string.Empty : (string)reader[0];
                travelReservationInfo.travelCode = reader.IsDBNull(1) ? string.Empty : (string)reader[1];
                travelReservationInfo.travelStepCode = reader.IsDBNull(2) ? string.Empty : (string)reader[2];
                travelReservationInfo.cost = reader.IsDBNull(3) ? 0 : (int)reader[3];
                string format = "yyyy-MM-dd HH:mm:ss";
                string formatDate = "yyyy-MM-dd";
                travelReservationInfo.startDate = ((DateTime)reader[4]).ToString(format);
                travelReservationInfo.travelTraditionalTitle = reader.IsDBNull(5) ? string.Empty : (string)reader[5];
                travelReservationInfo.payStatus = reader.IsDBNull(6) ? 0 : (int)reader[6];
                travelReservationInfo.orderName = string.Empty;
                travelReservationInfo.memberCode = memberCode;
                travelReservationInfo.travelStepDate = reader.IsDBNull(7) == true || reader.IsDBNull(8) == true ? string.Empty : ((DateTime)reader[7]).ToString(formatDate) + " ~ " + ((DateTime)reader[8]).ToString(formatDate);
                travelReservationInfo.travelReservationDate = reader.IsDBNull(9) ? string.Empty : ((DateTime)reader[9]).ToString(formatDate);
                travelReservationInfo.costInfo = (reader.IsDBNull(10) ? string.Empty : " " + (string)reader[10]) + (reader.IsDBNull(11) ? string.Empty : " " + (string)reader[11]) + (reader.IsDBNull(12) ? string.Empty : " " + (string)reader[12]) + (reader.IsDBNull(13) ? string.Empty : " " + (string)reader[13]) + (reader.IsDBNull(14) ? string.Empty : " " + (string)reader[14]) + (reader.IsDBNull(15) ? string.Empty : " " + (string)reader[15]);
                travelReservationInfoDatas.Add(travelReservationInfo);
            }
            connection.Close();
            return travelReservationInfoDatas;
        }

        public async Task<List<ReservationSeatInfoModel>> getMemberReservationSeatList(string memberCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "select " +
                                "mrl.reservation_code as reservationCode, " +
                                "tsl.transportation_licenses_number + spl.seat_pos_name as memberSeatInfo " +
                                "from member_reservation_list AS mrl INNER JOIN seat_list sl ON sl.seat_id = mrl.seat_id INNER JOIN seat_pos_list spl ON spl.seat_pos = sl.seat_pos INNER JOIN transportation_list tsl ON tsl.transportation_id = sl.transportation_id WHERE mrl.member_code = @memberCode and mrl.status > 0";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<ReservationSeatInfoModel> objs = new List<ReservationSeatInfoModel>();
            while (reader.Read())
            {
                ReservationSeatInfoModel obj = new ReservationSeatInfoModel
                {
                    travelReservationCode = reader.IsDBNull(0) ? string.Empty : (string)reader[0],
                    seatInfo = reader.IsDBNull(1) ? string.Empty : (string)reader[1]
                };
                objs.Add(obj);
            }
            connection.Close();
            return objs;
        }

        // 取消訂單
        public async Task<int> cancelMemberReservation(string travelReservationCode)
        {
            string[] seatIdsInfo = await this.getMembrReservationSeatIdsInfo(travelReservationCode);
            string travelStepId = seatIdsInfo[0];
            string seatIds = seatIdsInfo[1];
            string status = seatIdsInfo[2];
            int errorCode = 0;

            if (status == "-1" || seatIds == "")
            {
                errorCode = 90053;
                return errorCode;
            }

            // 將字串轉為陣列
            string[] seatIdArr = seatIds == null ? new string[0] : seatIds.Split(',');

            List<Task> tasks = new List<Task>();

            // 取消座位
            foreach (var seatId in seatIdArr)
            {
                if (seatId == "")
                {
                    continue;
                }
                tasks.Add(this.cancelMembrReservationSeat(travelStepId, seatId));
            }
            await Task.WhenAll(tasks);

            // 取消訂單
            await this.cancelMembrReservation(travelReservationCode);

            return errorCode;
        }

        private async Task<string[]> getMembrReservationSeatIdsInfo(string travelReservationCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "select travel_step_id as travelStepId, seat_ids as seatIds, status from reservation_list  WHERE reservation_code = @travelReservationCode and status >= 0";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@travelReservationCode", travelReservationCode);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            string[] stepIdseatIdsInfo = new string[] { "", "", "-1" };
            while (reader.Read())
            {
                stepIdseatIdsInfo[0] = reader.IsDBNull(0) ? "" : reader.GetSqlInt32(0).ToString();
                Console.WriteLine("{0}", reader.GetSqlInt32(0).ToString());
                stepIdseatIdsInfo[1] = reader.IsDBNull(1) ? "" : (string)reader[1];
                Console.WriteLine("{0}", (string)reader[1]);
                stepIdseatIdsInfo[2] = reader.IsDBNull(2) ? "" : reader.GetSqlInt32(2).ToString();
                Console.WriteLine("{0}", reader.GetSqlInt32(2).ToString());
            }
            connection.Close();
            return stepIdseatIdsInfo;
        }

        private async Task cancelMembrReservationSeat(string travelStepId, string seatId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE seat_travel_match_list SET status = @status WHERE travel_step_id = @travelStepId and seat_id = @seatId and status >= 0", connection);
            select.Parameters.AddWithValue("@travelStepId", Convert.ToInt16(travelStepId));
            select.Parameters.AddWithValue("@seatId", Convert.ToInt16(seatId));
            select.Parameters.Add("@status", SqlDbType.Int).Value = -1;
            await connection.OpenAsync();
            select.ExecuteNonQuery();
            connection.Close();
        }

        private async Task cancelMembrReservation(string travelReservationCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE reservation_list SET status = @status WHERE reservation_code = @travelReservationCode and status >= 0", connection);
            select.Parameters.AddWithValue("@travelReservationCode", travelReservationCode);
            select.Parameters.Add("@status", SqlDbType.Int).Value = -1;
            await connection.OpenAsync();
            select.ExecuteNonQuery();
            connection.Close();
        }


    }
}
