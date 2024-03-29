﻿using MonsterTradingCardsGame.Other;
using MonsterTradingCardsGame.Request_Handling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MonsterTradingCardsGame.Model
{
    public class User : IHealth
    {
        const int DECK_SIZE = 4;

        //Should not be safed
        [JsonIgnore]
        public readonly string Password;
        [JsonIgnore]
        public readonly bool IsAdmin = false;

        [JsonIgnore]
        public int ID { get; init; } = -1;
        public string Username { get; private set; }
        [JsonIgnore]
        public string Token { get; private set; }
        public string Bio { get; private set; } = "Hey, I am playing MTCG!";
        public string Image { get; private set; } = "°_°";
        public int Coins { get; private set; } = 20;
        public int Elo { get; private set; } = 200;
        public int Wins { get; private set; } = 0;
        public int Draws { get; private set; } = 0;
        public int Losses { get; private set; } = 0;

        [JsonIgnore]
        public List<Card> Cards { get; private set; }
        [JsonIgnore]
        public Card[] Deck { get; private set; }

        [JsonIgnore]
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

        //For Unit Test only
        public User(int id, string username, string password, string token)
        {
            ID = id;
            Username = username;
            Password = password;
            Token = token;
            HP = MaxHP;
            Deck = new Card[DECK_SIZE];
            Cards = new List<Card>();
        }

        //For Unit Test only
        public User(int id, string username, string password, string token, int elo) : this(id, username, password, token)
        {
            Elo = elo;
        }

        public User(int id, string username, string password, string token, bool isAdmin, string bio, string image, int coins, int elo, int wins, int draws, int losses, int maxHP) : this(username, password)
        {
            ID = id;
            Coins = coins;
            Elo = elo;
            Wins = wins;
            Draws = draws;
            Losses = losses;
            MaxHP = maxHP;
            HP = MaxHP;
            Username = username;
            Token = token;
            IsAdmin = isAdmin;
            Bio = bio;
            Image = image;
            Password = password;
            Cards = new List<Card>();
            Deck = new Card[DECK_SIZE];
        }

        public User(int id, string username, string password, string token, bool isAdmin, string bio, string image, int coins, int elo, int wins, int draws, int losses, int maxHP, List<Card> cards) : this(id, username, password, token, isAdmin, bio, image, coins, elo, wins, draws, losses, maxHP)
        {
            Cards = cards;
            ChangeDeck(cards.Where(c => c.InDeck).ToList());
        }

        public User(User user, List<Card> cards)
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
            ChangeDeck(cards.Where(c => c.InDeck).ToList());
        }

        public void AddNewCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                AddNewCard(card);
            }
        }

        private bool AddNewCard(Card card)
        {
            // Check Card already in Cards
            if (card.ID != -1 && Cards.Find(c => c.ID == card.ID) != null)
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

        private bool AddCardToDeck(Card card, int deckIndex = -1)
        {
            if (card == null)
            {
                return false;
            }

            // Check Owner
            if (card.UserID != ID)
            {
                Cards.Remove(card);
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
                return SwapCardPlaceInDeck(card, deckIndex);
            }

            if (deckIndex < 0)
            {
                // Fill Empty Slots first
                for (int i = 0; i < DECK_SIZE; i++)
                {
                    if (Deck[i] == null)
                    {
                        Deck[i] = card;
                        card.InDeck = true;
                        return true;
                    }
                }
                deckIndex = 0;
            }
            else if (deckIndex >=  DECK_SIZE)
            {
                deckIndex = DECK_SIZE - 1;
            }

            if (Deck[deckIndex] != null)
            {
                Deck[deckIndex].InDeck = false;
            }
            Deck[deckIndex] = card;
            card.InDeck = true;

            return true;
        }

        private bool SwapCardPlaceInDeck(Card card, int newIndex)
        {
            if(!card.InDeck)
            {
                return false;
            }

            if (newIndex < 0)
            {
                newIndex = 0;
            }

            if (newIndex >= DECK_SIZE)
            {
                newIndex = DECK_SIZE - 1;
            }

            if(Deck[newIndex] != null)
            {
                if(Deck[newIndex].ID == card.ID)
                {
                    return true;
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

        private bool ChangeDeck(List<Card> newDeck)
        {
            bool comethingChanged = false;

            for (int i = 0; i < newDeck.Count && i < DECK_SIZE; i++)
            {
                if(newDeck[i] == null)
                {
                    newDeck.RemoveAt(i);
                    i--;
                }
                if (Deck[i] == null || newDeck[i].ID != Deck[i].ID)
                {
                    if (AddCardToDeck(newDeck[i], i))
                    {
                        comethingChanged = true;
                    }
                    else
                    {
                        //Could not Change
                        newDeck.RemoveAt(i);
                        i--;
                    }
                }
            }
            return comethingChanged;
        }

        public bool ChangeDeck(List<int> newDeckIDs)
        {
            List<Card> newDeck = Cards.Where(c => newDeckIDs.Contains(c.ID)).ToList();
            return ChangeDeck(newDeck);
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

        public bool HasDeck()
        {
            return Deck.Count(c => c != null) == DECK_SIZE;
        }

        public int LooseHP(int dmg, EElementType elementType)
        {
            int realDMG = (dmg > HP) ? HP : dmg;
            HP -= realDMG;
            return realDMG;
        }

        public void ChangeUserData(string username, string bio, string image)
        {
            Username = username ?? Username;
            Bio = bio ?? Bio;
            Image = image ?? Image;
        }

        public string GetStatsAsJsonString()
        {
            string json = "{" + $"\"Username\":\"{Username}\",\"Elo\":\"{Elo}\",\"Wins\":\"{Wins}\",\"Draws\":\"{Draws}\",\"Losses\":\"{Losses}\"" + "}";
            return json;
        }

        public void AddWin(int enemyElo)
        {
            Wins++;

            float fElo = Convert.ToSingle(Elo);
            float fEnemyElo = Convert.ToSingle(enemyElo);
            if (fEnemyElo < 1)
            {
                fEnemyElo = 1;
            }
            if (fElo < 1)
            {
                fEnemyElo = 1;
            }
            float eloDifPercent = fEnemyElo / fElo;
            float eloChange = Battle.ELOCHANGE * eloDifPercent;
            Elo += ((int)(eloChange));
        }
        
        public void AddLoss(int enemyElo)
        {
            Losses++;

            float fElo = Convert.ToSingle(Elo);
            float fEnemyElo = Convert.ToSingle(enemyElo);
            if (fEnemyElo < 1)
            {
                fEnemyElo = 1;
            }
            if (fElo < 1)
            {
                fEnemyElo = 1;
            }
            float eloDifPercent = fElo / fEnemyElo;
            float eloChange = Battle.ELOCHANGE * eloDifPercent;
            Elo -= ((int)(eloChange));
            if (Elo < 0)
            {
                Elo = 0;
            }
        }
        
        public void AddDraw(int enemyElo)
        {
            Draws++;

            if (Elo == enemyElo)
            {
                return;
            }

            float fElo = Convert.ToSingle(Elo);
            float fEnemyElo = Convert.ToSingle(enemyElo);
            if (fEnemyElo < 1)
            {
                fEnemyElo = 1;
            }
            if (fElo < 1)
            {
                fEnemyElo = 1;
            }

            if (Elo < enemyElo)
            {
                float eloDifPercent = fEnemyElo / fElo;
                float eloChange = Battle.ELOCHANGE * (eloDifPercent - 1);
                Elo += ((int)(eloChange));
            }
            else if (Elo > enemyElo)
            {
                float eloDifPercent = fElo / fEnemyElo;
                float eloChange = Battle.ELOCHANGE * (eloDifPercent - 1);
                Elo -= ((int)(eloChange));
                if (Elo < 0)
                {
                    Elo = 0;
                }
            }
        }
    
        //Only used for Unit Tests
        public void ResetHP()
        {
            HP = MaxHP;
        }
    }
}
