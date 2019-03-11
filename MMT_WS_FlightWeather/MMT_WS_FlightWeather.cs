using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AutoMapper;
using Common;
using FlightStatus;
using FlightWeatherMailManager;
using HeartBeatLogger;
using Library.ErrorLog;
using Timer = System.Timers.Timer;

namespace MMT_WS_FlightWeather
{
    public partial class MMT_WS_FlightWeather : ServiceBase
    {
        #region Constants
        private static readonly int AllowedDelayMin = int.Parse(ConfigurationManager.AppSettings["AllowedDelayMin"]);
        private static readonly int MinMinute = int.Parse(ConfigurationManager.AppSettings["MinMinute"]);
        private static readonly string ItineraryDumpPath = ConfigurationManager.AppSettings["ItineraryXmlDumpPath"];
        private static readonly string ItineraryQueueDumpPath = ConfigurationManager.AppSettings["ItineraryQueueDumpPath"];
        static readonly string TimerTimeInMilliSec = ConfigurationManager.AppSettings["timerTime"];
        private static readonly int ResendMailMin = int.Parse(ConfigurationManager.AppSettings["ResendMailMin"]);
        private static readonly int DiffInDelayMin = int.Parse(ConfigurationManager.AppSettings["DiffInDelayMin"]);
        const string WindowsServiceName = "MMT_WS_FlightWeather";
        #endregion

        #region variables
        readonly Timer _objTimer = new Timer(double.Parse(TimerTimeInMilliSec));
        List<ItineraryModel> _itineraryModels = new List<ItineraryModel>();
        ConcurrentDictionary<string, ItineraryQueue> _itineraryQueues = new ConcurrentDictionary<string, ItineraryQueue>();
        private readonly ItineraryFlightStatusData _flightStatusData;
        private Thread _workerThread;

        #endregion

        #region ctor
        public MMT_WS_FlightWeather()
        {
            InitializeComponent();
            _objTimer.Enabled = false;
            _objTimer.Elapsed += objTimer_Elapsed;
            _flightStatusData = new ItineraryFlightStatusData();
            Mapper.CreateMap<ItineraryModel, ItineraryQueue>();
        }
        #endregion

        #region Properties

        public ItineraryFlightStatusData FlightStatusData
        {
            get { return _flightStatusData; }
        }
        #endregion

        #region Events
        protected override void OnStart(string[] args)
        {
           // Debugger.Launch();
            SerializeDeserialize.DeserializeObject(_itineraryModels, ItineraryDumpPath);
            SerializeDeserialize.DeserializeQueue(_itineraryQueues, ItineraryQueueDumpPath);
            //for (int i = 0; i < 1; i++)
            //{
            //    ItineraryModel testItineraryModelodel = new ItineraryModel
            //    {
            //        BookingID = "NF2723228788846",
            //        AirlineCode = "6E",
            //        BookingDetails = new BookingDetails()
            //        {
            //            Email = "dheeraj.kumar@makemytrip.com",
            //            FirstName = "Dheeraj",
            //            LastName = "Kumar",
            //            MobileNo = "9310099421",
            //            LOBCode = "LOB02710"
            //        },
            //        DepartureDateTime = DateTime.Now,
            //        FlightNo = "522",
            //        FromCity = "DEL",
            //        ToCity = "LKO",
            //        LineNo = (10000 * (i + 1)).ToString(CultureInfo.InvariantCulture),
            //        PNR = "LER5RR" + i
            //    };
            //    _itineraryModels.Add(testItineraryModelodel);
            //}
            _objTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            SerializeDeserialize.SerializeObject(_itineraryModels, ItineraryDumpPath);
            SerializeDeserialize.SerializeQueue(_itineraryQueues, ItineraryQueueDumpPath);
        }

        public void objTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _objTimer.Enabled = false;
                FlightDataSource dataSource = new FlightDataSource();
                dataSource.GetItineraryData(_itineraryModels);
                RemoveStaleItinerary(_itineraryModels);

