using System;
namespace xiaotasi.Vo
{
    public class ApiResult
    {
        public class ApiResult1<T>
        {
            /// <summary>
            /// 執行成功與否
            /// </summary>
            public int success { get; set; }
            /// <summary>
            /// 結果代碼(0=成功，其餘為錯誤代號)
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 英文訊息
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 中文訊息
            /// </summary>
            public string reason { get; set; }


            public ApiResult1() { }

            /// <summary>
            /// 建立成功結果(更新，上傳)
            /// </summary>
            /// <param name="data"></param>
            public ApiResult1(string msg, string reas)
            {
                success = 1;
                message = msg;
                reason = reas;
            }
        }

        public class ApiError1 : ApiResult1<object>
        {
            /// <summary>
            /// 建立失敗結果
            /// </summary>
            /// <param name="code"></param>
            /// <param name="success"></param>
            /// <param name="message"></param>
            /// <param name="reason"></param>
            public ApiError1(int codes, string msg, string reas)
            {
                code = codes;
                success = 0;
                message = msg;
                reason = reas;
            }
        }
    }
}
