//+---------------------------------------------------------------------+
//   DESCRIPTION: Connection string manager class
//       CREATED: Kehhf on 2014/05/29
// LAST MODIFIED:
//+---------------------------------------------------------------------+

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace Kehhf.Data
{
    public sealed class ConnectionStringManager : ICollection<ConnectionStringSettings>
    {
        public static readonly ConnectionStringManager Instance = new ConnectionStringManager();

        private readonly List<ConnectionStringSettings> _container = new List<ConnectionStringSettings>();

        private ConnectionStringManager() {
            LoadFromConfiguration();
        }

        public ConnectionStringSettings Get(int index) {
            return _container[index];
        }

        public ConnectionStringSettings Get(string name) {
            foreach (ConnectionStringSettings item in _container) {
                if (item.Name == name) {
                    return item;
                }
            }

            throw new KeyNotFoundException();
        }

        #region << ICollection<T> >>
        int ICollection<ConnectionStringSettings>.Count {
            get { return _container.Count; }
        }

        bool ICollection<ConnectionStringSettings>.IsReadOnly {
            get { return false; }
        }

        void ICollection<ConnectionStringSettings>.Add(ConnectionStringSettings item) {
            if (item == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(item.Name) || string.IsNullOrEmpty(item.ProviderName) || string.IsNullOrEmpty(item.ConnectionString)) throw new ArgumentException();
            if (_container.IndexOf(item) != -1) throw new ArgumentException();

            _container.Add(item);
        }

        void ICollection<ConnectionStringSettings>.Clear() {
            _container.Clear();
        }

        bool ICollection<ConnectionStringSettings>.Contains(ConnectionStringSettings item) {
            return _container.Contains(item);
        }

        void ICollection<ConnectionStringSettings>.CopyTo(ConnectionStringSettings[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        bool ICollection<ConnectionStringSettings>.Remove(ConnectionStringSettings item) {
            return _container.Remove(item);
        }
        #endregion

        #region << IEnumerable<T> >>
        IEnumerator<ConnectionStringSettings> IEnumerable<ConnectionStringSettings>.GetEnumerator() {
            return _container.GetEnumerator();
        }
        #endregion

        #region << IEnumerable >>
        IEnumerator IEnumerable.GetEnumerator() {
            return _container.GetEnumerator();
        }
        #endregion

        private void LoadFromConfiguration() {
            try {
                foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings) {
                    ((ICollection<ConnectionStringSettings>)this).Add(item);
                }
            }
            catch (ConfigurationErrorsException) { }
        }
    }
}
