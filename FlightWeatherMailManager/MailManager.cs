using System;
using Common;
using FlightWeatherMailManager.MMT_Webs_MailManager;
using Library.ErrorLog;

namespace FlightWeatherMailManager
{
    public static class MailManager
    {
        public static Byte[] GetETicketBytes(BaseItinerary itinerary)
        {
            try
            {
                using (MMT_WEBS_MailManagerSoapClient client = new MMT_WEBS_MailManagerSoapClient())
                {
                    var objMailRequest = new RequestObj
                    {
                        ContactNo = itinerary.BookingDetails.MobileNo,
                        LOBID = GetLOBName(itinerary.BookingDetails.LOBCode),
                        RequestDate = DateTime.Now,
                        From = MailText.From,
                        To = itinerary.BookingDetails.Email,
                        Subject = "MMT_WS_FlightWeather",
                        Source = MailText.Source,
                        SendMail = SendMailEnum.NoMail,
                        BodyDocument = "ETicket",
                        AttachmentDocument = new[] { "ETicket" },
                        RefrenceNo = itinerary.BookingID,
                        RefrenceType = "BookingID"
                    };

                    string strMailBody = string.Empty;
                    string strErrorMessage = string.Empty;
                    Byte[] byteAttachment = new byte[] { };
                    client.SendMail(objMailRequest, ref strMailBody, ref byteAttachment,
                        ref strErrorMessage);
                    return byteAttachment;
                }
            }
            catch (Exception exception)
            {

                ErrorLog.WriteErrorLog(exception, itinerary.BookingID, "MMT_WS_FlightWeather");
            }
            return null;
        }


        private static string GetLOBName(string LOBCode)
        {
            switch (LOBCode)
            {
                case "LOB02710":
                    return "B2C-ON-Air Domestic";
            }
            return null;
        }


    }
}
