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
        public int MaxHP { get; init; } = 1;

        public Monster() : base()
        {
            HP = MaxHP;
        }

        public Monster(int dmg, EElementType elementType, int maxHP) : base(dmg, elementType)
        {
            MaxHP = maxHP;
            HP = MaxHP;
        }

        public Monster(int id, string name, int dmg, EElementType elementType, int maxHP, bool inDeck, int userID) : base(id, name, dmg, elementType, inDeck, userID)
        {
            MaxHP = maxHP;
            HP = MaxHP;
        }

        public int LooseHP(int dmg, EElementType elementType)
        {
            int elementDifference = ElementType - elementType;
            int realDMG;

            if (ElementType == EElementType.NORMAL || elementType == EElementType.NORMAL)
            {
                realDMG = dmg;
            }
            else if (elementDifference == 1 || elementDifference == -4)
            {
                realDMG = (int)(dmg * 0.8);
            }
            else if (elementDifference == -1 || elementDifference == 4)
            {
                realDMG = (int)(dmg * 1.2);
            }
            else
            {
                realDMG = dmg;
            }

            realDMG = (realDMG > HP) ? HP : realDMG;

            HP -= realDMG;

            return realDMG;
        }

        public override string ToString()
        {
            return base.ToString() + $" | HP: {MaxHP}";
        }

        public override void ResetStats()
        {
            HP = MaxHP;
        }

        public override int CalculateStrength()
        {
            return MaxHP + DMG;
        }
    }
}
