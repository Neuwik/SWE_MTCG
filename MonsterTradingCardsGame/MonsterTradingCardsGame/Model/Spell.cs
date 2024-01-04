using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Model
{
    public class Spell : Card
    {
        public int MaxUses { get; init; } = 1;
        public int Uses { get; protected set; }


        public Spell() : base()
        {
            Uses = MaxUses;
        }

        public Spell(int dmg, EElementType elementType, int maxUses) : base(dmg, elementType)
        {
            MaxUses = maxUses;
            Uses = MaxUses;
        }

        public Spell(string name, int dmg, EElementType elementType, int maxUses, int userID) : base(name, dmg, elementType, userID)
        {
            MaxUses = maxUses;
            Uses = MaxUses;
        }

        public Spell(int id, string name, int dmg, EElementType elementType, int maxUses, bool inDeck, int userID) : base(id, name, dmg, elementType, inDeck, userID)
        {
            MaxUses = maxUses;
            Uses = MaxUses;
        }

        public Spell(int id, string name, int dmg, EElementType elementType, int uses, int maxUses, bool inDeck, int userID) : base(id, name, dmg, elementType, inDeck, userID)
        {
            MaxUses = maxUses;
            Uses = uses;
        }

        public override void PlayCard()
        {
            Uses--;
            Console.WriteLine($"The Spell {Name} (DMG: {DMG}, ElementType: {ElementType}) was played. ({Uses}/{MaxUses} Uses left)");
        }

        public override string ToString()
        {
            return base.ToString() + $" | Uses: {MaxUses}";
        }

        public override void ResetStats()
        {
            Uses = MaxUses;
        }

        public override int CalculateStrength()
        {
            return MaxUses * DMG;
        }
    }
}
