using translate_spa.Models;
using translate_spa.MongoDB.Interfaces;

namespace translate_spa.MongoDB.DbBuilder
{
    public class BaseDbBuilder : IDbBuilder
    {
        private static ConfigurationModel Configuration => new ConfigurationModel()
        {
            DatabaseName = Startup.Configuration.GetSection("ConnectionStrings")["DatabaseName"],
            ConnectionString = Startup.Configuration.GetSection("ConnectionStrings")["ConnectionString"],
        };

        public virtual string GetDatabaseName()
        {
            return Configuration.DatabaseName;
        }

        public virtual string GetConnectionString()
        {
            return Configuration.ConnectionString;
        }
    }
}