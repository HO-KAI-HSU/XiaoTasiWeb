using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using xiaotasi.Bo;
using xiaotasi.Models;
using xiaotasi.Pojo;
using xiaotasi.Service;
using xiaotasi.Vo;
using static xiaotasi.Vo.ApiResult;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace xiaotasi.Controllers
{
    public class TripReservationController : Controller
    {
        private readonly ApiResultService _apiResultService;
        private readonly MemberService _memberService;
        private readonly TripReservationService _tripReservationService;
        private readonly IConfiguration _config;

        public TripReservationController(IConfiguration config, MemberService memberService, ApiResultService apiResultService, TripReservationService tripReservationService)
        {
            _config = config;
            _memberService = memberService;
            _apiResultService = apiResultService;
            _tripReservationService = tripReservationService;
        }

        [Route("TripReservation/TripReservation/{traveType}/{traveCode}")]
        public IActionResult TripReservation(int traveType, string traveCode)
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> GetTripReservationSeatList(string token, string travelCode, string travelStepCode)
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
            // 旅遊梯次詳情與交通綁定資訊
            TripStepTransportMatchModel travelStepTransportMatch = await _tripReservationService.getTravelStepInfo(travelStepCode);
            GetReservationSeatListResponse getReservationSeatListApi = new GetReservationSeatListResponse();
            String transportationIds = travelStepTransportMatch.transportationIds;
            String[] transportationIdArr = transportationIds == null ? new String[0] : transportationIds.Split(',');
            // 取得陣列堆疊
            Stack<string> transportationIdArrStack = new Stack<string>(transportationIdArr);
            //// 後進先出，去掉最後一個值
            //transportationIdArrStack.Pop();
            String[] transportationIdArrNew = transportationIdArr.ToArray();
            List<ReservationSeatModel> reservationSeatList = new List<ReservationSeatModel>();
            List<TransportationModel> transportationList = new List<TransportationModel>();
            int transportationStep = 1;
            List<int> seatRetainCount = new List<int>();
            foreach (var transportationId in transportationIdArr)
            {
                if (string.IsNullOrEmpty(transportationId))
                {
                    continue;
                }
                int useStatus = 0;
                Console.WriteLine("{0}", transportationId);
                ReservationSeatModel reservationSeatData = new ReservationSeatModel();
                TransportationModel transportationListData = new TransportationModel();
                List<TripReservationSeatMatchModel> reservationSeatMatch = await _tripReservationService.getTravelSeatList(transportationId, travelStepTransportMatch.travelStepId);
                Console.WriteLine("{0}", travelStepTransportMatch.travelStepId);
                int retainSeat = 0;
                Console.WriteLine("{0}", retainSeat);
                foreach (TripReservationSeatMatchModel reservationSeatMatchTmp in reservationSeatMatch)
                {
                    if (reservationSeatMatchTmp.seatStatus.Equals(1))
                    {
                        retainSeat = retainSeat + 1;
                    }
                }
                seatRetainCount.Add(retainSeat);
                if ((transportationStep == 1 && retainSeat >= 0) || ((transportationStep - 2) >= 0 && seatRetainCount[transportationStep - 2] <= 5))
                {
                    useStatus = 1;
                }
                reservationSeatData.transportationId = Convert.ToInt16(transportationId);
                reservationSeatData.transportationStep = transportationStep;
                Console.WriteLine("{0}", transportationStep);
                reservationSeatData.reservationSeatList = reservationSeatMatch;
                reservationSeatData.useStatus = useStatus;
                transportationListData.transportationId = Convert.ToInt16(transportationId);
                transportationListData.transportationStep = transportationStep;
                transportationListData.remainSeatNum = retainSeat.ToString();
                transportationListData.transportationCode = "";
                reservationSeatList.Add(reservationSeatData);
                transportationList.Add(transportationListData);
                transportationStep++;
            }
            string format = "yyyy-MM-dd";
            string startDate = "";
            if (travelStepTransportMatch.travelStime != null)
            {
                DateTime startDateTime = DateTime.Parse(travelStepTransportMatch.travelStime);
                startDate = startDateTime.ToString(format);
            }
            getReservationSeatListApi.success = 1;
            getReservationSeatListApi.travelStep = travelStepCode;
            getReservationSeatListApi.startDate = startDate;
            getReservationSeatListApi.travelCode = travelCode;
            getReservationSeatListApi.reservationSeatList = reservationSeatList;
            getReservationSeatListApi.transportationList = transportationList;
            return Json(getReservationSeatListApi);
        }

        [HttpPost]
        public async Task<ActionResult> GetTripReservationBoardingList(string token, string travelCode)
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

            // 旅遊梯次詳情與交通綁定資訊

            string boardingType = await _tripReservationService.getTripBoardingType(travelCode);
            List<TripBoardingMatchPojo> tripBoardingMatchPojos;

            if (boardingType == "2")
            {
                tripBoardingMatchPojos = await _tripReservationService.getTripReservationCustomBoardingList(travelCode);
            }
            else if (boardingType == "1")
            {
                tripBoardingMatchPojos = await _tripReservationService.getTripReservationBoardingList(travelCode, 1);
            }
            else
            {
                tripBoardingMatchPojos = await _tripReservationService.getTripReservationBoardingList(travelCode, 0);
            }

            // 整理回傳參數
            TravelReservationBoardingListVo travelReservationBoardingListVo = new TravelReservationBoardingListVo();
            travelReservationBoardingListVo.success = 1;
            travelReservationBoardingListVo.travelReservationBoardingList = tripBoardingMatchPojos;
            return Json(travelReservationBoardingListVo);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTravelReservation([FromBody]CreateTravelReservationBo createTravelReservationBo)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((createTravelReservationBo.token == null || createTravelReservationBo.token == "") || (createTravelReservationBo.travelStepCode == null || createTravelReservationBo.travelStepCode == ""))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = await _apiResultService.apiAuth(createTravelReservationBo.token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }
            string travelStepCode = createTravelReservationBo.travelStepCode;
            var memberInfo = await _memberService.getMemberInfo("", createTravelReservationBo.token);
            var memberCode = memberInfo.memberCode;
            string seatIds = "";
            List<MemberReservationArrBo> memberReservationArrBo = createTravelReservationBo.memberReservationArr;
            TripStepTransportMatchModel travelStepTransportMatch = await _tripReservationService.getTravelStepInfo(travelStepCode);
            int reservationNum = memberReservationArrBo.Count();
            int reservationCost = memberReservationArrBo.Count() * travelStepTransportMatch.travelCost;
            int travelStepId = travelStepTransportMatch.travelStepId;
            string orderCode = this.createOrderNumber();
            bool writeFlag = false;
            foreach (MemberReservationArrBo memberReservation in memberReservationArrBo)
            {


                if ((memberReservation.name == null || memberReservation.name == "") || (memberReservation.id == null || memberReservation.id == "") || (memberReservation.birthday == null || memberReservation.birthday == "") || (memberReservation.phone == null || memberReservation.phone == "") || memberReservation.mealsType == 0)
                {
                    writeFlag = true;
                }
                // 新增旅遊預定會員資訊
                await _tripReservationService.addReservationMemberInfo(memberReservation, travelStepId, memberCode, orderCode);
                if (seatIds == "")
                {
                    seatIds = memberReservation.seatId + ",";
                }
                else
                {
                    seatIds = seatIds + memberReservation.seatId + ",";
                }

                // 新增行程定位與座位綁定資訊
                await _tripReservationService.addTravelReservationSeat(travelStepId, memberReservation.seatId);
            }
            if (writeFlag)
            {
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 2, 90052, "");
                return Json(apiError);
            }

            // 新增旅遊預定資訊
            await _tripReservationService.addReservation(memberCode, orderCode, reservationNum, reservationCost, seatIds, travelStepId, "", travelStepTransportMatch.travelId);

            ApiResult1<string> apiRes = _apiResultService.apiResult("zh-tw", 2, "TR001");
            return Json(apiRes);
        }

        // 新增旅遊訂位匯款資訊
        [HttpPost]
        public async Task<ActionResult> addReservationCheck([FromBody]AddTravelReservationCheckBo addTravelReservationCheckBo)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((addTravelReservationCheckBo.token == null || addTravelReservationCheckBo.token == "") || (addTravelReservationCheckBo.travelReservationCode == null || addTravelReservationCheckBo.travelReservationCode == "") || (addTravelReservationCheckBo.travelReservationCheckPicPath == null || addTravelReservationCheckBo.travelReservationCheckPicPath == "") || (addTravelReservationCheckBo.bankAccountCode == null || addTravelReservationCheckBo.bankAccountCode == ""))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = await _apiResultService.apiAuth(addTravelReservationCheckBo.token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            string token = addTravelReservationCheckBo.token;
            string travelReservationCode = addTravelReservationCheckBo.travelReservationCode;
            //string[] travelReservationCheckPicArr = addTravelReservationCheckBo.travelReservationCheckPicPath.Split('/');
            //string travelReservationCheckPicName = travelReservationCheckPicArr[travelReservationCheckPicArr.Length - 1];
            string travelReservationCheckPicName = addTravelReservationCheckBo.travelReservationCheckPicPath;
            string bankAccountCode = addTravelReservationCheckBo.bankAccountCode;
            var memberInfo = await _memberService.getMemberInfo("", token);
            var memberCode = memberInfo.memberCode;

            // 旅遊訂位匯款是否更新
            int errorCode = await _tripReservationService.checkReservationCheckIsUpdate(travelReservationCode);
            if (errorCode > 0)
            {
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 2, errorCode, "");
                return Json(apiError);
            }

            // 新增旅遊訂位匯款證明
            await _tripReservationService.addReservationCheck(memberCode, travelReservationCode, travelReservationCheckPicName, bankAccountCode);
            Console.WriteLine("addReservationCheckSuccess");

            //更新旅遊訂位狀態
            await _tripReservationService.updateReservationStatus(travelReservationCode, 2);
            Console.WriteLine("updateReservationStatusSuccess");

            ApiResult1<string> apiRes = _apiResultService.apiResult("zh-tw", 2, "TR002");
            return Json(apiRes);
        }

        //產生訂單編號
        private string createOrderNumber()
        {
            string n = DateTime.Now.ToString("yyyyMMddHHmmss");
            return "OR" + n;
        }
    }
}
