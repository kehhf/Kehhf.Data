//+---------------------------------------------------------------------+
//   DESCRIPTION: Database class
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
    public class Database : IDatabase
    {
        protected DbConnection _connection = null;
        protected DbTransaction _transaction = null;

        public Database(string connectionStringName)
            : this(ConnectionStringManager.Instance.Get(connectionStringName)) {
        }

        public Database(ConnectionStringSettings connectionStringSettings) {
            _connection = DataHelper.CreateConnection((ConnectionStringSettings = connectionStringSettings));
        }

        public ConnectionStringSettings ConnectionStringSettings { get; private set; }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified) {
            if (_transaction != null && _transaction.Connection != null) throw new InvalidOperationException();

            _connection.Open();
            _transaction = _connection.BeginTransaction(isolationLevel);
        }

        public void Commit() {
            _transaction.Commit();
            _connection.Close();

            _transaction = null;
        }

        public DbParameter CreateParameter(string parameterName, DbType type, object value) {
            DbParameter parameter = ProviderFactoryCache.Get(ConnectionStringSettings.ProviderName).CreateParameter();

            parameter.ParameterName = parameterName;
            parameter.DbType = type;
            parameter.Value = value;

            return parameter;
        }

        public DbDataReader ExecuteDataReader(CommandType commandType, string commandText, DbParameter[] commandParameters) {
            return DataHelper.ExecuteDataReader(_connection, _transaction, commandType, commandText, commandParameters);
        }

        public DataTable ExecuteDataTable(CommandType commandType, string commandText, DbParameter[] commandParameters) {
            return DataHelper.ExecuteDataTable(_connection, _transaction, commandType, commandText, commandParameters);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText, DbParameter[] commandParameters) {
            return DataHelper.ExecuteNonQuery(_connection, _transaction, commandType, commandText, commandParameters);
        }

        public object ExecuteScalar(CommandType commandType, string commandText, DbParameter[] commandParameters) {
            return DataHelper.ExecuteScalar(_connection, _transaction, commandType, commandText, commandParameters);
        }

        public void FillDataSet(CommandType commandType, string commandText, DbParameter[] commandParameters, DataSet dataSet, string[] tables) {
            DataHelper.FillDataSet(_connection, _transaction, commandType, commandText, commandParameters, dataSet, tables);
        }

        public void Rollback() {
            _transaction.Rollback();
            _connection.Close();

            _transaction = null;
        }
    }
}
