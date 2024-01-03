﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Users WHERE ID = @ID";
                command.AddParameter("ID", id);
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<User> GetAll()
        {
            List<User> users = new List<User>();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users";

                using (IDataReader reader = command.ExecuteReader())
                {
                    User user;
                    while ((user = ReadUserFromReader(reader)) != null)
                    {
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public User GetByID(int id)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users WHERE ID = @ID";
                command.AddParameter("ID", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    return ReadUserFromReader(reader);
                }
            }
        }

        public void Update(User user)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Users 
                                        SET Username = @Username, 
                                            Password = @Password, 
                                            Token = @Token, 
                                            Coins = @Coins, 
                                            Elo = @Elo, 
                                            Wins = @Wins, 
                                            Losses = @Losses, 
                                            MaxHP = @MaxHP 
                                        WHERE ID = @ID";

                command.AddParameter("@ID", user.ID);
                command.AddParameter("@Username", user.Username);
                command.AddParameter("@Password", user.Password);
                command.AddParameter("@Token", user.Token);
                command.AddParameter("@Coins", user.Coins);
                command.AddParameter("@Elo", user.Elo);
                command.AddParameter("@Wins", user.Wins);
                command.AddParameter("@Losses", user.Losses);
                command.AddParameter("@MaxHP", user.MaxHP);
                command.ExecuteNonQuery();
            }
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

        public User GetByUsernamePassword(string username, string password)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users WHERE Username = @Username AND  Password = @Password";
                command.AddParameter("Username", username);
                command.AddParameter("Password", password);

                using (IDataReader reader = command.ExecuteReader())
                {
                    return ReadUserFromReader(reader);
                }
            }
            return null;
        }

        public User GetByToken(string token)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users WHERE Token = @Token";
                command.AddParameter("Token", token);

                using (IDataReader reader = command.ExecuteReader())
                {
                    return ReadUserFromReader(reader);
                }
            }
            return null;
        }

        private User ReadUserFromReader(IDataReader reader)
        {
            if (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("ID"));
                string username = reader.GetString(reader.GetOrdinal("Username"));
                string password = reader.GetString(reader.GetOrdinal("Password"));
                string token = reader.GetString(reader.GetOrdinal("Token"));
                int coins = reader.GetInt32(reader.GetOrdinal("Coins"));
                int elo = reader.GetInt32(reader.GetOrdinal("Elo"));
                int wins = reader.GetInt32(reader.GetOrdinal("Wins"));
                int losses = reader.GetInt32(reader.GetOrdinal("Losses"));
                int maxHP = reader.GetInt32(reader.GetOrdinal("MaxHP"));

                return new User(id, username, password, token, coins, elo, wins, losses, maxHP);
            }
            return null;
        }
    }
}
