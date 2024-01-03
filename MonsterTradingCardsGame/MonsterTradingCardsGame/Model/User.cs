using MonsterTradingCardsGame.Request_Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Model
{
    public class User : IHealth
    {
        const int DECK_SIZE = 4;

        //Should not be safed
        public readonly string Password;

        public int ID { get; init; }
        public string Username { get; private set; }
        public string Token { get; private set; }
        public int Coins { get; private set; } = 20;
        public int Elo { get; private set; } = 200;
        public int Wins { get; private set; } = 0;
        public int Losses { get; private set; } = 0;
        public List<Card> Cards { get; private set; }
        public Card[] Deck { get; private set; }

        public int HP { get; set; }
        public int MaxHP { get; init; } = 20;

        public User(string username, string password)
        {
            if(username == null || password == null)
            {
                throw new ArgumentNullException("USER CONSTRUCTOR: username || password");
            }
            HP = MaxHP;
            Username = username;
            Password = password;
            Cards = new List<Card>();
            Deck = new Card[DECK_SIZE];
        }

        public User(int id, string username, string password, string token, int coins, int elo, int wins, int losses, int maxHP)
        {
            ID = id;
            Coins = coins;
            Elo = elo;
            Wins = wins;
            Losses = losses;
            MaxHP = maxHP;
            HP = MaxHP;
            Username = username;
            Token = token;
            Password = password;
            Cards = new List<Card>();
            Deck = new Card[DECK_SIZE];
        }

        public User(int id, string username, string password, string token, int coins, int elo, int wins, int losses, int maxHP, List<Card> cards)
        {
            ID = id;
            Coins = coins;
            Elo = elo;
            Wins = wins;
            Losses = losses;
            MaxHP = maxHP;
            HP = MaxHP;
            Username = username;
            Token = token;
            Password = password;
            Cards = cards;
            Deck = new Card[DECK_SIZE];
            ChangeDeck(Cards);
        }

        public User(int id, string username, string password, string token, int coins, int elo, int wins, int losses, int maxHP, List<Card> cards, List<Card> deck)
        {
            ID = id;
            Coins = coins;
            Elo = elo;
            Wins = wins;
            Losses = losses;
            MaxHP = maxHP;
            HP = MaxHP;
            Username = username;
            Token = token;
            Password = password;
            Cards = cards;
            Deck = new Card[DECK_SIZE];
            ChangeDeck(deck);
        }

        public User(User user, List<Card> cards, List<Card> deck = null)
        {
            ID = user.ID;
            Coins = user.Coins;
            Elo = user.Elo;
            Wins = user.Wins;
            Losses = user.Losses;
            MaxHP = user.MaxHP;
            HP = MaxHP;
            Username = user.Username;
            Token = user.Token;
            Password = user.Password;
            Cards = cards;
            Deck = new Card[DECK_SIZE];
            if (deck == null)
            {
                ChangeDeck(cards);
            }
            else
            {
                ChangeDeck(deck);
            }
        }

        public Card TradeCard(Card card)
        {
            throw new NotImplementedException();
        }

        public void AddNewCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                AddNewCard(card);
            }
        }

        public bool AddNewCard(Card card)
        {
            // Check Card already in Cards
            if (Cards.Contains(card))
            {
                return false;
            }

            card.TradeCard(ID);
            Cards.Add(card);

            // Fill Empty Deck Slots
            if (DECK_SIZE >= Cards.Count)
            {
                AddCardToDeck(card);
            }

            return true;
        }

        public bool AddCardToDeck(Card card, int index = 0)
        {
            // Check Owner
            if (card.UserID != ID)
            {
                if (Cards.Contains(card))
                {
                    Cards.Remove(card);
                }
                return false;
            }
            // Check Card in Cards
            if (!Cards.Contains(card))
            {
                return false;
            }

            // Swap Cards in Deck instead
            if (card.InDeck)
            {
                return SwapCardPlaceInDeck(card, index);
            }

            // Fill Empty Slots first
            if (DECK_SIZE >= Cards.Count)
            {
                for (int i = 0; i < DECK_SIZE; i++)
                {
                    if (Deck[i] == null)
                    {
                        Deck[i] = card;
                        card.InDeck = true;
                        return true;
                    }
                }
            }

            if (index >=  DECK_SIZE)
            {
                index = DECK_SIZE - 1;
            }
            if (Deck[index] != null)
            {
                Deck[index].InDeck = false;
            }
            Deck[index] = card;
            card.InDeck = true;

            return true;
        }

        public bool SwapCardPlaceInDeck(Card card, int newIndex)
        {
            if(!card.InDeck)
            {
                return false;
            }

            if(newIndex >= DECK_SIZE)
            {
                return false;
            }

            if(Deck[newIndex] != null)
            {
                if(Deck[newIndex].ID == card.ID)
                {
                    return false;
                }
                int currentIndex = -1;
                for (int i = 0; i < DECK_SIZE; i++)
                {
                    if (Deck[i] != null && Deck[i].ID == card.ID)
                    {
                        currentIndex = i;
                        break;
                    }
                }
                if (currentIndex < 0)
                {
                    return false;
                }
                Deck[currentIndex] = Deck[newIndex];
                Deck[newIndex] = card;
            }
            else
            {
                Deck[newIndex] = card;
            }

            return true;
        }

        public bool ChangeDeck(List<Card> newDeck)
        {
            bool allWorked = true;
            for (int i = 0; i < newDeck.Count && i < DECK_SIZE; i++)
            {
                if (!AddCardToDeck(newDeck[i]))
                {
                    allWorked = false;
                    newDeck.RemoveAt(i);
                    i--;
                }
            }
            return allWorked;
        }

        public List<Card> BuyPackage(EPackageType packageType = EPackageType.GENERIC)
        {
            if (Coins < Package.COST)
            {
                return null;
            }

            Coins -= Package.COST;
            List<Card> newCards = Package.GetPackage(packageType);
            AddNewCards(newCards);
            return newCards;
        }

        public void LooseHP(int dmg, EElementType elementType)
        {
            HP -= dmg;
        }
    }
}
