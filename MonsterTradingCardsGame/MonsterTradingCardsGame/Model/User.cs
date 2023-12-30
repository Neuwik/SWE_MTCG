using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Model
{
    public class User : IHealth
    {
        const int DECK_SIZE = 4;

        //Should not be safed
        public readonly string Password;

        public string Username { get; private set; }
        public string Token { get; private set; }
        public int Coins { get; private set; } = 20;
        public List<Card> Cards { get; private set; }
        public Card[] Deck { get; private set; }

        public int HP { get; set; }
        public int MaxHP { get; init; } = 10;

        public User(string username, string password, string token)
        {
            Coins = 20;
            MaxHP = 10;
            HP = MaxHP;
            Username = username;
            Token = token;
            Password = password;
            Cards = new List<Card>();
            Deck = new Card[DECK_SIZE];
        }

        public Card TradeCard(Card card)
        {
            throw new NotImplementedException();
        }

        public bool AddCardToDeck(Card card)
        {
            if (Cards.Contains(card))
            {
                return false;
            }

            Cards.Add(card);
            return true;
        }

        public void LooseHP(int dmg, EElementType elementType)
        {
            HP -= dmg;
        }
    }
}
