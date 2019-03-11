using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Common
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class ItineraryQueue : BaseItinerary
    {
        public string Status { get; set; }
        public DateTime ExpectedDateTime { get; set; }
        public int DelayInMinute { get; set; }
        public bool IsTriggered { get; set; }
        public DateTime PickTime { get; set; }
        [Obsolete]
        public bool IsPreviousTriggered { get; set; }
    }
}
