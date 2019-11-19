using SpotifyListener.DatabaseManager.Models;
using System;
using System.Data;
using System.Data.SQLite;
using Utilities.DesignPattern.UnitOfWork.Components;
using Utilities.Interfaces;

namespace SpotifyListener.DatabaseManager
{
    class SQLiteService : IUnitOfWork
    {
        private static Lazy<SQLiteService> Instant = new Lazy<SQLiteService>(() => new SQLiteService(), true);
        public static SQLiteService Context => Instant.Value;
        readonly IDatabaseConnectorExtension<SQLiteConnection, SQLiteParameter> Connector;
        SQLiteService()
        {
            Connector = new SQLite();
        }
        private Repository<ListenHistory, SQLiteConnection, SQLiteParameter> _listenHistories;
        public Repository<ListenHistory, SQLiteConnection, SQLiteParameter> ListenHistories
        {
            get
            {
                if (_listenHistories is null)
                {
                    _listenHistories = new Repository<ListenHistory, SQLiteConnection, SQLiteParameter>(Connector);
                }
                return _listenHistories;
            }
        }
        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void RollbackChanges(IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges(IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
