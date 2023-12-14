using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SyncWebjobs
{
    public class PropertyService
    {
        public PropertyService() { }



        public IEnumerable<string> GetAllPropertyIds()
        {
            List<string> ids = new List<string>();
            string connectionString = "Server=sql-eastus-pna-qa.database.windows.net;Database=sqldb-eastus-pna-portal-qa;User Id=zLZ3tOGLxjk3Wx69RvyY;Password='ETeIP2N5XhOPYL71R2mj';MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30; ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                string query = "SELECT [PropertyId] FROM [dbo].[PropertySync]";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Execute the query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Access data from the reader
                            //ids.Append(reader.GetString(0));
                            string propertyId = reader.GetString(0);
                            ids.Add(propertyId);
                        }
                    }
                }
            }
            return ids;
        }

    }
}
