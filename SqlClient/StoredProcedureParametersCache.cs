//+---------------------------------------------------------------------+
//   DESCRIPTION: Stored procedure parameters cache class
//       CREATED: Kehhf on 2014/05/29
// LAST MODIFIED:
//+---------------------------------------------------------------------+

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Kehhf.Data.SqlClient
{
    internal static class StoredProcedureParametersCache
    {
        private static readonly Dictionary<string, DbParameter[]> _cache = new Dictionary<string, DbParameter[]>();
        private static readonly object _syncRoot = new object();

        public static DbParameter[] Get(ConnectionStringSettings settings, string procedureName) {
            string key = string.Concat(settings.Name, "$", procedureName);
            DbParameter[] parameters;

            if (!_cache.TryGetValue(key, out parameters)) {
                lock (_syncRoot) {
                    if (!_cache.TryGetValue(key, out parameters)) {
                        _cache.Add(key, (parameters = DeriveParameters(settings, procedureName)));
                    }
                }
            }

            return DataHelper.CloneParameters(parameters);
        }

        private static SqlParameter[] DeriveParameters(ConnectionStringSettings settings, string procedureName) {
            using (SqlConnection connection = (SqlConnection)DataHelper.CreateConnection(settings)) {
                using (SqlCommand command = (SqlCommand)DataHelper.CreateCommand(connection, null, CommandType.StoredProcedure, procedureName, null)) {
                    connection.Open();

                    SqlCommandBuilder.DeriveParameters(command);

                    SqlParameter[] result = new SqlParameter[command.Parameters.Count - 1];

                    command.Parameters.RemoveAt(0);
                    command.Parameters.CopyTo(result, 0);

                    return result;
                }
            }
        }
    }
}
