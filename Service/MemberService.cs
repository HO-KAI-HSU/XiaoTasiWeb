using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using xiaotasi.Models;

namespace xiaotasi.Service
{
    public interface MemberService
    {
        Task<MemberInfoModel> getMemberInfo(string memberCode, string token);

        Task<List<MemberReservationModel>> getMembrReservationList(string memberCode);

        Task<int> cancelMemberReservation(string travelReservationCode);
    }
}
