using System;

namespace xiaotasi.Models
{
    public class LoginModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string memberCode { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public int role { get; set; }
        public int modify { get; set; }
        public int status { get; set; }
        public int groupId { get; set; }
        public string token { get; set; }
        public string phone { get; set; }
    }
}
