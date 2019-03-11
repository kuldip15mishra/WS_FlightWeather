using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace FlightStatus
{
    interface IFlightStatusDataSource
    {
        ItineraryFlightStatus GetStatus(BaseItinerary requestModel);
    }
}
