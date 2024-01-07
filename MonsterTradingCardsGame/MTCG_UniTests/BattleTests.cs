using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using MonsterTradingCardsGame.Model;
using MonsterTradingCardsGame.Other;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_UnitTests
{
    [TestClass]
    public class BattleTests
    {
        private Battle battle;
        private User user1;
        private User user2;
        private User user3;
        private User user4;
        private User userNoCards1;

        [TestInitialize]
        public void Init()
        {
            user1 = new User(1, "Max", "Muster", "MM");
            user2 = new User(2, "Otto", "Normal", "ON");
            user3 = new User(3, "Maxi", "Muster", "MM2", user1.Elo + Battle.MAXELODIF + 1);
            user4 = new User(4, "Otti", "Normal", "ON2", user1.Elo + Battle.MAXELODIF - 1);
            user1.BuyPackage();
            user2.BuyPackage();
            user3.BuyPackage();
            user4.BuyPackage();

            userNoCards1 = new User(5, "Manuel", "Huber", "MH");
        }

        [TestMethod]
        public void StartQueue()
        {
            battle = new Battle(user1);
            Assert.AreEqual(EBattleState.QUEUE, battle.CurrentBatteState);
        }

        [TestMethod]
        public void CanNotJoinOwnBattle()
        {
            battle = new Battle(user1);
            Assert.IsFalse(battle.MatchupIsAllowed(user1));
        }

        [TestMethod]
        public void CanNotJoinBattleEloDif()
        {
            battle = new Battle(user1);
            Assert.IsFalse(battle.MatchupIsAllowed(user3));
        }

        [TestMethod]
        public void CanJoinBattleEloDifAfterTime()
        {
            battle = new Battle(user1);
            Thread.Sleep(Battle.MAXQUEUETIME + 1);
            Assert.IsTrue(battle.MatchupIsAllowed(user3));
        }

        [TestMethod]
        public void CanJoinBattle()
        {
            battle = new Battle(user1);
            Assert.IsTrue(battle.MatchupIsAllowed(user2));
        }

        [TestMethod]
        public void JoinBattle()
        {
            StartQueue();
            Assert.IsTrue(battle.AddOponent(user2));
        }

        [TestMethod]
        public void UsersAreInBattle()
        {
            JoinBattle();
            Assert.IsTrue(battle.UserInBattle(user1.ID));
            Assert.IsTrue(battle.UserInBattle(user2.ID));
        }

        public void CanNotRunEmptyBattle()
        {
            StartQueue();
            battle.StartBattle(true);
            Assert.AreEqual(EBattleState.QUEUE, battle.CurrentBatteState);
        }

        [TestMethod]
        public void RunBattle()
        {
            JoinBattle();
            battle.StartBattle(true);
            Assert.AreEqual(EBattleState.ENDED, battle.CurrentBatteState);
        }

        [TestMethod]
        public void ReadLogAfterBattle()
        {
            RunBattle();
            Assert.IsTrue(battle.ReadBattleLog(user1.ID).Count > 0);
        }

        [TestMethod]
        public void LeaveDoneBattleAfterLogRead()
        {
            RunBattle();
            Assert.IsTrue(battle.ReadBattleLog(user1.ID).Count > 0);
            Assert.IsFalse(battle.UserInBattle(user1.ID));
        }

        [TestMethod]
        public void DeleteBattleAfterBothRead()
        {
            RunBattle();
            Assert.IsTrue(battle.ReadBattleLog(user1.ID).Count > 0);
            Assert.IsTrue(battle.ReadBattleLog(user2.ID).Count > 0);
            Assert.AreEqual(EBattleState.DELETABLE, battle.CurrentBatteState);
        }

        [TestMethod]
        public void WinLossDrawAdded()
        {
            RunBattle();
            Assert.AreEqual(1, user1.Wins + user1.Losses + user1.Draws);
            Assert.AreEqual(1, user2.Wins + user2.Losses + user2.Draws);
            Assert.AreEqual(user1.Wins, user2.Losses);
            Assert.AreEqual(user1.Losses, user2.Wins);
            Assert.AreEqual(user1.Draws, user2.Draws);
        }

        [TestMethod]
        public void EloChangeAfterEvenGame()
        {
            int elo1 = user1.Elo;
            int elo2 = user2.Elo;

            RunBattle();

            if (user1.Wins > 0)
            {
                Assert.IsTrue(user1.Elo > elo1);
                Assert.IsTrue(user2.Elo < elo2);
            }
            else if (user1.Losses > 0)
            {
                Assert.IsTrue(user1.Elo < elo1);
                Assert.IsTrue(user2.Elo > elo2);
            }
            else
            {
                Assert.AreEqual(elo1, user1.Elo);
                Assert.AreEqual(elo2, user2.Elo);
            }
        }

        [TestMethod]
        public void MoreEloChangeAfterUnevenGame()
        {
            int elo1 = user1.Elo;
            int elo4 = user4.Elo;

            battle = new Battle(user1);
            Assert.IsTrue(battle.AddOponent(user4));
            battle.StartBattle(true);
            Assert.AreEqual(EBattleState.ENDED, battle.CurrentBatteState);

            if (user1.Wins > 0)
            {
                Assert.IsTrue(user1.Elo - elo1 > Battle.ELOCHANGE);
                Assert.IsTrue(elo4 - user4.Elo > Battle.ELOCHANGE);
            }
            else if (user1.Losses > 0)
            {
                Assert.IsTrue(elo1 - user1.Elo < Battle.ELOCHANGE);
                Assert.IsTrue(user4.Elo - elo4 < Battle.ELOCHANGE);
            }
            else
            {
                Assert.IsTrue(user1.Elo > elo1);
                Assert.IsTrue(user4.Elo < elo4);
            }
        }

        [TestMethod]
        public void CanNotStartQueueWithNoCards()
        {
            battle = new Battle(userNoCards1);
            Assert.IsFalse(battle.UserInBattle(userNoCards1.ID));
            Assert.AreEqual(EBattleState.DELETABLE, battle.CurrentBatteState);
        }

        [TestMethod]
        public void CanNotJoinBattleWithNoCards()
        {
            StartQueue();
            Assert.IsFalse(battle.MatchupIsAllowed(userNoCards1));
        }

        [TestMethod]
        public void CanNotReadLogOfOtherUsersBattle()
        {
            RunBattle();
            Assert.IsFalse(battle.UserInBattle(user3.ID));
            Assert.IsNull(battle.ReadBattleLog(user3.ID));
        }
    }
}
