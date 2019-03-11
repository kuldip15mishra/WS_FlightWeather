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
    public class ItineraryModel : BaseItinerary
    {
        public string LineNo { get; set; }
        public bool IsProcessed { get; set; }
    }
}
