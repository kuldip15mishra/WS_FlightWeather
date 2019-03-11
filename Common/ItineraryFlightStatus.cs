using System;

namespace Common
{
    [Serializable]
    public class ItineraryFlightStatus
    {
        public string CarrierCode { get; set; }
        public string CarrierName { get; set; }
        public string FlightNumber { get; set; }
        public string FlightStatus { get; set; }
        public string DepartureAirportCode { get; set; }
        public string ArrivalAirportCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime ScheduledGateDeparture { get; set; }
        public DateTime EstimatedGateDeparture { get; set; }
        public DateTime ScheduledGateArrival { get; set; }
        public DateTime EstimatedGateArrival { get; set; }
        public DateTime ActualGateDeparture { get; set; }
        public DateTime ActualGateArrival { get; set; }
        public int DepartureGateDelayMin { get; set; }
        public string DepTerminal { get; set; }
        public string ArrTerminal { get; set; }
        public string DepCity { get; set; }
        public string ArrCity { get; set; }
        public string DepAirport { get; set; }
        public string ArrAirPort { get; set; }

    }
}
