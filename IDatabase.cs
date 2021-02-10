//+---------------------------------------------------------------------+
//   DESCRIPTION: Database interface
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
    public interface IDatabase
    {
        ConnectionStringSettings ConnectionStringSettings { get; }

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);

        void Commit();

        DbParameter CreateParameter(string parameterName, DbType type, object value);

        DbDataReader ExecuteDataReader(CommandType commandType, string commandText, DbParameter[] commandParameters);

        DataTable ExecuteDataTable(CommandType commandType, string commandText, DbParameter[] commandParameters);

        int ExecuteNonQuery(CommandType commandType, string commandText, DbParameter[] commandParameters);

        object ExecuteScalar(CommandType commandType, string commandText, DbParameter[] commandParameters);

        void FillDataSet(CommandType commandType, string commandText, DbParameter[] commandParameters, DataSet dataSet, string[] tables);

        void Rollback();
    }
}
