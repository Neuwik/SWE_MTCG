using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Model
{
    public enum EPackageType { STARTER = 0, GENERIC = 1, MONSTER = 2, SPELL = 3 }
    public static class Package
    {
        const int PACKAGE_SIZE = 4;
        public const int COST = 5;
        private static Random random = new Random();

        public static List<Card> GetPackage(int packageTypeIndex = 1)
        {
            Array enumValues = Enum.GetValues(typeof(EPackageType));
            if (packageTypeIndex >= enumValues.Length || packageTypeIndex < 0)
            {
                packageTypeIndex = 1;
            }
            EPackageType elementType = (EPackageType)enumValues.GetValue(packageTypeIndex);
            return GetPackage(elementType);
        }

        public static List<Card> GetPackage(EPackageType packageType = EPackageType.GENERIC)
        {
            switch (packageType)
            {
                case EPackageType.STARTER:
                    return GetStarterPackage();
                    break;
                case EPackageType.GENERIC:
                    return GetGenericPackage();
                    break;
                case EPackageType.MONSTER:
                    return GetMonsterPackage();
                    break;
                case EPackageType.SPELL:
                    return GetSpellPackage();
                    break;
                default:
                    return GetGenericPackage();
                    break;
            }
        }

        public static List<Card> GetStarterPackage()
        {
            List<Card> package = new List<Card>();
            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                if (i < 3)
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

        public static List<Card> GetGenericPackage()
        {
            List<Card> package = new List<Card>();
            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                if (random.Next(2) > 0)
                {
                    package.Add(GetRandomMonster());
                }
                else
                {
                    package.Add(GetRandomSpell());

                }
            }
            return package;
        }

        public static List<Card> GetMonsterPackage()
        {
            List<Card> package = new List<Card>();
            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                package.Add(GetRandomMonster());
            }
            return package;
        }

        public static List<Card> GetSpellPackage()
        {
            List<Card> package = new List<Card>();
            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                package.Add(GetRandomSpell());
            }
            return package;
        }

        private static Monster GetRandomMonster()
        {
            Monster monster;

            Array enumValues = Enum.GetValues(typeof(EElementType));
            int randomIndex = random.Next(enumValues.Length);
            EElementType elementType = (EElementType)enumValues.GetValue(randomIndex);

            do
            {
                int dmg = random.Next(Card.MAXSTRENGTH) + 1;
                int maxHP = random.Next(Card.MAXSTRENGTH) + 1;
                monster = new Monster(dmg, elementType, maxHP);
            }
            while (monster.CalculateStrength() > Card.MAXSTRENGTH);
            return monster;
        }

        private static Spell GetRandomSpell()
        {
            Spell spell;

            Array enumValues = Enum.GetValues(typeof(EElementType));
            int randomIndex = random.Next(enumValues.Length);
            EElementType elementType = (EElementType)enumValues.GetValue(randomIndex);

            do
            {
                int dmg = random.Next(Card.MAXSTRENGTH) + 1;
                int maxUses = random.Next(Card.MAXSTRENGTH) + 1;
                spell = new Spell(dmg, elementType, maxUses);
            }
            while (spell.CalculateStrength() > Card.MAXSTRENGTH);
            return spell;
        }
    }
}
