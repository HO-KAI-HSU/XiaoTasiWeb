using System;
namespace xiaotasi.Service
{
    public interface RegisterService
    {
        public void resetPassword(string cellphone, string password);

        public int isRegisterStatusByPhone(string cellphone, string verificationType);
    }
}
