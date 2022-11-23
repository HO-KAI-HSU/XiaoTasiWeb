using System.Collections.Generic;
using System.Threading.Tasks;
using xiaotasi.Models;
using xiaotasi.Pojo;

namespace xiaotasi.Service
{
    public interface TripService
    {
        Task<TripPojo> getTravelInfo(string travelCode);

        Task<List<TripStatisticListModel>> getTravelMonStatisticList(int travelId, string travelStepCode);
    }
}
