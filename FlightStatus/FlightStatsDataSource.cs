using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;
using System.Xml.Linq;
using Common;
using FlightStatus.FlightSoapApiService;
using Helper;
using Library.ErrorLog;

namespace FlightStatus
{
    public class FlightStatsDataSource : IFlightStatusDataSource
    {
        private readonly string _appId = WebConfigurationManager.AppSettings["FlightStatsAppID"];
        private readonly string _appKey = WebConfigurationManager.AppSettings["FlightStatsAppKey"];

        BaseItinerary _itineraryModel = new BaseItinerary();

        public ItineraryFlightStatus GetStatus(BaseItinerary requestModel)
        {
            _itineraryModel = requestModel;
            string key = requestModel.AirlineCode + requestModel.FlightNo +
                         requestModel.DepartureDateTime.ToString("ddd") + requestModel.FromCity + requestModel.ToCity;
            ItineraryFlightStatus response = CommonCache.GetOrInsertIntoCache(GetFlightStatsResponse, key);
            return response;
        }

        private ItineraryFlightStatus GetFlightStatsResponse()
        {
            string airlineCode = _itineraryModel.AirlineCode;
            string flightNo = _itineraryModel.FlightNo;
            int year = int.Parse(_itineraryModel.DepartureDateTime.ToString("yyyy"));
            int month = int.Parse(_itineraryModel.DepartureDateTime.ToString("MM"));
            int day = int.Parse(_itineraryModel.DepartureDateTime.ToString("dd"));

            responseFlightStatus responseFlightStatus;
            using (flightServiceClient client = new flightServiceClient())
            {
                responseFlightStatus = client.flightStatus_dep(_appId, _appKey, airlineCode, flightNo, year, month, day, "", "", "", "");
            }
            if (responseFlightStatus.error != null)
            {
                throw new Exception(responseFlightStatus.error.errorMessage);
            }

            var flightStatus =
                responseFlightStatus.flightStatuses.Where(
                    i => i.departureAirportFsCode == _itineraryModel.FromCity
                         || i.arrivalAirportFsCode == _itineraryModel.ToCity);

            var flightStatusV2S = flightStatus as flightStatusV2[] ?? flightStatus.ToArray();
            if (flightStatus == null || !flightStatusV2S.Any())
            {
                throw new Exception("No Flight status found for " + _itineraryModel.AirlineCode + " " + _itineraryModel.FlightNo);
            }

            return LogFlightStatus(flightStatusV2S.First(), MapItineraryFlightStatus(flightStatusV2S, responseFlightStatus));
        }

