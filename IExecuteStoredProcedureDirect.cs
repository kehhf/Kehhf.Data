//+---------------------------------------------------------------------+
//   DESCRIPTION: Execute stored procedure direct interface
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
    public interface IExecuteStoredProcedureDirect
    {
        DataTable ExecuteDataTable(string storedProcedureName, object[] parameterValues);

        int ExecuteNonQuery(string storedProcedureName, object[] parameterValues);

        object ExecuteScalar(string storedProcedureName, object[] parameterValues);

        void FillDataSet(string storedProcedureName, object[] parameterValues, DataSet dataSet, string[] tables);
    }
}
