using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;
using xiaotasi.Service;
using static xiaotasi.Vo.ApiResult;

namespace xiaotasi.Controllers
{
    public class MemberController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApiResultService _apiResultService;
        private readonly MemberService _memberService;

        public MemberController(ApiResultService apiResultService, MemberService memberService, IConfiguration config)
        {
            _apiResultService = apiResultService;
            _memberService = memberService;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MemberInfo()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> GetMemberInfo(string token)
        {

            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((token == null || token.Length == 0))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = await _apiResultService.apiAuth(token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            // 取得會員資訊
            MemberInfoModel member = await _memberService.getMemberInfo("", token);

            return Json(new ApiResult<MemberInfoModel>(member));
        }


        // 取得會員旅遊訂單列表
        [HttpPost]
        public async Task<ActionResult> getMemberReservationList(string token, int page, int limit)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((token == null || token.Length == 0))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = await _apiResultService.apiAuth(token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            // 取得會員資訊
            MemberInfoModel member = await _memberService.getMemberInfo("", token);
            string memberCode = member.memberCode;

            // 取得會員定位資訊
            List<MemberReservationModel> travelReservationInfoDatas = await _memberService.getMembrReservationList(memberCode);

            PageControl<MemberReservationModel> pageControl = new PageControl<MemberReservationModel>();
            //List<MemberReservationModel> travelReservationInfoDatasNew = pageControl.pageControl(page, limit, travelReservationInfoDatas);
            //getTravelReservationListApi.success = 1;
            //getTravelReservationListApi.count = pageControl.size;
            //getTravelReservationListApi.page = page;
            //getTravelReservationListApi.limit = limit;
            //getTravelReservationListApi.travelReservationList = travelReservationInfoDatasNew;
            return Json(new ApiResult<List<MemberReservationModel>>(travelReservationInfoDatas));
        }


        [HttpPost]
        public async Task<ActionResult> UpdateMemberInfo(string token, string name, string email, string address, string phone, string birthday, string emerContactName, string emerContactPhone)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((token == null || token.Length == 0))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = await _apiResultService.apiAuth(token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            // 取得會員資訊
            MemberInfoModel member = await _memberService.getMemberInfo("", token);
            string memberCode = member.memberCode;

            // 會員資料是否填寫完整
            if ((name == null || name.Length == 0) || (phone == null || phone.Length == 0) || (birthday == null || birthday.Length == 0))
            {
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 2, 90014, "");
                return Json(apiError);
            }

            string connectionString = _config.GetConnectionString("XiaoTasiTripContext");
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
            await connection.OpenAsync();
            select.ExecuteNonQuery();
            connection.Close();

            ApiResult1<string> apiRes = _apiResultService.apiResult("zh-tw", 2, "MA001");
            return Json(apiRes);
        }

        [HttpPost]
        public async Task<ActionResult> CancelMemberReservation(string token, string travelReservationCode)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((token == null || token.Length == 0) || (travelReservationCode == null || travelReservationCode.Length == 0))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = await _apiResultService.apiAuth(token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            int errorCode = await _memberService.cancelMemberReservation(travelReservationCode);

            if (errorCode > 0)
            {
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 2, errorCode, "");
                return Json(apiError);
            }

            ApiResult1<string> apiRes = _apiResultService.apiResult("zh-tw", 2, "TR003");
            return Json(apiRes);
        }
    }
}
