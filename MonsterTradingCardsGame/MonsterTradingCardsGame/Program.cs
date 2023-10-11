using System.Net;

namespace MonsterTradingCardsGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Thread serverThread = new Thread(StartServer);
                serverThread.Start();
                Thread.Sleep(1000);
                DataHandler.Instance.SendGetRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {

            }
        }

        static void StartServer()
        {
            MTCG_Server server = new MTCG_Server();
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

        

        static void TestCardCreation()
        {
            Card waterMonster = new Monster("Water Monster", 1, EElementType.WATER);
            Card earthMonster = new Monster("Earth Monster", 3, EElementType.EARTH);
            Card fireMonster = new Monster("Fire Monster", 5, EElementType.FIRE);
            Card windSpell = new Spell("Wind Spell", 2, EElementType.WIND);
            Card lightningSpell = new Spell("Lightning Spell", 4, EElementType.LIGHTNING);

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
        }
    }
}