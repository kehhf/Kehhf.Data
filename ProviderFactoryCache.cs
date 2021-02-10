//+---------------------------------------------------------------------+
//   DESCRIPTION: Database provider factory cache class
//       CREATED: Kehhf on 2014/05/29
// LAST MODIFIED:
//+---------------------------------------------------------------------+

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Text;

namespace Kehhf.Data
{
    internal static class ProviderFactoryCache
    {
        private static readonly Dictionary<string, DbProviderFactory> _cache = new Dictionary<string, DbProviderFactory>();
        private static object _syncRoot = new object();

        static ProviderFactoryCache() {
            _cache.Add("System.Data.Odbc", OdbcFactory.Instance);
            _cache.Add("System.Data.OleDb", OleDbFactory.Instance);
            _cache.Add("System.Data.SqlClient", SqlClientFactory.Instance);
        }

        public static DbProviderFactory Get(string providerName) {
            if (!_cache.ContainsKey(providerName)) {
                lock (_syncRoot) {
                    if (!_cache.ContainsKey(providerName)) {
                        _cache.Add(providerName, DbProviderFactories.GetFactory(providerName));
                    }
                }
            }

            return _cache[providerName];
        }
    }
}
