using System;
using ProtoBuf;

namespace Common
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class BookingDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string LOBCode { get; set; }
        public string GDSType { get; set; }
    }
}
