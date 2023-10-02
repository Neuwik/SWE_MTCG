using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    public static class Package
    {
        const int PACKAGE_SIZE = 5;
        const int COST = 5;

        public static List<Card> GetGenericPackage()
        {
            List<Card> package = new List<Card>();
            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                if (true)
                {
                    package.Add(new Monster());
                }
                else
                {
                    package.Add(new Spell());

                }
            }
            return package;
        }

        public static List<Card> GetMonsterPackage()
        {
            List<Card> package = new List<Card>();
            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                package.Add(new Monster());
            }
            return package;
        }

        public static List<Card> GetSpellPackage()
        {
            List<Card> package = new List<Card>();
            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                package.Add(new Spell());
            }
            return package;
        }
    }
}
