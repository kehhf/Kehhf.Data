//+---------------------------------------------------------------------+
//   DESCRIPTION: Database helper class
//       CREATED: Kehhf on 2014/05/29
// LAST MODIFIED:
//+---------------------------------------------------------------------+

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Kehhf.Data
{
    public static class DataHelper
    {
        public static DbParameter[] AssignParameters(DbParameter[] parameters, object[] parameterValues) {
            for (int i = 0; i < parameters.Length; ++i) {
                if (parameterValues[i] is DbParameter) {
                    parameters[i].Value = ((DbParameter)parameterValues[i]).Value ?? DBNull.Value;
                } else {
                    parameters[i].Value = parameterValues[i] ?? DBNull.Value;
                }
            }

            return parameters;
        }

        public static DbParameter[] CloneParameters(DbParameter[] parameters) {
            DbParameter[] clone = new DbParameter[parameters.Length];

            for (int i = 0; i < parameters.Length; ++i) {
                clone[i] = (DbParameter)(((ICloneable)parameters[i]).Clone());
                clone[i].Value = DBNull.Value;
            }

            return clone;
        }

        public static DbCommand CreateCommand(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters) {
            DbCommand command = null;

            if (transaction == null) {
                command = connection.CreateCommand();
            } else {
                command = transaction.Connection.CreateCommand();
            }

            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = transaction;

            if (commandParameters != null) {
                command.Parameters.AddRange(commandParameters);
            }

            return command;
        }

        public static DbConnection CreateConnection(ConnectionStringSettings connectionStringSettings) {
            DbConnection connection = ProviderFactoryCache.Get(connectionStringSettings.ProviderName).CreateConnection();

            connection.ConnectionString = connectionStringSettings.ConnectionString;

            return connection;
        }

        public static DbDataReader ExecuteDataReader(DbCommand command) {
            bool shouldClose = false;

            try {
                PrepareConnection(command.Connection, out shouldClose);

                return command.ExecuteReader(shouldClose ? CommandBehavior.CloseConnection : CommandBehavior.Default);
            }
            catch (Exception) {
                ReleaseConnection(command.Connection, shouldClose);
                throw;
            }
        }

        public static DbDataReader ExecuteDataReader(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters) {
            using (DbCommand command = CreateCommand(connection, transaction, commandType, commandText, commandParameters)) {
                return ExecuteDataReader(command);
            }
        }

        public static DataTable ExecuteDataTable(DbCommand command) {
            DataTable dt = new DataTable();

            dt.Load(ExecuteDataReader(command), LoadOption.OverwriteChanges);

            return dt;
        }

        public static DataTable ExecuteDataTable(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters) {
            using (DbCommand command = CreateCommand(connection, transaction, commandType, commandText, commandParameters)) {
                return ExecuteDataTable(command);
            }
        }

        public static int ExecuteNonQuery(DbCommand command) {
            bool shouldClose = false;

            try {
                PrepareConnection(command.Connection, out shouldClose);

                return command.ExecuteNonQuery();
            }
            finally {
                ReleaseConnection(command.Connection, shouldClose);
            }
        }

        public static int ExecuteNonQuery(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters) {
            using (DbCommand command = CreateCommand(connection, transaction, commandType, commandText, commandParameters)) {
                return ExecuteNonQuery(command);
            }
        }

        public static object ExecuteScalar(DbCommand command) {
            bool shouldClose = false;

            try {
                PrepareConnection(command.Connection, out shouldClose);

                return command.ExecuteScalar();
            }
            finally {
                ReleaseConnection(command.Connection, shouldClose);
            }
        }

        public static object ExecuteScalar(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters) {
            using (DbCommand command = CreateCommand(connection, transaction, commandType, commandText, commandParameters)) {
                return ExecuteScalar(command);
            }
        }

        public static void FillDataSet(DbCommand command, DataSet dataSet, string[] tables) {
            dataSet.Load(ExecuteDataReader(command), LoadOption.OverwriteChanges, tables);
        }

        public static void FillDataSet(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, DataSet dataSet, string[] tables) {
            using (DbCommand command = CreateCommand(connection, transaction, commandType, commandText, commandParameters)) {
                FillDataSet(command, dataSet, tables);
            }
        }

        private static void PrepareConnection(DbConnection connection, out bool shouldClose) {
            shouldClose = false;

            if (connection.State == ConnectionState.Broken) {
                connection.Close();
            }

            if (connection.State == ConnectionState.Closed) {
                connection.Open();
                shouldClose = true;
            }
        }

        private static void ReleaseConnection(DbConnection connection, bool shouldClose) {
            if ((shouldClose) &&
                (connection != null && connection.State != ConnectionState.Closed)) {
                connection.Close();
            }
        }
    }
}
