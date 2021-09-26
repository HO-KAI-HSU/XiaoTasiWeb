using System;
using System.Text;

namespace xiaotasi.Service.Impl
{
    public class LangServiceImpl : LangService
    {
        public LangServiceImpl()
        {
        }

        public int langMatch(string lang)
        {
            int num = 0;
            string clearLang = this.cleanStr(lang);
            string[] langMatchArr = new string[3] { "enus", "zhtw", "zhcn" };
            int count = 0;
            foreach (string langMatch in langMatchArr)
            {
                int index = langMatch.IndexOf(clearLang);
                if (index >= 0)
                {
                    num = count;
                }
                count += 1;
            }
            return num;
        }

        private string cleanStr(string s)
        {
            StringBuilder sb = new StringBuilder(s);

            sb.Replace("_", "");
            sb.Replace("#", "");
            sb.Replace("  ", "");
            sb.Replace("-", "");
            return sb.ToString().ToLower();
        }
    }
}
