using SpotifyListener.DatabaseManager.Models;
using System.Data.SQLite;
using System.Linq;
using Utilities.SQL;

namespace SpotifyListener.DatabaseManager
{
    public sealed class SQLite : DatabaseConnector<SQLiteConnection, SQLiteParameter>
    {
        private static string FilePath = "listener.db";
        private static string dbpath = $"Data Source={FilePath};Version=3";
        public SQLite() : base(dbpath)
        {
            Initialization();
        }
        public SQLite(string connectionString) : base(connectionString)
        {
            Initialization();
        }
        private void Initialization()
        {
            if (!IsTableExists("ListenHistory"))
            {
                this.CreateTable<ListenHistory>();
            }
        }
        private bool IsTableExists(string tableName)
        {
            return ExecuteReader($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';").Any();
        }

    }
}
