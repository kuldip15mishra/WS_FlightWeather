using System;
using Common;
using FlightStatus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightWeatherInfoTest
{
    [TestClass]
    public class FlightStatusTest
    {
        [TestMethod]
        public void TestMethodGetStatus()
        {
            ItineraryModel model = new ItineraryModel
            {
                AirlineCode = "9w",
                FlightNo = "312",
                DepartureDateTime = DateTime.Now,
                FromCity = "DEL",
                ToCity = "BOM"
            };
            FlightStatsDataSource stats = new FlightStatsDataSource();
            var a = stats.GetStatus(model);
        }
    }
}
