using System;
using System.Threading.Tasks;

namespace xiaotasi.Service
{
    public interface AuthService
    {
        Task<int> apiAuth(string token, int featureAccessLevel, int paramsAuthStatus);

        int isValidIDorRCNumber(string id);

        int isValidPhoneFormat(string phone);
    }
}
