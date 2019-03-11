using System;
using Library.ErrorLog;
using MMT_WS_FlightWeather.MMT_Webs_MOServices;

namespace MMT_WS_FlightWeather
{
    public static class MoveToDispatch
    {
        private const string UserName = "WEBLIVE";
        public static bool DoWithSP(string bookingID)
        {
            try
            {
                using (MOServices client = new MOServices())
                {
                    int isDispatch = 0;
                    string errorMessage = string.Empty;
                    client.MoveToDispatch(bookingID, UserName, ref isDispatch, ref errorMessage);

                    return isDispatch == 1;
                }
            }
            catch (Exception exception)
            {
                ErrorLog.WriteErrorLog(exception, bookingID, "MMT_WS_FlightWeather");
                return false;
            }
        }

        public static bool DoWithRequest(string bookingID)
        {
            try
            {
                using (MOServices client = new MOServices())
                {
                    WSRequest objWSRequest = new WSRequest
                    {
                        DocumentInfo = new DocumentInfo
                        {
                            RequestDate = DateTime.Now.ToString("yyyy-MM-dd"),
                            RequestTime = DateTime.Now.ToString("hh-mm-ss tt"),
                            RequestID = Guid.NewGuid().ToString(),
                            UserID = UserName
                        }
                    };
                    MoveToDispatchRequest moveToDispatchRequest = new MoveToDispatchRequest
                    {
                        DocumentNo = bookingID,
                        DocumentType = "1",
                        enMSGType = MSGType.ResendETicket
                    };

                    objWSRequest.Request = moveToDispatchRequest;
                    var response = client.ProcessRequest(objWSRequest);
                    return response.Response.Status == "1";
                }
            }
            catch (Exception exception)
            {
                ErrorLog.WriteErrorLog(exception, bookingID, "MMT_WS_FlightWeather");
                return false;
            }
        }
    }
}
