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

        public IActionResult TripReservation()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetTripReservationSeatList(string token, string travelCode, string travelStepCode)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((token == null || token.Length == 0))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = _apiResultService.apiAuth(token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }
            // 旅遊梯次詳情與交通綁定資訊
            TripStepTransportMatchModel travelStepTransportMatch = _tripReservationService.getTravelStepInfo(travelStepCode);
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
                int useStatus = 0;
                Console.WriteLine("{0}", transportationId);
                ReservationSeatModel reservationSeatData = new ReservationSeatModel();
                TransportationModel transportationListData = new TransportationModel();
                List<TripReservationSeatMatchModel> reservationSeatMatch = _tripReservationService.getTravelSeatList(transportationId, travelStepTransportMatch.travelStepId);
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
        public ActionResult GetTripReservationBoardingList(string token, string travelCode)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((token == null || token.Length == 0))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = _apiResultService.apiAuth(token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            // 旅遊梯次詳情與交通綁定資訊
            List<TripBoardingMatchPojo> tripBoardingMatchPojos = _tripReservationService.getTripReservationBoardingList(travelCode);

            // 整理回傳參數
            TravelReservationBoardingListVo travelReservationBoardingListVo = new TravelReservationBoardingListVo();
            travelReservationBoardingListVo.success = 1;
            travelReservationBoardingListVo.travelReservationBoardingList = tripBoardingMatchPojos;
            return Json(travelReservationBoardingListVo);
        }

        [HttpPost]
        public ActionResult CreateTravelReservation([FromBody]CreateTravelReservationBo createTravelReservationBo)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((createTravelReservationBo.token == null || createTravelReservationBo.token == "") || (createTravelReservationBo.travelStepCode == null || createTravelReservationBo.travelStepCode == ""))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = _apiResultService.apiAuth(createTravelReservationBo.token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }
            string travelStepCode = createTravelReservationBo.travelStepCode;
            string memberCode = _memberService.getMemberInfo("", createTravelReservationBo.token).memberCode;
            string seatIds = "";
            List<MemberReservationArrBo> memberReservationArrBo = createTravelReservationBo.memberReservationArr;
            TripStepTransportMatchModel travelStepTransportMatch = _tripReservationService.getTravelStepInfo(travelStepCode);
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
                _tripReservationService.addReservationMemberInfo(memberReservation, travelStepId, memberCode, orderCode);
                if (seatIds == "")
                {
                    seatIds = memberReservation.seatId + ",";
                }
                else
                {
                    seatIds = seatIds + memberReservation.seatId + ",";
                }

                // 新增行程定位與座位綁定資訊
                _tripReservationService.addTravelReservationSeat(travelStepId, memberReservation.seatId);
            }
            if (writeFlag)
            {
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 2, 90052, "");
                return Json(apiError);
            }

            // 新增旅遊預定資訊
            _tripReservationService.addReservation(memberCode, orderCode, reservationNum, reservationCost, seatIds, travelStepId, "", travelStepTransportMatch.travelId);

            ApiResult1<string> apiRes = _apiResultService.apiResult("zh-tw", 2, "TR001");
            return Json(apiRes);
        }

        // 新增旅遊訂位匯款資訊
        [HttpPost]
        public ActionResult addReservationCheck([FromBody]AddTravelReservationCheckBo addTravelReservationCheckBo)
        {
            int paramsAuthStatus = 0;

            // 必傳參數判斷
            if ((addTravelReservationCheckBo.token == null || addTravelReservationCheckBo.token == "") || (addTravelReservationCheckBo.travelReservationCode == null || addTravelReservationCheckBo.travelReservationCode == "") || (addTravelReservationCheckBo.travelReservationCheckPicPath == null || addTravelReservationCheckBo.travelReservationCheckPicPath == "") || (addTravelReservationCheckBo.bankAccountCode == null || addTravelReservationCheckBo.bankAccountCode == ""))
            {
                paramsAuthStatus = 1;
            }

            // 初始驗證
            ApiError1 apiAuth = _apiResultService.apiAuth(addTravelReservationCheckBo.token, "zh-tw", 2, 2, paramsAuthStatus);
            if (apiAuth.code > 0)
            {
                return Json(apiAuth);
            }

            string token = addTravelReservationCheckBo.token;
            string travelReservationCode = addTravelReservationCheckBo.travelReservationCode;
            string[] travelReservationCheckPicArr = addTravelReservationCheckBo.travelReservationCheckPicPath.Split('/');
            string travelReservationCheckPicName = travelReservationCheckPicArr[travelReservationCheckPicArr.Length - 1];
            string bankAccountCode = addTravelReservationCheckBo.bankAccountCode;
            string memberCode = _memberService.getMemberInfo("", token).memberCode;

            // 旅遊訂位匯款是否更新
            int errorCode = _tripReservationService.checkReservationCheckIsUpdate(travelReservationCode);
            if (errorCode > 0)
            {
                ApiError1 apiError = _apiResultService.apiAFailResult("zh-tw", 2, errorCode, "");
                return Json(apiError);
            }

            // 新增旅遊訂位匯款證明
            _tripReservationService.addReservationCheck(memberCode, travelReservationCode, travelReservationCheckPicName, bankAccountCode);

            //更新旅遊訂位狀態
            _tripReservationService.updateReservationStatus(travelReservationCode, 2);

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
