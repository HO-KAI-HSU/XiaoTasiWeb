using System;

namespace xiaotasi.Models
{
    public class MemberInfoModel
    {
        public int id { get; set; }
        public string memberCode { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string birthday { get; set; }
        public int status { get; set; }
    }
}
