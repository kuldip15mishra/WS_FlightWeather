using System;
using System.Configuration;
using System.Linq;
using System.Text;

namespace FlightWeatherMailManager
{
    public static class MailText
    {
        private static readonly int AllowedDelayMin = Int32.Parse(ConfigurationManager.AppSettings["AllowedDelayMin"]);
        public const string SenderName = "MkMyTrip";
        public const string TriggerPoint = "Confirmation";
        public const string Source = "NAVISION";
        public const string ReferenceTypeSMS = "FlightWeatherStatus SMS";
        public const string From = "noreply@makemytrip.com";

        public static string EmailSubject(FlightWeatherResponse flightWeatherResponse)
        {
            string flightNo = flightWeatherResponse.ItineraryQueue.AirlineCode + "-" +
                              flightWeatherResponse.ItineraryQueue.FlightNo;
            string subject = String.Empty;
            string airlineName = flightWeatherResponse.ItineraryFlightStatus.CarrierName;
            string sectorName = flightWeatherResponse.ItineraryQueue.FromCity + "-" +
            flightWeatherResponse.ItineraryQueue.ToCity;
            switch (flightWeatherResponse.ItineraryQueue.Status)
            {
                case "S":
                    subject = String.Format("Upcoming Flight : {0} ({1}) Status",
                        flightNo,
                        sectorName);
                    break;
                case "L":
                    subject = String.Format("Upcoming Flight : {0} ({1}) Status",
                           flightNo,
                           sectorName);
                    break;
                case "C":
                    subject = String.Format("Upcoming Flight : {0} ({1}) Status",
                           flightNo,
                           sectorName);
                    break;
            }
            if (flightWeatherResponse.ItineraryQueue.Status != "C" && flightWeatherResponse.Forecast != null
               && flightWeatherResponse.Forecast.simpleforecast != null
                 && flightWeatherResponse.Forecast.simpleforecast.forecastday.Count > 0)
            {
                subject =
                    subject + string.Format(
                        " & Weather in {0}",
                        flightWeatherResponse.ItineraryFlightStatus.ArrCity);
            }
            return subject;
        }

        public static string GetSMSText(FlightWeatherResponse flightWeatherResponse)
        {
            string flightNo = flightWeatherResponse.ItineraryQueue.AirlineCode + "-" +
                              flightWeatherResponse.ItineraryQueue.FlightNo;
            DateTime depDate = flightWeatherResponse.ItineraryFlightStatus.EstimatedGateDeparture == DateTime.MinValue
            ? flightWeatherResponse.ItineraryFlightStatus.ScheduledGateDeparture : flightWeatherResponse.ItineraryFlightStatus.EstimatedGateDeparture;
            string airlineName = flightWeatherResponse.ItineraryFlightStatus.CarrierName;
            string sectorName = flightWeatherResponse.ItineraryQueue.FromCity + "-" +
            flightWeatherResponse.ItineraryQueue.ToCity;
            StringBuilder smsText = new StringBuilder();
            switch (flightWeatherResponse.ItineraryQueue.Status)
            {

                case "S":
                    if (flightWeatherResponse.ItineraryFlightStatus.DepartureGateDelayMin < AllowedDelayMin)
                    {
                        smsText = smsText.AppendFormat("Your flight {0} from {1} is On Time. Departure time: {2} hrs on {3}. ",
                            flightNo,
                            sectorName,
                            depDate.ToString("hh:mm tt"),
                            depDate.ToString("d"));
                    }
                    else
                    {
                        smsText = smsText.AppendFormat("Your flight {0} from {1} is delayed by {2} mins. New Departure time: {3} hrs on {4}. ",
                            flightNo,
                            sectorName,
                            flightWeatherResponse.ItineraryFlightStatus.DepartureGateDelayMin,
                            depDate.ToString("hh:mm tt"),
                            depDate.ToString("d"));
                    }
                    break;
                case "C":
                    return smsText.AppendFormat("Your flight {0} from {1} has been cancelled. Please contact airline for further enquiry.",
                            flightNo,
                            sectorName).ToString();
            }
            if (!String.IsNullOrWhiteSpace(flightWeatherResponse.ItineraryFlightStatus.DepTerminal))
            {
                smsText = smsText.AppendFormat("Departure Terminal: {0}. ",
                    flightWeatherResponse.ItineraryFlightStatus.DepTerminal);
            }
            if (flightWeatherResponse.Forecast != null
                && flightWeatherResponse.Forecast.simpleforecast != null
                && flightWeatherResponse.Forecast.simpleforecast.forecastday.Count > 0)
            {
                var currentDayForecast = flightWeatherResponse.Forecast.simpleforecast.forecastday.First();
                smsText =
                    smsText.AppendFormat(
                        "Weather in {0}- Min temp: {1}C, Max Temp: {2}C.",
                        flightWeatherResponse.ItineraryFlightStatus.ArrCity, currentDayForecast.low.celsius,
                        currentDayForecast.high.celsius);
            }
            return smsText.ToString();
        }
    }
}
