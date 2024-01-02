using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.Model;

namespace MonsterTradingCardsGame
{
    public class UserRepo : IRepository<User>
    {
        private static UserRepo instance;
        private IDbConnection connection;

        private UserRepo()
        {
            connection = DBConnection.Instance.Connection;
            CreateUsersTableIfNotExists();
        }

        public static UserRepo Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserRepo();
                }
                return instance;
            }
        }

        public void CreateUsersTableIfNotExists()
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Users (ID SERIAL PRIMARY KEY, Username VARCHAR(255) UNIQUE, Password VARCHAR(255), Token VARCHAR(255) UNIQUE, Coins INT, Elo INT, Wins INT, Losses INT, MaxHP INT)";
                command.ExecuteNonQuery();
            }
        }

        public void DropUsersTable()
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "DROP TABLE IF EXISTS Users";
                command.ExecuteNonQuery();
                instance = null;
            }
        }

        public void Add(User user)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users (Username, Password, Token, Coins, Elo, Wins, Losses, MaxHP) VALUES (@Username, @Password, @Token, @Coins, @Elo, @Wins, @Losses, @MaxHP)";
                command.AddParameter("Username", user.Username);
                command.AddParameter("Password", user.Password);
                if (user.Token != null)
                {
                    command.AddParameter("Token", user.Token);
                }
                else
                {
                    command.AddParameter("Token", user.Username + "-mtcgToken");
                }
                command.AddParameter("Token", user.Token ?? "RandomToken");
                command.AddParameter("Coins", user.Coins);
                command.AddParameter("Elo", user.Elo);
                command.AddParameter("Wins", user.Wins);
                command.AddParameter("Losses", user.Losses);
                command.AddParameter("MaxHP", user.MaxHP);
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }

        public string GetTokenByUsernamePassword(string username, string password)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Token FROM Users WHERE Username = @Username AND  Password = @Password";
                command.AddParameter("Username", username);
                command.AddParameter("Password", password);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(reader.GetOrdinal("Token"));
                    }
                }
            }
            return null;
        }

        public bool UsernameExists(string username)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Username FROM Users WHERE Username = @Username";
                command.AddParameter("Username", username);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(reader.GetOrdinal("Username")) != null;
                    }
                }
            }
            return false;
        }
    }
}
