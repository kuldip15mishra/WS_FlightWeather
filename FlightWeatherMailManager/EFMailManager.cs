using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Helper;
using Library.ErrorLog;

namespace FlightWeatherMailManager
{
    class EFMailManager : IManager
    {
        public bool SendMail(FlightWeatherResponse flightWeatherResponse)
        {
            throw new NotImplementedException();
        }

        public bool SendSMS(FlightWeatherResponse flightWeatherResponse)
        {
            throw new NotImplementedException();
        }

        public bool[] SendSMS(List<FlightWeatherResponse> flightWeatherResponses)
        {
            bool[] isSMSSent = new bool[flightWeatherResponses.Count];
            int i = 0;


            var connectionString = StaticHelperValues.MMTLiveEntitiesConnectionString;
            using (MMTLiveEntities entities = new MMTLiveEntities(connectionString))
            {
                foreach (SMS_Queue smsQueue in from flightWeatherResponse in flightWeatherResponses
                                               let smsText = MailText.GetSMSText(flightWeatherResponse)
                                               select new SMS_Queue
                {
                    Message_ID = Guid.NewGuid().ToString(),
                    Request_Date_Time = DateTime.Now,
                    Sender_Name = MailText.SenderName,
                    //menan TODO: Use ur no menan
                    //Mobile_No_ = "7827334489",
                    Mobile_No_ = flightWeatherResponse.ItineraryQueue.BookingDetails.MobileNo,
                    Message_Text = smsText,
                    Document_No_ = flightWeatherResponse.ItineraryQueue.BookingID,
                    Trigger_Point = MailText.TriggerPoint,
                    Schedule_SMS = 0,
                    LOBID = flightWeatherResponse.ItineraryQueue.BookingDetails.LOBCode,
                    BodyDocument = "",
                    AttachmentDocument = "",
                    Acknowledgement_No_ = "",
                    From = "",
                    To = "",
                    CC = "",
                    BCC = "",
                    Subject = "",
                    Source = MailText.Source,
                    RefrenceType = MailText.ReferenceTypeSMS
                })
                {
                    entities.SMS_Queues.Add(smsQueue);
                }

                try
                {
                    entities.SaveChanges();
                    isSMSSent[i++] = true;
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (
                        DbValidationError error in
                            dbEx.EntityValidationErrors.SelectMany(entityErr => entityErr.ValidationErrors))
                    {
                        string errorMessage = string.Format("Error Property Name {0} : Error Message: {1}",
                           error.PropertyName, error.ErrorMessage);
                        ErrorLog.WriteErrorLog(errorMessage, "", "MMT_WS_FlightWeather");
                       
                    }
                    isSMSSent[i++] = false;

                }
                catch (Exception exception)
                {
                    isSMSSent[i++] = false;
                    foreach (var flightWeatherResponse in flightWeatherResponses)
                    {
                        ErrorLog.WriteErrorLog(exception, flightWeatherResponse.ItineraryQueue.BookingID, "MMT_WS_FlightWeather");
                    }
                }
            }
            return isSMSSent;
        }
    }
}
