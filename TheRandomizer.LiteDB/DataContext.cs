using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System.Configuration;
using TheRandomizer.Utility.Collections;

namespace TheRandomizer.LiteDB
{
    public class DataContext : IDisposable
    {
        public string ConnectionString { get; private set; }
        
        private LiteDatabase _db;

        private DataContext(string connectionStringName)
        {
            ConnectionString = ConfigurationManager.AppSettings[connectionStringName];
        }

        private bool Open()
        {
            if (_db == null)
            {
                _db = new LiteDatabase(ConnectionString);
            }
            return _db != null;
        }

        private bool Close()
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            return _db == null;
        }

        #region Generators

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_db != null) _db.Dispose();
                }
                
                disposedValue = true;
            }
        }
        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
