using System;
using System.Collections.Generic;
using xiaotasi.Bo;
using xiaotasi.Models;
using xiaotasi.Pojo;

namespace xiaotasi.Service
{
    public interface TripReservationService
    {
        bool addTravelReservationSeat(int travelStepId, int seatId);

        int checkReservationCheckIsUpdate(string reservationCode);

        bool updateReservationStatus(string reservationCode, int status);

        bool addReservationCheck(string memberCode, string travelReservationCode, string travelReservationCheckPicName, string bankAccountCode);

        void addReservation(string memberCode, string reservationCode, int reservationNum, int reservationCost, string seatIds, int travelStepId, string note, int travelId);

        void addReservationMemberInfo(MemberReservationArrBo memberReservationArrBo, int travelStepId, string memberCode, string orderCode);

        TripStepTransportMatchModel getTravelStepInfo(string travelStepCode);

        List<TripReservationSeatMatchModel> getTravelSeatList(string transportationId, int travelStepId);

        List<TripBoardingMatchPojo> getTripReservationBoardingList(string travelCode);
    }
}
