using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Lab4_ADONET_SQLite
{
    /// <summary>
    /// ADO.NET operations required by LAB4:
    /// connect, disconnect, insert a keyword, and list keywords.
    /// </summary>
    public sealed class SQLiteHandler
    {
        private const string ConnectionString =
            "Data Source=keywords.db;Version=3;";

        private SQLiteConnection connection;

        public void ConnectToDb()
        {
            if (connection == null)
                connection = new SQLiteConnection(ConnectionString);

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    @"CREATE TABLE IF NOT EXISTS Keywords
                      (
                          id INTEGER PRIMARY KEY AUTOINCREMENT,
                          keyword TEXT
                      );";

                command.ExecuteNonQuery();
            }
        }

        public void DisconnectFromDb()
        {
            if (connection != null &&
                connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public void InsertKeyword(string keyword)
        {
            EnsureConnected();

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "INSERT INTO Keywords (keyword) VALUES (@keyword);";

                command.Parameters.AddWithValue("@keyword", keyword);
                command.ExecuteNonQuery();
            }
        }

        public List<string> GetAllKeywords()
        {
            EnsureConnected();

            List<string> result = new List<string>();

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT keyword FROM Keywords ORDER BY id;";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            result.Add(reader.GetString(0));
                    }
                }
            }

            return result;
        }

        private void EnsureConnected()
        {
            if (connection == null ||
                connection.State != ConnectionState.Open)
            {
                ConnectToDb();
            }
        }

        public void Dispose()
        {
            DisconnectFromDb();

            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }
    }
}
