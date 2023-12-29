using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.Model;

namespace MonsterTradingCardsGame
{
    public class UserRepo : IRepository<Card>
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
                command.CommandText = "CREATE TABLE IF NOT EXISTS Users (ID SERIAL PRIMARY KEY, Username VARCHAR(255) UNIQUE, Password VARCHAR(255), Token VARCHAR(255) UNIQUE, Coins INT, HP INT, MaxHP INT)";
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

        public void Add(Card entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetAll()
        {
            throw new NotImplementedException();
        }

        public Card GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Card entity)
        {
            throw new NotImplementedException();
        }
    }
}
