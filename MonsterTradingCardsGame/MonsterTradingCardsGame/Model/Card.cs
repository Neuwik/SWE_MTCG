using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Model
{
    public enum EElementType { NORMAL = 0, WATER = 1, LIGHTNING = 2, WIND = 3, FIRE = 4, EARTH = 5 }

    public abstract class Card
    {
        public readonly int ID;
        public readonly int DMG;
        public bool InDeck { get; set; }
        public int UserID { get; private set; }

        public string Name { get; private set; }
        public EElementType ElementType { get; private set; }

        public Card()
        {
            //Random Stats
        }

        public Card(string name, int dmg, EElementType elementType, int userID)
        {
            //Add new Card to db
            Name = name;
            DMG = dmg;
            ElementType = elementType;
            InDeck = false;
            UserID = userID;
        }

        public Card(int id, string name, int dmg, EElementType elementType, bool inDeck, int userID)
        {
            ID = id;
            Name = name;
            DMG = dmg;
            ElementType = elementType;
            InDeck = inDeck;
            UserID = userID;
        }

        public virtual void PlayCard()
        {
            Console.WriteLine($"The Card {Name} (DMG: {DMG}, ElementType: {ElementType}) was played.");
        }

        public void Attack(IHealth target)
        {
            target.LooseHP(DMG, ElementType);
        }

        public void TradeCard(int userID)
        {
            UserID = userID;
            ResetStats();
        }

        public abstract void ResetStats();
    }
}
