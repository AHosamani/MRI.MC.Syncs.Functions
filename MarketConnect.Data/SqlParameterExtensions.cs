using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MarketConnect.Data
{
    public static class SqlParameterExtensions
    {
        public const string SQL_COMMAND_IN_TEXT = "{in}";
        public static SqlParameter AddWithNullableValue(this SqlParameterCollection collection, string parameter, object value)
        {
            return collection.AddWithValue(parameter, value ?? DBNull.Value);
        }

        public static void In<T>(this SqlCommand command, List<T> parameters)
        {
            if (!command.CommandText.Contains(SQL_COMMAND_IN_TEXT))
            {
                throw new ArgumentException($"The SQL command text requires {SQL_COMMAND_IN_TEXT} in order to use SqlCommand.In().");
            }

            if (!parameters.Any())
            {
                throw new ArgumentException("SqlCommand.In() parameter list did not contain any items.");
            }

            var inText = string.Empty;

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = $"@P{i}";
                inText += $",{parameter}";
                command.Parameters.AddWithValue(parameter, parameters[i]);
            }

            command.CommandText = command.CommandText.Replace(SQL_COMMAND_IN_TEXT, inText.TrimStart(','));
        }
    }
}
