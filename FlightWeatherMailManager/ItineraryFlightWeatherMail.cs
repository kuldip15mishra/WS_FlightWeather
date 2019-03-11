using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Library.ErrorLog;
using WeatherStatus;

namespace FlightWeatherMailManager
{
    public class ItineraryFlightWeatherMail
    {
        public List<bool[]> Send(List<ItineraryQueue> itineraryQueues, ItineraryFlightStatus itineraryFlightStatus)
        {
            try
            {
                Forecast forecast = null;
                var isAnyItemNotTriggered = itineraryQueues.Any(i => i.IsTriggered == false);
                if (isAnyItemNotTriggered && itineraryFlightStatus.FlightStatus != "C")
                {
                    ItineraryWeatherStatusData itineraryWeatherStatusData = new ItineraryWeatherStatusData(new WUDataSource());
                    forecast = itineraryWeatherStatusData.GetItineraryWeatherStatus(itineraryFlightStatus.ArrivalAirportCode, DateTime.Now);
                }

                ItineraryQueue item = itineraryQueues.FirstOrDefault();
                bool[] isEmailSent = item != null && !item.IsTriggered ? SendEmail(itineraryQueues, itineraryFlightStatus, forecast) : new bool[itineraryQueues.Count];
                var isSMSSent = SendSMS(itineraryQueues, itineraryFlightStatus, forecast);

                return new List<bool[]> { isEmailSent, isSMSSent };
            }
            catch (Exception exception)
            {
                foreach (var itineraryQueue in itineraryQueues)
                {
                    ErrorLog.WriteErrorLog(exception, itineraryQueue.BookingID, "MMT_WS_FlightWeather");
                }
                return new List<bool[]> { new bool[itineraryQueues.Count], new bool[itineraryQueues.Count] };
            }
        }

        private static bool[] SendSMS(IEnumerable<ItineraryQueue> itineraryQueues, ItineraryFlightStatus itineraryFlightStatus, Forecast forecast)
        {
            EFMailManager smsManager = new EFMailManager();
            List<FlightWeatherResponse> responses = new List<FlightWeatherResponse>();
            foreach (var itineraryQueue in itineraryQueues)
            {
                FlightWeatherResponse response = new FlightWeatherResponse
                {
                    ItineraryFlightStatus = itineraryFlightStatus,
                    ItineraryQueue = itineraryQueue
                };
                if (!itineraryQueue.IsTriggered)
                {
                    response.Forecast = forecast;
                }
                responses.Add(response);
            }
            bool[] isSMSSent = smsManager.SendSMS(responses);
            return isSMSSent;
        }

        private static bool[] SendEmail(IReadOnlyCollection<ItineraryQueue> itineraryQueues, ItineraryFlightStatus itineraryFlightStatus, Forecast forecast)
        {
            bool[] isEmailSent = new bool[itineraryQueues.Count];
            InteractionMailManager mailManager = new InteractionMailManager();

            int i = 0;
            foreach (FlightWeatherResponse response in itineraryQueues.Select(itineraryQueue => new FlightWeatherResponse
            {
                ItineraryFlightStatus = itineraryFlightStatus,
                ItineraryQueue = itineraryQueue,
                Forecast = forecast
            }))
            {
                bool emailSent;
                try
                {
                    emailSent = mailManager.SendMail(response);
                }
                catch (Exception exception)
                {
                    emailSent = false;
                    ErrorLog.WriteErrorLog(exception, response.ItineraryQueue.BookingID, "MMT_WS_FlightWeather");
                }
                isEmailSent[i++] = emailSent;
            }
            return isEmailSent;
        }
    }
}
