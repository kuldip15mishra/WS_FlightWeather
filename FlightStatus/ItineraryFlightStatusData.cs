using System.Web.Configuration;
using Common;

namespace FlightStatus
{
    public class ItineraryFlightStatusData
    {

        public ItineraryFlightStatus GetItineraryFlightStatus(BaseItinerary source)
        {
            IFlightStatusDataSource dataSource = GetFlightDataSource(source);
            return dataSource.GetStatus(source);
        }

        private static IFlightStatusDataSource GetFlightDataSource(BaseItinerary source)
        {
            string airlineCodesGDSSource = WebConfigurationManager.AppSettings["AirlineCodeGDSSource"];
            if (airlineCodesGDSSource.Contains(source.AirlineCode))
            {
                return new GDSDataSource();
            }
            return new FlightStatsDataSource();
        }
    }
}
