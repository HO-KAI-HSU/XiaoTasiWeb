using System;
using System.Collections.Generic;
using xiaotasi.Models;

namespace xiaotasi.Service
{
    public interface MemberService
    {
        MemberInfoModel getMemberInfo(string memberCode, string token);

        List<MemberReservationModel> getMembrReservationList(string memberCode);

        int cancelMemberReservation(string travelReservationCode);
    }
}
