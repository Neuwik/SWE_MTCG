using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

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
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Cards (ID SERIAL PRIMARY KEY, Name VARCHAR(255), DMG INT, ElementType INT, HP INT, MaxHP INT)";
                command.ExecuteNonQuery();
            }
        }

        public void Add(Card card)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Cards (Name, DMG, ElementType, HP, MaxHP) VALUES (@Name, @DMG, @ElementType, @HP, @MaxHP)";
                command.AddParameter("Name", card.Name);
                command.AddParameter("DMG", card.DMG);
                command.AddParameter("ElementType", (int)card.ElementType);

                if (card is Monster monster)
                {
                    command.AddParameter("HP", monster.HP);
                    command.AddParameter("MaxHP", monster.MaxHP);
                }
                else
                {
                    command.AddParameter("HP", DBNull.Value);
                    command.AddParameter("MaxHP", DBNull.Value);
                }

                command.ExecuteNonQuery();
            }
        }

        public Card GetById(int id)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Cards WHERE ID = @ID";
                command.AddParameter("ID", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader.GetString(reader.GetOrdinal("Name"));
                        int dmg = reader.GetInt32(reader.GetOrdinal("DMG"));
                        EElementType elementType = (EElementType)reader.GetInt32(reader.GetOrdinal("ElementType"));
                        int hp = reader.GetInt32(reader.GetOrdinal("HP"));
                        int maxHP = reader.GetInt32(reader.GetOrdinal("MaxHP"));

                        if (maxHP == null || maxHP == 0)
                        {
                            return new Spell(id, name, dmg, elementType);
                        }

                        return new Monster(id, name, dmg, elementType, hp, maxHP);
                    }
                    return null;
                }
            }
        }

        public void Update(Card card)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Cards SET Name = @Name, DMG = @DMG, ElementType = @ElementType WHERE ID = @ID";
                command.AddParameter("ID", card.ID);
                command.AddParameter("Name", card.Name);
                command.AddParameter("DMG", card.DMG);
                command.AddParameter("ElementType", (int)card.ElementType);
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Cards WHERE ID = @ID";
                command.AddParameter("ID", id);
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<Card> GetAll()
        {
            List<Card> cards = new List<Card>();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Cards";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("ID"));
                        string name = reader.GetString(reader.GetOrdinal("Name"));
                        int dmg = reader.GetInt32(reader.GetOrdinal("DMG"));
                        EElementType elementType = (EElementType)reader.GetInt32(reader.GetOrdinal("ElementType"));
                        int hp = reader.GetInt32(reader.GetOrdinal("HP"));
                        int maxHP = reader.GetInt32(reader.GetOrdinal("MaxHP"));

                        if (maxHP == null || maxHP == 0)
                        {
                            cards.Add(new Spell(id, name, dmg, elementType));
                        }

                        cards.Add(new Monster(id, name, dmg, elementType, hp, maxHP));
                    }
                }
            }

            return cards;
        }
        public void DropCardsTable()
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "DROP TABLE IF EXISTS Cards";
                command.ExecuteNonQuery();
                instance = null;
            }
        }
    }
}
