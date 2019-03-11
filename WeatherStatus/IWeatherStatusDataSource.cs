using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStatus
{
   public interface IWeatherStatusDataSource
    {
        RootObject GetWeatherStatus(string airportCode);
    }
}
