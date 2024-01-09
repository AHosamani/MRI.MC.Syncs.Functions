using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MarketConnect.Data.Repository
{
    public class BaseRepository
    {
        /// <summary>
        /// Get connection dashBoard database connection string
        /// </summary>
        public IDbConnection DefaultConnection
        {
            get
            {
                return new SqlConnection(ConfigurationManager.ConnectionStrings["MarketConnect.Connection"].ConnectionString);
            }
        }
    }
}