        private ItineraryFlightStatus MapItineraryFlightStatus(flightStatusV2[] statuses, responseFlightStatus responseFlightStatus)
        {
            var departureStatus = statuses.First(i => i.departureAirportFsCode == _itineraryModel.FromCity);
            var arrivalStatus = statuses.First(i => i.arrivalAirportFsCode == _itineraryModel.ToCity);
            var depAirportInfo = responseFlightStatus.appendix.airports.FirstOrDefault(i => i.iata == _itineraryModel.FromCity);
            var arrAirportInfo = responseFlightStatus.appendix.airports.FirstOrDefault(i => i.iata == _itineraryModel.ToCity);
            var airlineInfo =
                responseFlightStatus.appendix.airlines.FirstOrDefault(i => i.fs == _itineraryModel.AirlineCode);


            ItineraryFlightStatus flightStatus = new ItineraryFlightStatus
            {
                CarrierCode = departureStatus.carrierFsCode,
                FlightNumber = departureStatus.flightNumber,
                FlightStatus = departureStatus.status,
                DepartureAirportCode = departureStatus.departureAirportFsCode,
                ArrivalAirportCode = arrivalStatus.arrivalAirportFsCode
            };
            if (departureStatus.departureDate != null && departureStatus.departureDate.dateLocal != null)
            {
                flightStatus.DepartureDate = Convert.ToDateTime(departureStatus.departureDate.dateLocal);
            }
            if (arrivalStatus.arrivalDate != null && arrivalStatus.arrivalDate.dateLocal != null)
            {
                flightStatus.ArrivalDate = Convert.ToDateTime(arrivalStatus.arrivalDate.dateLocal);
            }
            if (departureStatus.operationalTimes != null && departureStatus.operationalTimes.scheduledGateDeparture != null
                && departureStatus.operationalTimes.scheduledGateDeparture.dateLocal != null)
            {
                flightStatus.ScheduledGateDeparture = Convert.ToDateTime(departureStatus.operationalTimes.scheduledGateDeparture.dateLocal);
            }
            if (departureStatus.operationalTimes != null && departureStatus.operationalTimes.estimatedGateDeparture != null
                && departureStatus.operationalTimes.estimatedGateDeparture.dateLocal != null)
            {
                flightStatus.EstimatedGateDeparture = Convert.ToDateTime(departureStatus.operationalTimes.estimatedGateDeparture.dateLocal);
            }

            if (departureStatus.operationalTimes != null && departureStatus.operationalTimes.actualGateDeparture != null
               && departureStatus.operationalTimes.actualGateDeparture.dateLocal != null)
            {
                flightStatus.ActualGateDeparture = Convert.ToDateTime(arrivalStatus.operationalTimes.actualGateDeparture.dateLocal);
            }

            if (arrivalStatus.operationalTimes != null && arrivalStatus.operationalTimes.scheduledGateArrival != null
               && arrivalStatus.operationalTimes.scheduledGateArrival.dateLocal != null)
            {
                flightStatus.ScheduledGateArrival = Convert.ToDateTime(arrivalStatus.operationalTimes.scheduledGateArrival.dateLocal);
            }
            if (arrivalStatus.operationalTimes != null && arrivalStatus.operationalTimes.estimatedGateArrival != null
               && arrivalStatus.operationalTimes.estimatedGateArrival.dateLocal != null)
            {
                flightStatus.EstimatedGateArrival = Convert.ToDateTime(arrivalStatus.operationalTimes.estimatedGateArrival.dateLocal);
            }

            if (arrivalStatus.operationalTimes != null && arrivalStatus.operationalTimes.actualGateArrival != null
                && arrivalStatus.operationalTimes.actualGateArrival.dateLocal != null)
            {
                flightStatus.ActualGateArrival = Convert.ToDateTime(arrivalStatus.operationalTimes.actualGateArrival.dateLocal);
            }

            if (departureStatus.delays != null)
            {
                flightStatus.DepartureGateDelayMin = departureStatus.delays.departureGateDelayMinutes;
            }
            if (departureStatus.airportResources != null)
            {
                flightStatus.DepTerminal = departureStatus.airportResources.departureTerminal;
            }
            if (arrivalStatus.airportResources != null)
            {
                flightStatus.ArrTerminal = arrivalStatus.airportResources.arrivalTerminal;
            }

            if (depAirportInfo != null)
            {
                flightStatus.DepCity = depAirportInfo.city;
                flightStatus.DepAirport = depAirportInfo.name;
            }

            if (arrAirportInfo != null)
            {
                flightStatus.ArrCity = arrAirportInfo.city;
                flightStatus.ArrAirPort = arrAirportInfo.name;
            }
            if (airlineInfo != null)
            {
                flightStatus.CarrierName = airlineInfo.name;
            }

            return flightStatus;
        }

        private ItineraryFlightStatus LogFlightStatus(flightStatusV2 status, ItineraryFlightStatus flightStatus)
        {
            using (MMTSqlLiveEntities entities = new MMTSqlLiveEntities(StaticHelperValues.MMTSqlEntitiesConnectionString))
            {
                tbl_FlightStatusLog flightStatusLog = new tbl_FlightStatusLog
                {
                    AirlineCode = flightStatus.CarrierCode,
                    FlightNo = flightStatus.FlightNumber,
                    Delays = flightStatus.DepartureGateDelayMin,
                    DepartureDate = flightStatus.DepartureDate,

                    Status = flightStatus.FlightStatus,
                    FromAirportCode = _itineraryModel.FromCity,
                    ToAirportCode = _itineraryModel.ToCity,
                    LastModifiedDate = DateTime.Now,
                    Response = XElement.Parse(SerializeDeserialize.SerializeObject(status)).ToString(SaveOptions.DisableFormatting)
                };
                if (flightStatus.EstimatedGateDeparture != DateTime.MinValue)
                {
                    flightStatusLog.ExpectedDate = flightStatus.EstimatedGateDeparture;
                }
                try
                {
                    entities.tbl_FlightStatusLog.Add(flightStatusLog);
                    entities.SaveChanges();
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
                }
                catch (Exception exception)
                {
                    ErrorLog.WriteErrorLog(exception, "", "MMT_WS_FlightWeather");
                }
            }
            return flightStatus;
        }
    }
}
