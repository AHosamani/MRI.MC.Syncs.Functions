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
            IEnumerable<string> ids = new List<string>();
            string connectionString = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                string query = "SELECT [PropertyId] FROM [dbo].[PropertySync]";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Execute the query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Access data from the reader
                            ids.Append(reader.GetString(0));
                        }
                    }
                }
            }
            return ids;
        }
        
    }
}
