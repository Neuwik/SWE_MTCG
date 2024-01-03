using System.Net;
using MonsterTradingCardsGame.Model;

namespace MonsterTradingCardsGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ResetDB();
            StartServer();
        }

        static void StartServer()
        {
            MTCG_Server server = MTCG_Server.Instance;
            try
            {
                server.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }

        static void ResetDB()
        {
            CardRepo.Instance.DropCardsTable();
            UserRepo.Instance.DropUsersTable();
        }

        /*static void TestCardCreation()
        {
            List<Card> cards = Package.GetStarterPackage();
            cards.ForEach(x => { Console.WriteLine(x.Name); });

            cards = Package.GetGenericPackage();
            cards.ForEach(x => { Console.WriteLine(x.Name); });

            cards = Package.GetMonsterPackage();
            cards.ForEach(x => { Console.WriteLine(x.Name); });

            cards = Package.GetSpellPackage();
            cards.ForEach(x => { Console.WriteLine(x.Name); });

            Card waterMonster = new Monster("Water Monster", 1, EElementType.WATER, 5, 1);
            Card earthMonster = new Monster("Earth Monster", 3, EElementType.EARTH, 5, 1);
            Card fireMonster = new Monster("Fire Monster", 5, EElementType.FIRE, 5, 1);
            Card windSpell = new Spell("Wind Spell", 2, EElementType.WIND, 3, 1);
            Card lightningSpell = new Spell("Lightning Spell", 4, EElementType.LIGHTNING, 3, 1);

            User user1 = new User("User1", "1234");
            user1.AddCardToDeck(waterMonster);
            user1.AddCardToDeck(earthMonster);
            user1.AddCardToDeck(fireMonster);
            user1.AddCardToDeck(windSpell);
            user1.AddCardToDeck(lightningSpell);

            User user2 = new User("User2", "1234");

            Console.WriteLine($"User 2 starts with {user2.HP}/{user2.MaxHP} HP");

            foreach (Card card in user1.Cards)
            {
                card.PlayCard();
                card.Attack(user2);
            }

            Console.WriteLine($"User 2 has {user2.HP}/{user2.MaxHP} HP");
        }*/
    }
}