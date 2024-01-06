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
        protected static List<string> Names = new List<string>() { "Pawn", "Rook", "Kight", "Bishop", "Queen", "King" };
        public static readonly int MAXSTRENGTH = Names.Count * 3;

        public int ID { get; init; } = -1;
        public int DMG { get; init; } = 1;
        public bool InDeck { get; set; } = false;
        public int UserID { get; private set; }

        public string Name { get; protected set; }
        public EElementType ElementType { get; protected set; } = EElementType.NORMAL;

        public Card()
        {
            Name = GenerateCardName();
        }

        public Card(int dmg, EElementType elementType)
        {
            DMG = dmg;
            ElementType = elementType;
            Name = GenerateCardName();
        }

        public Card(string name, int dmg, EElementType elementType, int userID)
        {
            //Add new Card to db
            Name = name;
            DMG = dmg;
            ElementType = elementType;
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

        public int Attack(IHealth target)
        {
            return target.LooseHP(DMG, ElementType);
        }

        public void TradeCard(int userID)
        {
            UserID = userID;
            //Console.WriteLine(UserID);
            ResetStats();
        }

        protected string GenerateCardName()
        {
            int i = CalculateStrength() / (MAXSTRENGTH / Names.Count);
            if (i >= Names.Count)
            {
                i = Names.Count - 1;
            }
            return ElementType.ToString() + " " + Names[i] + " " + this.GetType().Name;
        }

        public override string ToString()
        {
            return $"{Name} | Element Type: {ElementType.ToString()} | DMG: {DMG}";
        }

        public abstract void ResetStats();

        public abstract int CalculateStrength();
    }
}
