using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace MonsterTradingCardsGame
{
    internal class DBConnection
    {
        //docker run --name SWE_MTCG -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=debian123! -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres

        private const int PORT = 5432;
        private const string DBNAME = "SWE_MTCG";
        private const string USERNAME = "postgres";
        private const string PASSWORD = "debian123!";
        private static DBConnection instance;
        private IDbConnection connection;
        private string connectionString;

        private DBConnection()
        {
            connectionString = $"Server=localhost;Port={PORT};Database={DBNAME};User ID={USERNAME};Password={PASSWORD};";
            connection = new NpgsqlConnection(connectionString);
        }

        public static DBConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBConnection();
                }
                return instance;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                if (connection.State != ConnectionState.Open)
                {
                    CreateDatabaseIfNotExists();
                    connection.Open();
                }
                return connection;
            }
        }

        private void CreateDatabaseIfNotExists()
        {
            bool dbExists = false;
            string postgresConnectionString = $"Host=localhost;Username={USERNAME};Password={PASSWORD};Database=postgres";
            using var postgresConnection = new NpgsqlConnection(postgresConnectionString);
            postgresConnection.Open();

            using (IDbCommand command = postgresConnection.CreateCommand())
            {
                command.CommandText = $"SELECT EXISTS(SELECT datname FROM pg_catalog.pg_database WHERE datname = '{DBNAME}')";
                dbExists = (bool)command.ExecuteScalar();
            }
            if(!dbExists)
            {
                using (IDbCommand command = postgresConnection.CreateCommand())
                {
                    command.CommandText = $"CREATE DATABASE \"{DBNAME}\"";
                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Database '{DBNAME}' created successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating database: {ex.Message}");
                    }
                }
            }
            postgresConnection.Close();
        }
    }
}
