//+---------------------------------------------------------------------+
//   DESCRIPTION: SqlClient database class
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
    public sealed class SqlDatabase : Database, IBulkCopy, IExecuteStoredProcedureDirect, IQueryNotification
    {
        public SqlDatabase(string connectionStringName)
            : base(connectionStringName) {
        }

        #region << IBulkCopy >>
        public void BulkCopy(DataTable data, string destinationTable) {
            using (SqlBulkCopy sbc = new SqlBulkCopy(ConnectionStringSettings.ConnectionString, SqlBulkCopyOptions.UseInternalTransaction)) {
                sbc.BulkCopyTimeout = 1800;
                sbc.DestinationTableName = destinationTable;

                sbc.WriteToServer(data);
            }
        }
        #endregion

        #region << IExecuteStoredProcedureDirect >>
        public DataTable ExecuteDataTable(string storedProcedureName, object[] parameterValues) {
            return ExecuteDataTable(CommandType.StoredProcedure, storedProcedureName, DeriveStoredProcedureParameters(storedProcedureName, parameterValues));
        }

        public int ExecuteNonQuery(string storedProcedureName, object[] parameterValues) {
            return ExecuteNonQuery(CommandType.StoredProcedure, storedProcedureName, DeriveStoredProcedureParameters(storedProcedureName, parameterValues));
        }

        public object ExecuteScalar(string storedProcedureName, object[] parameterValues) {
            return ExecuteScalar(CommandType.StoredProcedure, storedProcedureName, DeriveStoredProcedureParameters(storedProcedureName, parameterValues));
        }

        public void FillDataSet(string storedProcedureName, object[] parameterValues, DataSet dataSet, string[] tables) {
            FillDataSet(CommandType.StoredProcedure, storedProcedureName, DeriveStoredProcedureParameters(storedProcedureName, parameterValues), dataSet, tables);
        }
        #endregion

        #region << IQueryNotification >>
        public DataTable ExecuteDataTable(CommandType commandType, string commandText, DbParameter[] commandParameters, EventHandler OnChangeCallback) {
            using (DbCommand command = CreateCommand(commandType, commandText, commandParameters, OnChangeCallback)) {
                return DataHelper.ExecuteDataTable(command);
            }
        }
        #endregion

        private DbCommand CreateCommand(CommandType commandType, string commandText, DbParameter[] commandParameters, EventHandler OnChangeCallback) {
            SqlCommand command = (SqlCommand)DataHelper.CreateCommand(_connection, _transaction, commandType, commandText, commandParameters);
            SqlDependency dependency = new SqlDependency(command);

            dependency.OnChange += new OnChangeEventHandler(OnChangeCallback);

            return command;
        }

        private DbParameter[] DeriveStoredProcedureParameters(string procedureName, object[] parameterValues) {
            DbParameter[] parameters = StoredProcedureParametersCache.Get(ConnectionStringSettings, procedureName);

            DataHelper.AssignParameters(parameters, parameterValues);

            return parameters;
        }
    }
}
