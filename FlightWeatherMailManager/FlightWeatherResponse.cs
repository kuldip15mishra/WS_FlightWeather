using System.Collections.Generic;
using Common;
using WeatherStatus;

namespace FlightWeatherMailManager
{
    
    public class FlightWeatherResponse
    {
        public ItineraryQueue ItineraryQueue { get; set; }
        public ItineraryFlightStatus ItineraryFlightStatus { get; set; }
        public Forecast Forecast { get; set; }
    }
}
