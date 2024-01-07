using MonsterTradingCardsGame.Model;
using MonsterTradingCardsGame.Other;

namespace MTCG_UnitTests
{
    [TestClass]
    public class UserTests
    {
        private User user1;
        private User user2;

        [TestInitialize]
        public void Init()
        {
            user1 = new User(1, "Max", "Muster", "MM", 100);
            user2 = new User(2, "Otto", "Normal", "ON", 500);
        }

        [TestMethod]
        public void EloChangeOnWin()
        {
            int elo1 = user1.Elo;
            int elo2 = user2.Elo;

            //Against same Elo
            user1.AddWin(user1.Elo);
            Assert.AreEqual(elo1 + Battle.ELOCHANGE, user1.Elo);

            //Against higher Elo
            elo1 = user1.Elo;
            user1.AddWin(elo2);
            Assert.IsTrue(user1.Elo > elo1 + Battle.ELOCHANGE);

            //Against lower Elo
            user2.AddWin(elo1);
            Assert.IsTrue(user2.Elo < elo2 + Battle.ELOCHANGE);
        }

        [TestMethod]
        public void EloChangeOnLoss()
        {
            int elo1 = user1.Elo;
            int elo2 = user2.Elo;

            //Against same Elo
            user1.AddLoss(user1.Elo);
            Assert.AreEqual(elo1 - Battle.ELOCHANGE, user1.Elo);

            //Against higher Elo
            elo1 = user1.Elo;
            user1.AddLoss(elo2);
            Assert.IsTrue(user1.Elo > elo1 - Battle.ELOCHANGE);

            //Against lower Elo
            user2.AddLoss(elo1);
            Assert.IsTrue(user2.Elo < elo2 - Battle.ELOCHANGE);
        }

        [TestMethod]
        public void EloChangeOnDraw()
        {
            int elo1 = user1.Elo;
            int elo2 = user2.Elo;

            //Against same Elo
            user1.AddDraw(elo1);
            Assert.AreEqual(elo1, user1.Elo);

            //Against higher Elo
            user1.AddDraw(elo2);
            Assert.IsTrue(user1.Elo > elo1);

            //Against lower Elo
            user2.AddDraw(elo1);
            Assert.IsTrue(user2.Elo < elo2);
        }
    }
}