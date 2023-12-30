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
                command.CommandText = "CREATE TABLE IF NOT EXISTS Users (ID SERIAL PRIMARY KEY, Username VARCHAR(255) UNIQUE, Password VARCHAR(255), Token VARCHAR(255) UNIQUE, Coins INT, MaxHP INT)";
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
                command.CommandText = "INSERT INTO Users (Username, Password, Token, Coins, MaxHP) VALUES (@Username, @Password, @Token, @Coins, @MaxHP)";
                command.AddParameter("Username", user.Username);
                command.AddParameter("Password", user.Password);
                command.AddParameter("Token", user.Token);
                command.AddParameter("Coins", user.Coins);
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
    }
}
