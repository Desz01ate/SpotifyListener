using System;
using Utilities.SQL;
using Utilities.Interfaces;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using SpotifyListener.DatabaseManager.Repositories;
namespace SpotifyListener.DatabaseManager
{
    public partial class Service : IDisposable
    {
        private readonly IDatabaseConnector Connector;
        public Service(string connectionString)
        {
            Connector = new DatabaseConnector(typeof(System.Data.SQLite.SQLiteConnection),connectionString);
        }
        private ListenHistoryRepository _ListenHistory { get; set; }
        public ListenHistoryRepository ListenHistory
        {
            get
            {
                if (_ListenHistory == null)
                {
                    _ListenHistory = new ListenHistoryRepository(Connector);
                }
                return _ListenHistory;
            }
        }
        public void Dispose()
        {
            Connector?.Dispose();
        }
    }
}
