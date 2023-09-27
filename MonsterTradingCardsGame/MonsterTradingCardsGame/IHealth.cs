using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    public interface IHealth
    {
        public  int MaxHP { get; init; }
        public int HP { get; protected set; }

        public void LooseHP(int dmg, EElementType elementType = EElementType.NORMAL);
    }
}
