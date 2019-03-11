using System;
using System.Runtime.InteropServices.ComTypes;
using ProtoBuf;

namespace Common
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(100, typeof(ItineraryModel))]
    [ProtoInclude(101, typeof(ItineraryQueue))]
    public class BaseItinerary
    {
        public string BookingID { get; set; }
        public string PNR { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNo { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public DateTime DepartureDateTime { get; set; }
        [ProtoMember(103)]
        public BookingDetails BookingDetails { get; set; }
    }
}
