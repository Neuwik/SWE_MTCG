﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    public class Monster : Card, IHealth
    {
        public int HP { get; set; }
        public int MaxHP { get; init; }

        public Monster() : base()
        {
            //Random Stats
        }

        public Monster(string name, int dmg, EElementType elementType) : base(name, dmg, elementType)
        {
            MaxHP = 5;
            HP = MaxHP;
        }

        public Monster(string name, int dmg, EElementType elementType, int maxHP) : base(name, dmg, elementType)
        {
            MaxHP = maxHP;
            HP = MaxHP;
        }

        public Monster(int id, string name, int dmg, EElementType elementType, int HP, int maxHP) : base(id, name, dmg, elementType)
        {
            MaxHP = maxHP;
            HP = MaxHP;
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
            else if(elementDifference == 1 || elementDifference == -4)
            {
                HP -= (int)(dmg * 0.8);
            }
            else if(elementDifference == -1 || elementDifference == 4)
            {
                HP -= (int)(dmg * 1.2);
            }
            else
            {
                HP -= dmg;
            }
        }
    }
}
