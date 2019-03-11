using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace FlightWeatherMailManager
{
    interface IManager
    {
        bool SendMail(FlightWeatherResponse flightWeatherResponse);
        bool SendSMS(FlightWeatherResponse flightWeatherResponse);
        bool[] SendSMS(List<FlightWeatherResponse> flightWeatherResponses);
    }
}
