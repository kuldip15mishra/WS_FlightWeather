using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Common;
using FlightWeatherMailManager.InteractionManager;
using Library.ErrorLog;

namespace FlightWeatherMailManager
{
    class InteractionMailManager : IManager
    {

        public bool SendMail(FlightWeatherResponse flightWeatherResponse)
        {
            string body = GetBody(flightWeatherResponse);
            var subject = MailText.EmailSubject(flightWeatherResponse);
            using (MMT_WEBS_InteractionManagerSoapClient client = new MMT_WEBS_InteractionManagerSoapClient())
            {
                if (string.IsNullOrWhiteSpace(body))
                {
                    return false;
                }
                EMailAttachment[] attachments = null;
                if (flightWeatherResponse.ItineraryFlightStatus.FlightStatus == "S")
                {
                    try
                    {
                        attachments = new EMailAttachment[1];
                        EMailAttachment attachment = new EMailAttachment
                        {
                            Content = MailManager.GetETicketBytes(flightWeatherResponse.ItineraryQueue),
                            ContentId = "E-Ticket",
                            FileName = flightWeatherResponse.ItineraryQueue.BookingID + ".E-Ticket.pdf"
                        };
                        attachments[0] = attachment;
                    }
                    catch (Exception exception)
                    {
                        ErrorLog.WriteErrorLog(exception, flightWeatherResponse.ItineraryQueue.BookingID, "MMT_WS_FlightWeather");
                    }
                }

                EMailMessage mailMessage = new EMailMessage
                {
                    To = new[] { flightWeatherResponse.ItineraryQueue.BookingDetails.Email },
                    Subject = subject,
                    Source = MailText.Source,
                    ReferenceNo = flightWeatherResponse.ItineraryQueue.BookingID,
                    IsBodyHtml = true,
                    Body = body,
                    From = MailText.From,
                    ReferenceType = "BookingID",
                    RequestDate = DateTime.Now,
                    Attachment = attachments,
                    emailType = EmailType.General
                };
                string errorMessage = string.Empty;
                return client.SendEmail(mailMessage, ref errorMessage);
            }
        }

        public bool SendSMS(FlightWeatherResponse flightWeatherResponse)
        {
            throw new NotImplementedException();
        }

        public bool[] SendSMS(List<FlightWeatherResponse> flightWeatherResponses)
        {
            throw new NotImplementedException();
        }

        private string GetBody(FlightWeatherResponse flightWeatherResponse)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamReader rd = new StreamReader(ms))
                {
                    const string fileName = "Template.xslt";
                    XslCompiledTransform trans = Helper.CommonCache.GetOrInsertIntoCache(GetXslCompiledTransform, "FlightWeatherMailTemplate",
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", fileName));
                    if (trans == null)
                    {
                        return string.Empty;
                    }
                    XmlWriter writer = XmlWriter.Create(ms, trans.OutputSettings);
                    XElement element = getXElement(flightWeatherResponse);
                    trans.Transform(element.CreateReader(), writer);
                    ms.Position = 0;
                    return rd.ReadToEnd();
                }
            }
        }

        private XElement getXElement(FlightWeatherResponse flightWeatherResponse)
        {
            return XElement.Parse(SerializeDeserialize.SerializeObject(flightWeatherResponse));
        }

        private XslCompiledTransform GetXslCompiledTransform()
        {
            const string fileName = "Template.xslt";
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Templates",fileName)))
                return null;
            string strXSL = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + fileName);
            XmlReader reader = XmlReader.Create(new StringReader(strXSL));
            XslCompiledTransform trans = new XslCompiledTransform();
            trans.Load(reader);
            reader.Close();
            return trans;
        }
    }
}
