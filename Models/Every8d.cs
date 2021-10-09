using System;
using System.Text;

namespace xiaotasi.Models
{
    public class Every8d
    {
        /*Every8d參數設定*/
        public string sendSMSUrl = "https://oms.every8d.com/API21/HTTP/sendSMS.ashx";
        public string uid = "0917620588";
        public string pwd = "George128458162";
        public string sb = "小蔡旅遊";

        HttpClientTest httpClientTest = new HttpClientTest();

        // 傳送簡訊
        public string sendSMS(string msgStr, string phone)
        {
            string sendSMSUrl = this.sendSMSUrl;
            string destEn = this.urlEncode(phone); // 電話:需要 urlEncode
            //byte[] msgEn = this.urlEncodeToUtf8(this.urlEncode(msgStr)); // 訊息:需要 urlEncode+UTF-8
            string msgEn = this.urlEncode("[" + this.sb + "]" + msgStr); // 訊息:需要 urlEncode+UTF-8
            string sbEn = this.urlEncode(this.sb); // 電話:需要 urlEncode
            string sendSMSUrlNew = sendSMSUrl + "?UID=" + this.uid + "&PWD=" + this.pwd + "&SB=" + sbEn + "&MSG=" + msgEn + "&DEST=" + destEn + "&ST=" + "";
            return httpClientTest.httpClientGet(sendSMSUrlNew);
        }

        // 訊息編碼
        public string urlEncode(string text)
        {
            return System.Web.HttpUtility.UrlEncode(text);
        }

        // 訊息編碼
        public byte[] urlEncodeToUtf8(string text)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] bytes = utf8.GetBytes(text);
            return bytes;
        }

    }
}
