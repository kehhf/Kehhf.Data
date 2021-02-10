//+---------------------------------------------------------------------+
//   DESCRIPTION: Bulk copy interface
//       CREATED: Kehhf on 2014/05/29
// LAST MODIFIED:
//+---------------------------------------------------------------------+

using System;
using System.Collections.Generic;
using System.Data;

namespace Kehhf.Data
{
    public interface IBulkCopy
    {
        void BulkCopy(DataTable data, string destinationTable);
    }
}
