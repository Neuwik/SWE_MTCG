using MonsterTradingCardsGame.Model;
using MonsterTradingCardsGame.Other;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Request_Handling
{
    public class BattleHandler
    {
        private static BattleHandler instance;
        public static BattleHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleHandler();
                }
                return instance;
            }
        }

        private List<Battle> battles;

        private BattleHandler() 
        {
            battles = new List<Battle>();
            ThreadPool.QueueUserWorkItem(new WaitCallback(ManageLogQueue));
        }

        public bool UserJoinQueue(User user)
        {
            if (UserIsInBattle(user))
            {
                return false;
            }

            lock (battles)
            {
                battles.OrderBy(b => b.QueueStartTime);
                Battle battle = battles.FirstOrDefault(b => b.MatchupIsAllowed(user));
                if (battle != null)
                {
                    if (!battle.AddOponent(user))
                    {
                        return false;
                    }
                    ThreadPool.QueueUserWorkItem(new WaitCallback(battle.StartBattle));
                    return true;
                }

                battles.Add(new Battle(user));
            }
            return true;
        }

        public bool UserLeaveQueue(User user)
        {
            lock (battles)
            {
                return battles.RemoveAll(b => b.UserInBattle(user.ID) && b.CurrentBatteState == BattleState.QUEUE) > 0;
            }
        }

        public List<string> ReadBattleLog(User user)
        {
            List<string> battleLog = new List<string>();

            lock (battles)
            {
                Battle battle = battles.FirstOrDefault(b => b.UserInBattle(user.ID));
                if (battle != null)
                {
                    battleLog = battle.ReadBattleLog(user.ID);
                }

                battles.RemoveAll(b => b.CurrentBatteState == BattleState.DELETABLE);
            }

            return battleLog;
        }

        public bool UserIsInQueue(User user)
        {
            lock(battles)
            {
                return battles.FirstOrDefault(b => b.UserInBattle(user.ID) && b.CurrentBatteState == BattleState.QUEUE) != null;
            }
        }

        public bool UserIsInRunningBattle(User user)
        {
            lock (battles)
            {
                return battles.FirstOrDefault(b => b.UserInBattle(user.ID) && b.CurrentBatteState == BattleState.RUNNING) != null;
            }
        }

        public bool UserIsInBattle(User user)
        {
            lock (battles)
            {
                return battles.FirstOrDefault(b => b.UserInBattle(user.ID)) != null;
            }
        }
    
        private void ManageLogQueue(object state)
        {
            while (true)
            {
                Thread.Sleep(Battle.MAXQUEUETIME * 2);
                lock (battles)
                {
                    battles = battles.OrderBy(b => b.QueueStartTime).ToList();
                    for (int i = battles.Count - 1; i >= 0; i--)
                    {
                        for (int j = battles.Count - 1; j >= 0; j--)
                        {
                            if (i != j)
                            {
                                if (battles[i].MatchupIsAllowed(battles[j].User1))
                                {
                                    if (battles[i].AddOponent(battles[j].User1))
                                    {
                                        ThreadPool.QueueUserWorkItem(new WaitCallback(battles[i].StartBattle));
                                        battles.RemoveAt(j);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
