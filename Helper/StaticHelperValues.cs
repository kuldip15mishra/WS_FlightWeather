using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class StaticHelperValues
    {
        public static readonly string MMTLiveEntitiesConnectionString;
        public static readonly string MMTSqlEntitiesConnectionString;
        static StaticHelperValues()
        {
            #region Entity Connection String
            var entityMetadata = ConfigurationManager.AppSettings["MMTLiveEntitiesMetadata"];
            var connectionString = ConfigurationManager.AppSettings["ConnectionMMTLive"];
            MMTLiveEntitiesConnectionString = entityMetadata + "provider connection string=\"" + connectionString + "\""; ;

            var entitySqlMetadata = ConfigurationManager.AppSettings["MMTSQLLiveEntitiesMetadata"];
            var connectionSqlString = ConfigurationManager.AppSettings["ConnectionMMTSQL"];
            MMTSqlEntitiesConnectionString = entitySqlMetadata + "provider connection string=\"" + connectionSqlString + "\"";
            #endregion
        }
    }
}
