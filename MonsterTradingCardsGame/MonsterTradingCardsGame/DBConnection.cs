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
        private const int PORT = 5432;
        private const string DBNAME = "SWE_MTCG";
        private const string USERNAME = "postgres";
        private const string PASSWORD = "debian123!";
        private static DBConnection instance;
        private IDbConnection connection;

        private DBConnection()
        {
            string connectionString = $"Server=localhost;Port={PORT};Database={DBNAME};User Id={USERNAME};Password={PASSWORD};";
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
                    connection.Open();
                }
                return connection;
            }
        }
    }
}
