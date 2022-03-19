using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Bo;
using xiaotasi.Models;
using xiaotasi.Pojo;

namespace xiaotasi.Service.Impl
{
    public class TripReservationServiceImpl : TripReservationService
    {
        private readonly IConfiguration _config;

        public TripReservationServiceImpl(IConfiguration config)
        {
            _config = config;
        }

        // 新增行程定位與座位綁定資訊
        public bool addTravelReservationSeat(int travelStepId, int seatId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("INSERT INTO seat_travel_match_list (seat_id, travel_step_id) " +
            "VALUES (@seatId, @travelStepId)", connection);
            select.Parameters.Add("@seatId", SqlDbType.Int).Value = seatId;
            select.Parameters.Add("@travelStepId", SqlDbType.Int).Value = travelStepId;
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        // 新增旅遊訂位匯款證明
        public bool addReservationCheck(string memberCode, string travelReservationCode, string travelReservationCheckPicName, string bankAccountCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("INSERT INTO reservation_check_list (reservation_code, member_code, bankbook_account_name, bankbook_account_code, reservation_check_pic_path, f_date, e_date) " +
            "VALUES (@reservationCode, @memberCode, @bankAccountName,  @bankAccountCode, '" + travelReservationCheckPicName + "', @fDate, @eDate)", connection);
            select.Parameters.Add("@memberCode", SqlDbType.NVarChar).Value = memberCode;
            select.Parameters.Add("@reservationCode", SqlDbType.NVarChar).Value = travelReservationCode;
            select.Parameters.Add("@bankAccountName", SqlDbType.NVarChar).Value = "";
            select.Parameters.Add("@bankAccountCode", SqlDbType.NVarChar).Value = bankAccountCode;
            select.Parameters.Add("@fDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            select.Parameters.Add("@eDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            ////開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        // 更新旅遊訂位狀態
        public bool updateReservationStatus(string reservationCode, int status)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE reservation_list SET status = @status,  e_date = @eDate WHERE reservation_code = @reservationCode", connection);
            select.Parameters.AddWithValue("@reservationCode", reservationCode);
            select.Parameters.Add("@status", SqlDbType.Int).Value = status;
            select.Parameters.Add("@eDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ////開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        // 旅遊訂位匯款是否更新
        public int checkReservationCheckIsUpdate(string reservationCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select reservation_code as reservationCode, member_code as memberCode, bankbook_account_name as bankbookAccountName, bankbook_account_code as bankbookAccountCode, status as checkStatus, f_date as fDate, e_date as eDate from reservation_check_list WHERE reservation_code = @reservationCode and status = @status ", connection);
            select.Parameters.AddWithValue("@reservationCode", reservationCode);
            select.Parameters.AddWithValue("@status", 2);

            int errorCode = 0;
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();

            while (reader.Read())
            {
                errorCode = 90051;
            }
            connection.Close();
            return errorCode;
        }

        // 新增旅遊預定資訊
        public void addReservation(string memberCode, string reservationCode, int reservationNum, int reservationCost, string seatIds, int travelStepId, string note, int travelId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("INSERT INTO reservation_list (reservation_code, reservation_num, reservation_cost, seat_ids, note, travel_id, travel_step_id, member_code) " +
            "VALUES ('" + reservationCode + "', " + reservationNum + ", " + reservationCost + ", '" + seatIds + "', '" + note + "', " + travelId + ", " + travelStepId + ", '" + memberCode + "')", connection);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
        }

        // 新增旅遊預定會員資訊
        public void addReservationMemberInfo(MemberReservationArrBo memberReservationArrBo, int travelStepId, string memberCode, string orderCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("INSERT INTO member_reservation_list (id, name, phone, birthday, meals_type, rooms_type, seat_id, boarding_id, transportation_id, travel_step_id, member_code, reservation_code, note) " +
            "VALUES ('" + memberReservationArrBo.id + "', @reservationName, '" + memberReservationArrBo.phone + "', '" + memberReservationArrBo.birthday + "', " + memberReservationArrBo.mealsType + ", " + memberReservationArrBo.roomsType + ", " + memberReservationArrBo.seatId + ", " + memberReservationArrBo.boardingId + ", " + memberReservationArrBo.transportationId + ", " + travelStepId + ", '" + memberCode + "', '" + orderCode + "', '" + memberReservationArrBo.seatId + "')", connection);
            select.Parameters.Add("@reservationName", SqlDbType.NVarChar).Value = memberReservationArrBo.name;
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
        }

        // 取得旅遊梯次乘車詳情
        public TripStepTransportMatchModel getTravelStepInfo(string travelStepCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "tsl.travel_step_id as travelStepId, tsl.travel_step_code as travelStepCode, tsl.travel_cost as travelCost, tsl.travel_num as travelNum, tsl.travel_s_time as travelStime, tsl.travel_e_time as travelEtime, tsl.travel_id as travelId, tsl.sell_seat_num as sellSeatNum, tsl.remain_seat_num as remainSeatNum, ttml.transportation_ids as transportationIds";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from travel_step_list as tsl inner join travel_transportation_match_list ttml ON  ttml.travel_step_id = tsl.travel_step_id WHERE tsl.travel_step_code = @travelStepCode and tsl.status = 1", connection);
            select.Parameters.AddWithValue("@travelStepCode", travelStepCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            TripStepTransportMatchModel travelStepTransportMatch = new TripStepTransportMatchModel();
            while (reader.Read())
            {
                travelStepTransportMatch.travelStepId = (int)reader[0];
                travelStepTransportMatch.travelStepCode = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelStepTransportMatch.travelCost = reader.IsDBNull(2) ? 0 : (int)reader[2];
                travelStepTransportMatch.travelNum = reader.IsDBNull(3) ? 0 : (int)reader[3];
                string format = "yyyy-MM-dd HH:mm:ss";
                travelStepTransportMatch.travelStime = ((DateTime)reader[4]).ToString(format);
                travelStepTransportMatch.travelEtime = ((DateTime)reader[5]).ToString(format);
                travelStepTransportMatch.travelId = reader.IsDBNull(6) ? 0 : (int)reader[6];
                travelStepTransportMatch.sellSeatNum = reader.IsDBNull(7) ? 0 : (int)reader[7];
                travelStepTransportMatch.remainSeatNum = reader.IsDBNull(8) ? 0 : (int)reader[8];
                travelStepTransportMatch.transportationIds = reader.IsDBNull(9) ? "" : (string)reader[9];
            }
            return travelStepTransportMatch;
        }

        // 取得旅遊梯次乘車詳情
        public List<TripReservationSeatMatchModel> getTravelSeatList(string transportationId, int travelStepId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "select sl.seat_id as seatId, spl.seat_pos_name as seatName, stml.status from seat_list AS sl INNER JOIN seat_pos_list spl ON spl.seat_pos = sl.seat_pos LEFT JOIN seat_travel_match_list stml ON stml.seat_id = sl.seat_id and stml.travel_step_id = @travelStepId and stml.status >= 0 WHERE sl.transportation_id = @transportationId order by sl.transportation_id ASC, sl.seat_pos ASC";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@transportationId", transportationId);
            select.Parameters.AddWithValue("@travelStepId", travelStepId);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<TripReservationSeatMatchModel> reservationSeatList = new List<TripReservationSeatMatchModel>();
            while (reader.Read())
            {
                TripReservationSeatMatchModel reservationSeatMatch = new TripReservationSeatMatchModel();
                reservationSeatMatch.seatId = (int)reader[0];
                reservationSeatMatch.seatName = reader.IsDBNull(1) ? "" : (string)reader[1];
                reservationSeatMatch.seatStatus = reader.IsDBNull(2) ? 1 : (int)reader[2];
                reservationSeatList.Add(reservationSeatMatch);
            }
            return reservationSeatList;
        }

        // 取得旅遊梯次乘車詳情
        public List<TripReservationSeatMatchModel> getTraveltransportationList(string transportationId)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "select sl.seat_id as seatId, spl.seat_pos_name as seatName, stml.status from seat_list AS sl INNER JOIN seat_pos_list spl ON spl.seat_pos = sl.seat_pos LEFT JOIN seat_travel_match_list stml ON stml.seat_id = sl.seat_id and stml.travel_step_id = @travelStepId WHERE sl.transportation_id = @transportationId order by sl.transportation_id ASC, sl.seat_pos ASC";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@transportationId", transportationId);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<TripReservationSeatMatchModel> reservationSeatList = new List<TripReservationSeatMatchModel>();
            while (reader.Read())
            {
                TripReservationSeatMatchModel reservationSeatMatch = new TripReservationSeatMatchModel();
                reservationSeatMatch.seatId = (int)reader[0];
                reservationSeatMatch.seatName = reader.IsDBNull(1) ? "" : (string)reader[1];
                reservationSeatMatch.seatStatus = reader.IsDBNull(2) ? 1 : (int)reader[2];
                reservationSeatList.Add(reservationSeatMatch);
            }
            return reservationSeatList;
        }

        // 取得旅遊梯次乘車詳情
        public List<TripBoardingMatchPojo> getTripReservationBoardingList(string travelCode)
        {
            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "bl.boarding_id as boardingId, bl.boarding_datetime as boardingTime, ll.location_name as locationName";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from boarding_list bl inner join location_list ll ON ll.location_id = bl.location_id", connection);
            select.Parameters.AddWithValue("@travelCode", travelCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<TripBoardingMatchPojo> tripBoardingMatchPojos = new List<TripBoardingMatchPojo>();
            while (reader.Read())
            {
                TripBoardingMatchPojo tripBoardingMatchPojo = new TripBoardingMatchPojo();
                tripBoardingMatchPojo.travelCode = "";
                tripBoardingMatchPojo.boardingId = reader.IsDBNull(0) ? 0 : (int)reader[0];
                string format = "HH:mm";
                tripBoardingMatchPojo.boardingTime = ((DateTime)reader[1]).ToString(format);
                tripBoardingMatchPojo.boardingLocationName = reader.IsDBNull(2) ? "" : (string)reader[2];
                tripBoardingMatchPojos.Add(tripBoardingMatchPojo);
            }
            return tripBoardingMatchPojos;
        }
    }
}
