//+---------------------------------------------------------------------+
//   DESCRIPTION: Query notification interface
//       CREATED: Kehhf on 2014/05/29
// LAST MODIFIED:
//+---------------------------------------------------------------------+

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Kehhf.Data
{
    public interface IQueryNotification
    {
        DataTable ExecuteDataTable(CommandType commandType, string commandText, DbParameter[] commandParameters, EventHandler OnChangeCallback);
    }
}
