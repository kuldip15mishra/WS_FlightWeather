using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace WeatherStatus
{
    public class WUDataSource : IWeatherStatusDataSource
    {
        private readonly string _key = WebConfigurationManager.AppSettings["WUApiKey"];
        public string Key
        {
            get { return _key; }
        }

        public RootObject GetWeatherStatus(string airportCode)
        {
            string path = "http://api.wunderground.com/api/" + Key + "/forecast/q/" + airportCode + ".json";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        return JsonConvert.DeserializeObject<RootObject>(reader.ReadToEnd());
                    }
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    string errorText = reader.ReadToEnd();
                    throw new Exception(errorText);
                }
            }
            return null;
        }
    }
}
