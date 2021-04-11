using System; 
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;

namespace xiaotasi.Controllers
{
    public class MemberController : Controller
    {
        private readonly ILogger<MemberController> _logger;
        private readonly IConfiguration configuration;

        public MemberController(ILogger<MemberController> logger, IConfiguration config)
        {
            _logger = logger;
            configuration = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MemberInfo()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public ActionResult getMemberInfo(string memberCode)
        {
            //if ((memberCode == null || memberCode.Length == 0))
            //{
            //    return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            //}

            MemberInfoModel member = this._getMemberInfo(memberCode);
            //SqlDataReader reder = select.ExecuteReader();
            //if (member == null)
            //{
            //    return Json(new ApiError(0006, "No Such Staff!", "無此人員資料！"));
            //}
            //else
            //{
            //    memberData.memberCode = (string)reader[0];
            //    memberData.username = (string)reader[1];
            //    memberData.password = (string)reader[2];
            //    memberData.name = reader.IsDBNull(3) ? "" : reader[3].ToString();
            //    memberData.email = reader.IsDBNull(4) ? "" : reader[4].ToString();
            //    memberData.address = reader.IsDBNull(5) ? "" : reader[5].ToString();
            //    memberData.phone = reader.IsDBNull(6) ? "" : reader[6].ToString();
            //    memberData.status = (int)reader[7];
            //}
            //connection.Close();
            return Json(member);
        }


        public MemberInfoModel _getMemberInfo(string memberCode)
        {
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("select ml.member_code, al.username, al.password, ml.name, ml.email, ml.address, ml.telephone as phone, al.phone as cellphone, al.status, ml.birthday, ml.emer_contact_name as emername, ml.emer_contact_phone as emerphone from member_list AS ml JOIN account_list al ON ml.member_code = al.member_code WHERE ml.member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
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


        // 取得會員旅遊訂單列表
        [HttpPost]
        public ActionResult getMemberReservationList(string memberCode, int page, int limit)
        {

            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            string fieldSql = "select rl.reservation_code as travelReservationCode, tl.travel_code as travelCode, stl.travel_step_code as travelStepCode, rl.reservation_cost as cost, stl.travel_s_time as startDate, tl.travel_name as travelTraditionalTitle, rl.status as payStatus, stl.travel_s_time as travelStepSdate, stl.travel_e_time as travelStepEdate, rl.f_date as reservationFdate from reservation_list AS rl INNER JOIN travel_step_list stl ON stl.travel_step_id = rl.travel_step_id INNER JOIN travel_list tl ON tl.travel_id = stl.travel_id WHERE rl.member_code = @memberCode";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<MemberReservationModel> travelReservationInfoDatas = new List<MemberReservationModel>();
            PageControl<MemberReservationModel> pageControl = new PageControl<MemberReservationModel>();
            MemberReservationModel getTravelReservationListApi = new MemberReservationModel();
            String name = this._getMemberInfo(memberCode).name;
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
                travelReservationInfo.orderName = name;
                travelReservationInfo.memberCode = memberCode;
                travelReservationInfo.travelStepDate = reader.IsDBNull(7) == true || reader.IsDBNull(8) == true ? "" : ((DateTime)reader[7]).ToString(formatDate) + " ~ " + ((DateTime)reader[8]).ToString(formatDate);
                travelReservationInfo.travelReservationDate = reader.IsDBNull(9) ? "" : ((DateTime)reader[9]).ToString(formatDate); 
                travelReservationInfoDatas.Add(travelReservationInfo);
            }
            connection.Close();
            //List<MemberReservationModel> travelReservationInfoDatasNew = pageControl.pageControl(page, limit, travelReservationInfoDatas);
            //getTravelReservationListApi.success = 1;
            //getTravelReservationListApi.count = pageControl.size;
            //getTravelReservationListApi.page = page;
            //getTravelReservationListApi.limit = limit;
            //getTravelReservationListApi.travelReservationList = travelReservationInfoDatasNew;
            return Json(travelReservationInfoDatas);
        }


        [HttpPost]
        public ActionResult UpdateMemberInfo(string memberCode, string name, string email, string address, string phone, string birthday, string emerContactName, string emerContactPhone)
        {
            string connectionString = configuration.GetConnectionString("XiaoTasiTripContext");
            SqlConnection connection = new SqlConnection(connectionString);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE member_list SET name = @memberName, email = @email, address = @address, telephone = @phone, birthday = @birthday, emer_contact_name = @emername, emer_contact_phone = @emerphone WHERE member_Code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            select.Parameters.Add("@memberName", SqlDbType.NVarChar).Value = name;
            select.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
            select.Parameters.Add("@address", SqlDbType.NVarChar).Value = address;
            select.Parameters.Add("@phone", SqlDbType.NVarChar).Value = phone;
            select.Parameters.Add("@birthday", SqlDbType.DateTime).Value = DateTime.ParseExact(birthday, "yyyy-mm-dd", null);
            select.Parameters.Add("@emername", SqlDbType.NVarChar).Value = emerContactName;
            select.Parameters.Add("@emerphone", SqlDbType.NVarChar).Value = emerContactPhone;
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("更新成功"));
        }
    }
}
