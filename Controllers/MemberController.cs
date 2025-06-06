﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Models;
using xiaotasi.Service;
using static xiaotasi.Vo.ApiResult;

namespace xiaotasi.Controllers
{
    public class MemberController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApiResultService _apiResultService;
        private readonly AuthService _authService;
        private readonly MemberService _memberService;

        public MemberController(ApiResultService apiResultService, MemberService memberService, AuthService authService, IConfiguration config)
        {
            _apiResultService = apiResultService;
            _memberService = memberService;
            _authService = authService;
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
            if (string.IsNullOrWhiteSpace(token))
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
            if (string.IsNullOrWhiteSpace(token))
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
            var travelReservationInfoDatas = await _memberService.getMemberReservationList(memberCode);

            var objs = await _memberService.getMemberReservationSeatList(memberCode);


            var res = objs
                .GroupBy(x => x.travelReservationCode)
                .Select(x => new ReservationSeatInfoModel
                {
                    travelReservationCode = x.Key,
                    seatInfo = x.Select(x => x.seatInfo).Any() ? string.Join(",", x.Select(x => x.seatInfo).ToArray()) : string.Empty
                })
                .Join(travelReservationInfoDatas,
                    rsim => rsim.travelReservationCode,
                    tri => tri.travelReservationCode,
                (rsim, tri) => new { rsim, tri })
                .Select(x => new MemberReservationModel
                {
                    travelReservationCode = x.tri.travelReservationCode,
                    travelCode = x.tri.travelCode,
                    travelStepCode = x.tri.travelStepCode,
                    cost = x.tri.cost,
                    startDate = x.tri.startDate,
                    travelTraditionalTitle = x.tri.travelTraditionalTitle,
                    payStatus = x.tri.payStatus,
                    orderName = string.Empty,
                    memberCode = x.tri.memberCode,
                    travelStepDate = x.tri.travelStepDate,
                    travelReservationDate = x.tri.travelReservationDate,
                    costInfo = x.tri.costInfo,
                    seatInfo = x.rsim.seatInfo
                }).ToList();
            return Json(new ApiResult<List<MemberReservationModel>>(res));
        }


        [HttpPost]
        public async Task<ActionResult> UpdateMemberInfo(string token, string name, string email, string address, string phone, string birthday, string emerContactName, string emerContactPhone, string cellphone)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if (string.IsNullOrWhiteSpace(token))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = await _apiResultService.apiAuth(token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            int errorCode = _authService.isValidPhoneFormat(cellphone);
            if (errorCode > 0)
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "手機號碼格式有誤"));
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
            select.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
            select.Parameters.Add("@address", SqlDbType.NVarChar).Value = address;
            select.Parameters.Add("@phone", SqlDbType.VarChar).Value = phone;
            select.Parameters.Add("@birthday", SqlDbType.DateTime).Value = DateTime.ParseExact(birthday, "yyyy-mm-dd", null);
            select.Parameters.Add("@emername", SqlDbType.NVarChar).Value = emerContactName;
            select.Parameters.Add("@emerphone", SqlDbType.VarChar).Value = emerContactPhone;
            //開啟資料庫連線
            await connection.OpenAsync();
            await select.ExecuteNonQueryAsync();
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
