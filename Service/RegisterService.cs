using System;
using System.Threading.Tasks;

namespace xiaotasi.Service
{
    public interface RegisterService
    {
        Task resetPassword(string cellphone, string password);

        Task<int> isRegisterStatusByPhone(string cellphone, string verificationType);
    }
}
