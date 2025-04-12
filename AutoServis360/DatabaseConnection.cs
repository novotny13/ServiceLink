using System;
using System.Configuration;
using System.Data.SqlClient;

namespace AutoServis360
{
    public class DatabaseConnection
    {
        private static readonly Lazy<DatabaseConnection> _instance =
            new Lazy<DatabaseConnection>(() => new DatabaseConnection());

        private readonly string _connectionString;

        private DatabaseConnection()
        {
            // Načtení připojovacího řetězce z App.config
            _connectionString = ConfigurationManager.ConnectionStrings["AutoServis360Database"].ConnectionString;
        }

        public static DatabaseConnection GetInstance() => _instance.Value;

        // Vytvoří a vrátí nové připojení k databázi
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}