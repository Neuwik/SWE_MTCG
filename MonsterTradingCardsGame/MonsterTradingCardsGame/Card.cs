using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    public enum EElementType { NORMAL = 0, WATER = 1, LIGHTNING = 2, WIND = 3, FIRE = 4, EARTH = 5 }

    public abstract class Card
    {
        public readonly int DMG;

        public string Name { get; private set; }
        public EElementType ElementType { get; private set; }

        public Card(string name, int dmg, EElementType elementType)
        {
            Name = name;
            DMG = dmg;
            ElementType = elementType;
        }

        public virtual void PlayCard()
        {
            Console.WriteLine($"The Card {Name} (DMG: {DMG}, ElementType: {ElementType}) was played.");
        }

        public void Attack(IHealth target)
        {
            target.LooseHP(DMG, ElementType);
        }
    }
}
