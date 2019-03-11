using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace FlightStatus
{
    class GDSDataSource : IFlightStatusDataSource
    {
        public ItineraryFlightStatus GetStatus(BaseItinerary requestModel)
        {
            throw new NotImplementedException();
        }
    }
}
