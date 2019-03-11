using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Runtime.Caching;
using Library.ErrorLog;

namespace Helper
{
    public static class CommonCache
    {
        private const int CacheTimeDefault = 25;
        static class TypeLock<T>
        {
            public static readonly object SyncLock = new object();
        }
        public static int CacheTime
        {
            get
            {
                return WebConfigurationManager.AppSettings["CacheTime"] == null
                    ? CacheTimeDefault
                    : int.Parse(WebConfigurationManager.AppSettings["CacheTime"]);
            }
        }
        public static T GetOrInsertIntoCache<T>(Func<T> func, string key)
        {
            Object obj = MemoryCache.Default[key];
            try
            {
                if (obj != null)
                {
                    return (T)obj;
                }
                lock (string.Intern(key))
                {
                    Object obj1 = MemoryCache.Default[key];
                    if (obj1 != null)
                    {
                        return (T)obj1;
                    }
                    T obj2 = func();
                    if (obj2 == null) return default(T);
                    var obj3 = MemoryCache.Default.AddOrGetExisting(key, obj2, DateTime.Now.AddMinutes(CacheTime));
                    return obj3 != null ? (T)obj3 : obj2;
                }
            }
            catch (Exception exception)
            {
                ErrorLog.WriteErrorLog(exception, key, "MMT_WS_FlightWeather");
                return default(T);
            }
        }
        public static T GetOrInsertIntoCache<T>(Func<T> func, string key, string filePath)
        {
            Object obj = MemoryCache.Default[key];
            try
            {
                if (obj != null)
                {
                    return (T)obj;
                }
                lock (TypeLock<T>.SyncLock)
                {
                    Object obj1 = MemoryCache.Default[key];
                    if (obj1 != null)
                    {
                        return (T)obj1;
                    }
                    T obj2 = func();
                    if (obj2 == null) return default(T);
                    CacheItemPolicy policy = new CacheItemPolicy();
                    List<string> filePaths = new List<string> { filePath };
                    policy.ChangeMonitors.Add(new HostFileChangeMonitor(filePaths));
                    policy.AbsoluteExpiration = DateTimeOffset.MaxValue;
                    var obj3 = MemoryCache.Default.AddOrGetExisting(key, obj2, policy);
                    return obj3 != null ? (T)obj3 : obj2;
                }
            }
            catch (Exception exception)
            {
                ErrorLog.WriteErrorLog(exception, "", "MMT_WS_FlightWeather");
                return default(T);
            }
        }
    }
}
