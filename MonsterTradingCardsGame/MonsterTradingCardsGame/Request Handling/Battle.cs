using MonsterTradingCardsGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Request_Handling
{
    public enum BattleState { QUE = 1, RUNNING = 2, ENDED = 3 }
    public class Battle
    {
        private readonly int MAXQUETIME = 10000;
        private readonly int MAXELODIF = 100;
        private readonly int MAXROUNDS = 100;

        public User User1 { get; private set; }
        public User User2 { get; private set; }

        public DateTime QueStartTime { get; private set; }
        public BattleState CurrentBatteState { get; private set; }
        public List<string> BattleLog { get; private set; }

        private List<Card> deck1;
        private List<Card> deck2;

        public Battle(User user1)
        {
            User1 = user1;
            QueStartTime = DateTime.Now;
            CurrentBatteState = BattleState.QUE;
            BattleLog = new List<string>();
        }

        public bool MatchupIsAllowed(User user2)
        {
            if (CurrentBatteState != BattleState.QUE)
            {
                return false;
            }

            if ((DateTime.Now - QueStartTime).TotalSeconds > MAXQUETIME)
            {
                return true;
            }

            if (user2.Elo < User1.Elo - MAXELODIF || user2.Elo > User1.Elo + MAXELODIF)
            {
                return false;
            }

            return true;
        }

        public void StartBattle(User user2)
        {
            if (!MatchupIsAllowed(user2))
            {
                return;
            }

            CurrentBatteState = BattleState.RUNNING;
            User2 = user2;

            deck1 = User1.Deck.ToList();
            deck2 = User2.Deck.ToList();

            int roundCounter = 0;

            while(roundCounter < MAXROUNDS || (User1.HP > 0 && User2.HP > 0))
            {
                PlayRound();
                roundCounter++;
            }

            //TODO: Add Win Draw Loss to users
            if (User1.HP <= 0)
            {
                int user1Elo = User1.Elo;
                User1.AddLoss(User2.Elo);
                User2.AddWin(user1Elo);
            }
            else if (User2.HP <= 0)
            {
                int user1Elo = User1.Elo;
                User1.AddWin(User2.Elo);
                User2.AddLoss(user1Elo);
            }
            else
            {
                User1.AddDraw();
                User2.AddDraw();
            }    
            CurrentBatteState = BattleState.ENDED;
        }

        private void PlayRound()
        {

        }
    }
}
