using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Model
{
    public class Monster : Card, IHealth
    {
        public int HP { get; set; }
        public int MaxHP { get; init; }

        public Monster() : base()
        {
            //Random Stats
        }

        public Monster(string name, int dmg, EElementType elementType, int maxHP, int userID) : base(name, dmg, elementType, userID)
        {
            MaxHP = maxHP;
            HP = MaxHP;
        }

        public Monster(int id, string name, int dmg, EElementType elementType, int maxHP, bool inDeck, int userID) : base(id, name, dmg, elementType, inDeck, userID)
        {
            MaxHP = maxHP;
            HP = MaxHP;
        }

        public Monster(int id, string name, int dmg, EElementType elementType, int hp, int maxHP, bool inDeck, int userID) : base(id, name, dmg, elementType, inDeck, userID)
        {
            MaxHP = maxHP;
            HP = hp;
        }

        public override void PlayCard()
        {
            Console.WriteLine($"The Monster {Name} (HP: {HP}, DMG: {DMG}, ElementType: {ElementType}) was played.");
        }

        public void LooseHP(int dmg, EElementType elementType)
        {
            int elementDifference = ElementType - elementType;

            if (ElementType == EElementType.NORMAL || elementType == EElementType.NORMAL)
            {
                HP -= dmg;
            }
            else if (elementDifference == 1 || elementDifference == -4)
            {
                HP -= (int)(dmg * 0.8);
            }
            else if (elementDifference == -1 || elementDifference == 4)
            {
                HP -= (int)(dmg * 1.2);
            }
            else
            {
                HP -= dmg;
            }
        }

        public override void ResetStats()
        {
            HP = MaxHP;
        }
    }
}
