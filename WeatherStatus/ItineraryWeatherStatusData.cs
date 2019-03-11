using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;
using Helper;
using Library.ErrorLog;
using Newtonsoft.Json;

namespace WeatherStatus
{
    public class ItineraryWeatherStatusData
    {
        private readonly int _staleTime = int.Parse(WebConfigurationManager.AppSettings["WeatherStatusStaleTimeInMin"]);
        private readonly IWeatherStatusDataSource _weatherStatusDataSource;
        private string _airportCode = string.Empty;
        private DateTime _date;

        public ItineraryWeatherStatusData(IWeatherStatusDataSource source)
        {
            _weatherStatusDataSource = source;
        }

        public Forecast GetItineraryWeatherStatus(string airportCode, DateTime date)
        {
            _airportCode = airportCode;
            _date = date;
            return CommonCache.GetOrInsertIntoCache(GetWeatherStatusFromDB, airportCode + date.ToString("ddd"));
        }

        private Forecast GetWeatherStatusFromDB()
        {
            using (MMTSqlLiveEntities entities = new MMTSqlLiveEntities(StaticHelperValues.MMTSqlEntitiesConnectionString))
            {
                DateTime lastModifieDateTime = DateTime.Now.AddMinutes(-_staleTime);
                var status =
                    entities.tbl_WeatherStatusLog.Where(
                        i =>
                            i.AirPortCode == _airportCode && i.WeatherStatusDate >=
                                _date.Date && i.LastModifiedDate > lastModifieDateTime).Take(3);
                if (status.Count() < 3)
                {
                    return UpdateWeatherStatusInDB(_airportCode, _date);
                }
                Forecast forecast = new Forecast();
                TxtForecast txtForecast = new TxtForecast();
                List<Forecastday> forecastdaysEnumerable = new List<Forecastday>();
                Simpleforecast simpleforecast = new Simpleforecast();
                List<Forecastday2> forecastdays = new List<Forecastday2>();
                foreach (var weatherStatusLog in status)
                {
                    forecastdaysEnumerable.Add(JsonConvert.DeserializeObject<Forecastday>(weatherStatusLog.MorningStatus));
                    forecastdaysEnumerable.Add(JsonConvert.DeserializeObject<Forecastday>(weatherStatusLog.EveningStatus));
                    forecastdays.Add(JsonConvert.DeserializeObject<Forecastday2>(weatherStatusLog.CompleteDayStatus));
                }
                txtForecast.forecastday = forecastdaysEnumerable;
                simpleforecast.forecastday = forecastdays;
                forecast.txt_forecast = txtForecast;
                forecast.simpleforecast = simpleforecast;
                return forecast;
            }
        }

        private Forecast UpdateWeatherStatusInDB(string airportCode, DateTime date)
        {
            RootObject obj = _weatherStatusDataSource.GetWeatherStatus(airportCode);
            if (obj == null || obj.forecast == null) return null;
            UpdateWeatherStatusInDB(airportCode, obj.forecast);
            int days = (date - DateTime.Now).Days;
            if (days < 0)
            {
                throw new Exception("date should be greater than equal to current date");
            }
            return obj.forecast;

        }

        private void UpdateWeatherStatusInDB(string airportCode, Forecast forecast)
        {
            var forecastdaysEnumerable = forecast.txt_forecast.forecastday.GetEnumerator();
            var simpleForecastdaysEnumerable = forecast.simpleforecast.forecastday.GetEnumerator();
            using (MMTSqlLiveEntities entities = new MMTSqlLiveEntities(StaticHelperValues.MMTSqlEntitiesConnectionString))
            {
                DateTime lastModifieDateTime = DateTime.Now.AddMinutes(-_staleTime);
                var date = DateTime.Now.Date;
                var status =
                    entities.tbl_WeatherStatusLog.Where(
                        i =>
                            i.AirPortCode == airportCode && i.WeatherStatusDate >= date
                            && i.LastModifiedDate > lastModifieDateTime)
                        .OrderBy(i => i.WeatherStatusDate);
                UpdateWeatherStatusLog(status, forecastdaysEnumerable, simpleForecastdaysEnumerable);
                AddToWeatherStatusLog(airportCode, status, forecastdaysEnumerable, simpleForecastdaysEnumerable, entities);
                try
                {
                    entities.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (
                        DbValidationError error in
                            dbEx.EntityValidationErrors.SelectMany(entityErr => entityErr.ValidationErrors))
                    {
                        string errorMessage = string.Format("Error Property Name {0} : Error Message: {1}",
                            error.PropertyName, error.ErrorMessage);
                        ErrorLog.WriteErrorLog(errorMessage, "", "MMT_WS_FlightWeather");
                       
                    }
                }
            }
        }

        private void AddToWeatherStatusLog(string airportCode, IOrderedQueryable<tbl_WeatherStatusLog> status,
            List<Forecastday>.Enumerator forecastdaysEnumerable, List<Forecastday2>.Enumerator simpleForecastdaysEnumerable, MMTSqlLiveEntities entities)
        {
            for (int i = 0; i < 3 - status.Count(); i++)
            {
                if (!forecastdaysEnumerable.MoveNext())
                {
                    break;
                }
                tbl_WeatherStatusLog log = new tbl_WeatherStatusLog
                {
                    AirPortCode = airportCode,
                    LastModifiedDate = DateTime.Now,
                    WeatherStatusDate = DateTime.Now.AddDays(i + status.Count()).Date,
                    MorningStatus = JsonConvert.SerializeObject(forecastdaysEnumerable.Current)
                };

                if (!forecastdaysEnumerable.MoveNext())
                {
                    break;
                }
                log.EveningStatus = JsonConvert.SerializeObject(forecastdaysEnumerable.Current);
                if (!simpleForecastdaysEnumerable.MoveNext())
                {
                    break;
                }
                log.CompleteDayStatus = JsonConvert.SerializeObject(simpleForecastdaysEnumerable.Current);
                entities.tbl_WeatherStatusLog.Add(log);
            }
        }

        private void UpdateWeatherStatusLog(IEnumerable<tbl_WeatherStatusLog> status, List<Forecastday>.Enumerator forecastdaysEnumerable,
            List<Forecastday2>.Enumerator simpleForecastdaysEnumerable)
        {
            foreach (tbl_WeatherStatusLog weatherStatusLog in status)
            {
                weatherStatusLog.LastModifiedDate = DateTime.Now;
                if (!forecastdaysEnumerable.MoveNext())
                {
                    break;
                }
                weatherStatusLog.MorningStatus = JsonConvert.SerializeObject(forecastdaysEnumerable.Current);
                if (!forecastdaysEnumerable.MoveNext())
                {
                    break;
                }
                weatherStatusLog.EveningStatus = JsonConvert.SerializeObject(forecastdaysEnumerable.Current);
                if (!simpleForecastdaysEnumerable.MoveNext())
                {
                    break;
                }
                weatherStatusLog.CompleteDayStatus = JsonConvert.SerializeObject(simpleForecastdaysEnumerable.Current);

            }
        }
    }
}
