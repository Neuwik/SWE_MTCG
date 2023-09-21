using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    public class Package
    {
        const int PACKAGE_SIZE = 5;
        const int COST = 5;
        Card[] cards;

        public Package() 
        {
            cards = new Card[PACKAGE_SIZE];
        }
    }
}
