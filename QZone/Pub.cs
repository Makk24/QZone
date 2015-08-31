using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QZone
{
    public class Pub
    {
        public static string GetPage(string url)
        {
            string html = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "*/*";
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
                if (stream != null) reader = new StreamReader(stream, Encoding.GetEncoding("gb2312"));
                if (reader != null) html = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.Write("HttpWebResponse Error!");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return html;
        }
        public static string sKey(string Key)
        {
            int num = 5381;
            int length = Key.Length;
            for (int i = 0; i < length; i++)
            {
                char c = Convert.ToChar(Key.Substring(i, 1));
                num += (num << 5) + (int)c;
            }
            return Convert.ToString(num & 2147483647);
        }
        public static DateTime TimeStamp(string timeStamp)
        {
            DateTime dtStart2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow2 = new TimeSpan(lTime);
            DateTime dtResult = dtStart2.Add(toNow2);
            return dtResult;
        }
    }
}
