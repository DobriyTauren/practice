using System.Net;

namespace practice
{
    public class RequestSender
    {
        public static string Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetRatesByDate(DateTime date)
        {
            string dateUTC = date.ToString("yyyy-MM-dd");

            return Get($"https://api.nbrb.by/exrates/rates?ondate={dateUTC}&periodicity=0");
        }
    }
}