                var itineraryModelsGroupedByFlight = _itineraryModels.Where(i => i.IsProcessed == false)
                    .GroupBy(i => i.AirlineCode + i.FlightNo + i.DepartureDateTime.ToString("t") + i.FromCity + i.ToCity);
                foreach (var itineraries in itineraryModelsGroupedByFlight)
                {
                    ItineraryModel model = itineraries.FirstOrDefault();
                    if (model == null) continue;
                    try
                    {
                        var itineraryFlightStatus = FlightStatusData.GetItineraryFlightStatus(model);
                        if (itineraryFlightStatus != null && (new[] { "S", "C" }).Any(i => i == itineraryFlightStatus.FlightStatus.ToUpper()))
                        {
                            DateTime triggerDateTime
                                = itineraryFlightStatus.DepartureGateDelayMin < AllowedDelayMin
                                    ? itineraryFlightStatus.DepartureDate.AddMinutes(-MinMinute)
                                    : DateTime.Now;
                            foreach (ItineraryModel itineraryModel in itineraries)
                            {
                                string key = itineraryModel.BookingID + "-" + itineraryModel.AirlineCode +
                                             itineraryModel.FlightNo + "-" + itineraryModel.PNR + "-" +
                                             itineraryModel.DepartureDateTime.ToString("t");

                                ItineraryQueue queue = Mapper.Map<ItineraryQueue>(itineraryModel);
                                queue.IsTriggered = false;
                                queue.PickTime = triggerDateTime;
                                queue.Status = itineraryFlightStatus.FlightStatus;
                                queue.ExpectedDateTime = itineraryFlightStatus.EstimatedGateDeparture == DateTime.MinValue
                                                        ? itineraryFlightStatus.ScheduledGateDeparture : itineraryFlightStatus.EstimatedGateDeparture;
                                _itineraryQueues.TryAdd(key, queue);
                                itineraryModel.IsProcessed = true;
                            }
                        }
                        else
                        {
                            SendGeneralTicket(itineraries, itineraryFlightStatus);
                        }
                    }
                    catch (Exception exception)
                    {
                        SendGeneralTicket(itineraries, exception);
                    }
                }
                StartThreadsForQueue(_itineraryQueues);

            }
            catch (Exception exception)
            {
                ErrorLog.WriteErrorLog(exception, "", WindowsServiceName);
            }
            finally
            {
                _objTimer.Enabled = true;
                HeartBeatLoggerClass logger = new HeartBeatLoggerClass();
                logger.Log(WindowsServiceName);
            }
        }

        private static void SendGeneralTicket(IEnumerable<ItineraryModel> itineraries, Exception exception)
        {
            try
            {
                Parallel.ForEach(itineraries, itineraryModel =>
                {
                    itineraryModel.IsProcessed = true;
                    // MoveToDispatch.DoWithSP(itineraryModel.BookingID);
                    ErrorLog.WriteErrorLog(exception, itineraryModel.BookingID, WindowsServiceName);
                });
            }
            catch (AggregateException aggregateException)
            {
                ErrorLog.WriteErrorLog(aggregateException.Flatten(), "", WindowsServiceName);
            }
            catch (Exception ex)
            {
                ErrorLog.WriteErrorLog(ex, "", WindowsServiceName);
            }
        }

        private static void SendGeneralTicket(IEnumerable<ItineraryModel> itineraries, ItineraryFlightStatus itineraryFlightStatus)
        {
            try
            {
                Parallel.ForEach(itineraries, itineraryModel =>
                {
                    itineraryModel.IsProcessed = true;
                    string flightStatus = itineraryFlightStatus != null
                        ? itineraryFlightStatus.FlightStatus
                        : string.Empty;
                    // MoveToDispatch.DoWithSP(itineraryModel.BookingID);
                    ErrorLog.WriteErrorLog("FlightStatus: " + flightStatus, itineraryModel.BookingID,
                        "MMT_WS_FlightWeather");
                });
            }
            catch (AggregateException aggregateException)
            {
                ErrorLog.WriteErrorLog(aggregateException.Flatten(), "", WindowsServiceName);
            }
            catch (Exception exception)
            {
                ErrorLog.WriteErrorLog(exception, "", WindowsServiceName);
            }
        }

        #endregion

        #region private members
        private void RemoveStaleItinerary(List<ItineraryModel> list)
        {
            list.RemoveAll(i => i.DepartureDateTime < DateTime.Now.AddMinutes(-MinMinute + 2));
        }

        private void StartThreadsForQueue(ConcurrentDictionary<string, ItineraryQueue> queues)
        {
            if (_workerThread != null && !_workerThread.IsAlive)
            {
                _workerThread = null;
            }
            if (_workerThread == null)
            {
                _workerThread = new Thread(ProcessQueue) { Name = "QueueThread" };
                _workerThread.Start(queues);
            }
        }

        private void ProcessQueue(object obj)
        {
            ConcurrentDictionary<string, ItineraryQueue> queues = (ConcurrentDictionary<string, ItineraryQueue>)obj;

            while (queues.Count > 0)
            {
                foreach (var itemToRemove in queues.Where(i => (i.Value.ExpectedDateTime != DateTime.MinValue
                   && i.Value.PickTime > i.Value.ExpectedDateTime.AddMinutes(-MinMinute))))
                {
                    ItineraryQueue itineraryQueue;
                    queues.TryRemove(itemToRemove.Key, out itineraryQueue);
                }
                var toProcess =
                    queues.Where(i => i.Value.PickTime < DateTime.Now)
                        .GroupBy(i => i.Value.AirlineCode + i.Value.FlightNo + i.Value.DepartureDateTime.ToString("t") + i.Value.FromCity + i.Value.ToCity).ToList();
                try
                {
                    Parallel.ForEach(toProcess, eachEntry =>
                    {
                        ItineraryQueue item = eachEntry.First().Value;
                        try
                        {
                            var itineraryFlightStatus = FlightStatusData.GetItineraryFlightStatus(item);

                            if (item.IsTriggered == false || (item.DelayInMinute != itineraryFlightStatus.DepartureGateDelayMin
                                   && Math.Abs(itineraryFlightStatus.DepartureGateDelayMin - item.DelayInMinute) > DiffInDelayMin))
                            {
                                ItineraryFlightWeatherMail itineraryFlightWeatherMail = new ItineraryFlightWeatherMail();
                                itineraryFlightWeatherMail.Send(eachEntry.Select(i => i.Value).ToList(), itineraryFlightStatus);
                            }
                            eachEntry.ToList().ForEach(i =>
                            {
                                i.Value.DelayInMinute = itineraryFlightStatus.DepartureGateDelayMin;
                                i.Value.ExpectedDateTime = itineraryFlightStatus.EstimatedGateDeparture == DateTime.MinValue
                                    ? itineraryFlightStatus.ScheduledGateDeparture : itineraryFlightStatus.EstimatedGateDeparture;

                                i.Value.PickTime = i.Value.PickTime.AddMinutes(ResendMailMin);
                                i.Value.IsTriggered = true;
                            });
                        }
                        catch (Exception ex)
                        {
                            eachEntry.ToList().ForEach(i =>
                            {
                                i.Value.PickTime = i.Value.PickTime.AddMinutes(ResendMailMin);
                                i.Value.IsTriggered = true;
                            });
                            foreach (var keyValuePair in eachEntry)
                            {
                                ErrorLog.WriteErrorLog(ex, keyValuePair.Value.BookingID, WindowsServiceName);
                            }
                        }
                    });
                }
                catch (AggregateException aggregateException)
                {
                    ErrorLog.WriteErrorLog(aggregateException.Flatten(), "", WindowsServiceName);
                }
                catch (Exception exception)
                {
                    ErrorLog.WriteErrorLog(exception, "", WindowsServiceName);
                }
                Thread.Sleep(6000);
            }
        }
        #endregion
    }
}
