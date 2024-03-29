﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MonsterTradingCardsGame.Model;

namespace MonsterTradingCardsGame
{
    public class CardRepo : IRepository<Card>
    {
        private static CardRepo instance;
        private IDbConnection connection;

        private CardRepo()
        {
            connection = DBConnection.Instance.Connection;
            CreateCardsTableIfNotExists();
        }

        public static CardRepo Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CardRepo();
                }
                return instance;
            }
        }

        public void CreateCardsTableIfNotExists()
        {
            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Cards (ID SERIAL PRIMARY KEY, Name VARCHAR(255), DMG INT, ElementType INT, MaxHP INT, MaxUses INT, InDeck BOOLEAN, UserID INT REFERENCES Users(ID))";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DropCardsTable()
        {
            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DROP TABLE IF EXISTS Cards";
                    command.ExecuteNonQuery();
                    instance = null;
                }
            }
        }

        public void Add(Card card)
        {
            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Cards (Name, DMG, ElementType, MaxHP, MaxUses, InDeck, UserID) VALUES (@Name, @DMG, @ElementType, @MaxHP, @MaxUses, @InDeck, @UserID)";
                    command.AddParameter("Name", card.Name);
                    command.AddParameter("DMG", card.DMG);
                    command.AddParameter("ElementType", (int)card.ElementType);
                    command.AddParameter("InDeck", card.InDeck);
                    command.AddParameter("UserID", card.UserID);

                    if (card is Monster monster)
                    {
                        command.AddParameter("MaxHP", monster.MaxHP);
                        command.AddParameter("MaxUses", 0);
                    }
                    else if (card is Spell spell)
                    {
                        command.AddParameter("MaxHP", 0);
                        command.AddParameter("MaxUses", spell.MaxUses);
                    }
                    else
                    {
                        throw new Exception("CardRepo: Card Type does not exist.");
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public Card GetByID(int id)
        {
            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Cards WHERE ID = @ID";
                    command.AddParameter("ID", id);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        return ReadCardFromReader(reader);
                    }
                }
            }
        }

        public void Update(Card card)
        {
            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    if (card is Monster monster)
                    {
                        command.CommandText = "UPDATE Cards SET Name = @Name, DMG = @DMG, ElementType = @ElementType, MaxHP = @MaxHP, InDeck = @InDeck, UserID = @UserID WHERE ID = @ID";
                        command.AddParameter("MaxHP", monster.MaxHP);
                    }
                    else if (card is Spell spell)
                    {
                        command.CommandText = "UPDATE Cards SET Name = @Name, DMG = @DMG, ElementType = @ElementType, MaxUses = @MaxUses, InDeck = @InDeck, UserID = @UserID WHERE ID = @ID";
                        command.AddParameter("MaxUses", spell.MaxUses);
                    }
                    else
                    {
                        throw new Exception("CardRepo: Card Type does not exist.");
                    }
                    command.AddParameter("ID", card.ID);
                    command.AddParameter("Name", card.Name);
                    command.AddParameter("DMG", card.DMG);
                    command.AddParameter("ElementType", (int)card.ElementType);
                    command.AddParameter("InDeck", card.InDeck);
                    command.AddParameter("UserID", card.UserID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Cards WHERE ID = @ID";
                    command.AddParameter("ID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Card> GetAll()
        {
            List<Card> cards = new List<Card>();

            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Cards";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        Card card;
                        while ((card = ReadCardFromReader(reader)) != null)
                        {
                            cards.Add(card);
                        }
                    }
                }
            }

            return cards;
        }

        public List<Card> GetCardsOfUser(int userID)
        {
            List<Card> cards = new List<Card>();

            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Cards WHERE UserID = @UserID";
                    command.AddParameter("UserID", userID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        Card card;
                        while ((card = ReadCardFromReader(reader)) != null)
                        {
                            cards.Add(card);
                        }
                    }
                }
            }

            return cards;
        }

        public List<Card> GetDeckOfUser(int userID)
        {
            List<Card> cards = new List<Card>();

            lock (connection)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Cards WHERE UserID = @UserID AND InDeck = TRUE";
                    command.AddParameter("UserID", userID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        Card card;
                        while ((card = ReadCardFromReader(reader)) != null)
                        {
                            cards.Add(card);
                        }
                    }
                }
            }

            return cards;
        }

        private Card ReadCardFromReader(IDataReader reader)
        {
            if (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("ID"));
                string name = reader.GetString(reader.GetOrdinal("Name"));
                int dmg = reader.GetInt32(reader.GetOrdinal("DMG"));
                EElementType elementType = (EElementType)reader.GetInt32(reader.GetOrdinal("ElementType"));
                int maxHP = reader.GetInt32(reader.GetOrdinal("MaxHP"));
                int maxUses = reader.GetInt32(reader.GetOrdinal("MaxUses"));
                bool inDeck = reader.GetBoolean(reader.GetOrdinal("InDeck"));
                int userID = reader.GetInt32(reader.GetOrdinal("UserID"));

                if (maxHP != null && maxHP != 0)
                {
                    return new Monster(id, name, dmg, elementType, maxHP, inDeck, userID);
                }
                else if (maxUses != null && maxUses != 0)
                {
                    return new Spell(id, name, dmg, elementType, maxUses, inDeck, userID);
                }
            }
            return null;
        }
    }
}
