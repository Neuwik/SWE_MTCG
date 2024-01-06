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

        private ConcurrentBag<Battle> battles;

        private BattleHandler() 
        {
            battles = new ConcurrentBag<Battle>();
        }

        public bool UserJoinQue(User user)
        {
            if (UserIsInBattle(user) || UserIsInQue(user))
            {
                return false;
            }

            battles.OrderBy(b => b.QueStartTime);
            Battle battle = battles.FirstOrDefault(b => b.MatchupIsAllowed(user));
            if (battle != null)
            {
                Task.Run(() => battle.StartBattle(user));
                return true;
            }

            battles.Add(new Battle(user));
            return true;
        }

        public bool UserLeaveQue(User user)
        {
            Battle battle = battles.FirstOrDefault(b => b.UserInBattle(user.ID) && b.CurrentBatteState == BattleState.QUE);
            if (battle != null)
            {
                battles = new ConcurrentBag<Battle>(battles.Where(b => !object.ReferenceEquals(b, battle)));
                return true;
            }
            return false;
        }

        public List<string> ReadBattleLog(User user)
        {
            List<string> battleLog = new List<string>();

            Battle battle = battles.FirstOrDefault(b => b.UserInBattle(user.ID));
            if (battle != null)
            {
                battleLog = battle.ReadBattleLog(user.ID);
            }

            battles = new ConcurrentBag<Battle>(battles.Where(b => b.CurrentBatteState != BattleState.DELETABLE));

            return battleLog;
        }

        public bool UserIsInQue(User user)
        {
            return battles.FirstOrDefault(b => b.UserInBattle(user.ID) && b.CurrentBatteState == BattleState.QUE) != null;
        }

        public bool UserIsInBattle(User user)
        {
            return battles.FirstOrDefault(b => b.UserInBattle(user.ID) && b.CurrentBatteState == BattleState.RUNNING) != null;
        }
    }
}
