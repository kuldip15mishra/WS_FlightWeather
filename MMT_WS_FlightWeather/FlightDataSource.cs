using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper.Internal;
using Common;
using Library.ErrorLog;

namespace MMT_WS_FlightWeather
{
    class FlightDataSource
    {
        readonly string _readNavDBConnectionString = ConfigurationManager.AppSettings["ConnectionMMTLive"];
        readonly string _lobCodes = ConfigurationManager.AppSettings["LOBCodes"];
        readonly int _maxMinute = int.Parse(ConfigurationManager.AppSettings["MaxMinute"]);
        readonly int _minMinute = int.Parse(ConfigurationManager.AppSettings["MinMinute"]);
        const string WindowsServiceName = "MMT_WS_FlightWeather";

        public List<ItineraryModel> GetItineraryData(List<ItineraryModel> sourceList)
        {
            DataTable sqlDataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(_readNavDBConnectionString))
                {
                    const string strProcName = "usp_GetCurrentDepartureItineraryData";
                    SqlCommand cmd = new SqlCommand(strProcName, connection) { CommandType = CommandType.StoredProcedure };
                    SqlDataAdapter sqlDataAdapterObj = new SqlDataAdapter(cmd);
                    SqlCommandBuilder sqlCommandBuilderObj = new SqlCommandBuilder(sqlDataAdapterObj);
                    sqlDataAdapterObj.Fill(sqlDataTable);
                }
                MergeDataTableToList(sourceList, sqlDataTable);
            }
            catch (Exception exception)
            {
                ErrorLog.WriteErrorLog(exception, "", WindowsServiceName);
            }
            return sourceList;
        }

        private void MergeDataTableToList(List<ItineraryModel> source, DataTable table)
        {
            try
            {
                foreach (ItineraryModel model in from DataRow dataRow in table.Rows
                                                 where !CheckDataRowInItineraryList(source, dataRow) && !IsMultiSegmentFlightNo(dataRow)
                                                 select new ItineraryModel
                                                 {
                                                     AirlineCode =
                                                         dataRow["Offered Airline"] != null ? dataRow["Offered Airline"].ToString() : string.Empty,
                                                     BookingID = dataRow["No_"] != null ? dataRow["No_"].ToString() : string.Empty,
                                                     DepartureDateTime = (DateTime)dataRow["DepartureDateTime"],
                                                     FlightNo = dataRow["Flight No_"] != null ? dataRow["Flight No_"].ToString() : string.Empty,
                                                     FromCity = dataRow["From City"] != null ? dataRow["From City"].ToString() : string.Empty,
                                                     ToCity = dataRow["To City"] != null ? dataRow["To City"].ToString() : string.Empty,
                                                     LineNo = dataRow["Line No_"] != null ? dataRow["Line No_"].ToString() : string.Empty,
                                                     PNR = dataRow["PNR No_"] != null ? dataRow["PNR No_"].ToString() : string.Empty,
                                                     BookingDetails = new BookingDetails
                                                     {
                                                         Email = dataRow["E-Mail ID"] != null ? dataRow["E-Mail ID"].ToString() : string.Empty,
                                                         FirstName = dataRow["First Name"] != null ? dataRow["First Name"].ToString() : string.Empty,
                                                         LastName = dataRow["Last Name"] != null ? dataRow["Last Name"].ToString() : string.Empty,
                                                         MobileNo = dataRow["Mobile No_"] != null ? dataRow["Mobile No_"].ToString() : string.Empty,
                                                         PhoneNo = dataRow["Phone No_"] != null ? dataRow["Phone No_"].ToString() : string.Empty,
                                                         GDSType = dataRow["GDS Type"] != null ? dataRow["GDS Type"].ToString() : string.Empty,
                                                         LOBCode = dataRow["LOB Code"] != null ? dataRow["LOB Code"].ToString() : string.Empty
                                                     }
                                                 })
                {
                    source.Add(model);
                }
            }
            catch (Exception exception)
            {
                ErrorLog.WriteErrorLog(exception, "", WindowsServiceName);
            }
        }

        private static bool CheckDataRowInItineraryList(IEnumerable<ItineraryModel> source, DataRow row)
        {
            return source.Any(i => i.BookingID == row["No_"].ToNullSafeString() && i.LineNo == row["Line No_"].ToNullSafeString());
        }

        private static bool IsMultiSegmentFlightNo(DataRow row)
        {
            return row["Flight No_"].ToNullSafeString().Contains("/");
        }
    }
}
