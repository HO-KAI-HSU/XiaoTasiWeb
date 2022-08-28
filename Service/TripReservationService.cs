using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using xiaotasi.Bo;
using xiaotasi.Models;
using xiaotasi.Pojo;

namespace xiaotasi.Service
{
    public interface TripReservationService
    {
        Task<bool> addTravelReservationSeat(int travelStepId, int seatId);

        Task<int> checkReservationCheckIsUpdate(string reservationCode);

        Task<bool> updateReservationStatus(string reservationCode, int status);

        Task<bool> addReservationCheck(string memberCode, string travelReservationCode, string travelReservationCheckPicName, string bankAccountCode);

        Task addReservation(string memberCode, string reservationCode, int reservationNum, int reservationCost, string seatIds, int travelStepId, string note, int travelId);

        Task addReservationMemberInfo(MemberReservationArrBo memberReservationArrBo, int travelStepId, string memberCode, string orderCode);

        Task<string> getTripBoardingType(string travelCode);

        Task<TripStepTransportMatchModel> getTravelStepInfo(string travelStepCode);

        Task<List<TripReservationSeatMatchModel>> getTravelSeatList(string transportationId, int travelStepId);

        Task<List<TripBoardingMatchPojo>> getTripReservationBoardingList(string travelCode, int travelBoardingType);

        Task<List<TripBoardingMatchPojo>> getTripReservationCustomBoardingList(string travelCode);
    }
}
