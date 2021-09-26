using System;
namespace xiaotasi.Service
{
    public interface AuthService
    {
        int apiAuth(string token, int featureAccessLevel, int paramsAuthStatus);
    }
}
