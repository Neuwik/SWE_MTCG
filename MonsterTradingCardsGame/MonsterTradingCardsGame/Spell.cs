using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    public class Spell : Card
    {
        public Spell() : base()
        {
            //Random Stats
        }

        public Spell(string name, int dmg, EElementType elementType) : base(name, dmg, elementType)
        {

        }

        public override void PlayCard()
        {
            Console.WriteLine($"The Spell {Name} (DMG: {DMG}, ElementType: {ElementType}) was played.");
        }
    }
}
