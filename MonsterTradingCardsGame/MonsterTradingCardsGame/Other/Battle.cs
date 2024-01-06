using MonsterTradingCardsGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Other
{
    public enum BattleState { QUE = 1, RUNNING = 2, ENDED = 3, DELETABLE = 5 }
    public class Battle
    {
        private readonly int MAXQUETIME = 10000;
        private readonly int MAXELODIF = 100;
        private readonly int MAXROUNDS = 100;
        public static readonly int ELOCHANGE = 20;

        public User User1 { get; private set; }
        public User User2 { get; private set; }

        public DateTime QueStartTime { get; private set; }
        public BattleState CurrentBatteState { get; private set; }
        private List<string> battleLog;

        private List<Card> deck1;
        private List<Card> deck2;
        private List<Card> playedCards1;
        private List<Card> playedCards2;

        public Battle(User user1)
        {
            User1 = user1;
            QueStartTime = DateTime.Now;
            CurrentBatteState = BattleState.QUE;
            battleLog = new List<string>();
            deck1 = new List<Card>();
            deck2 = new List<Card>();
            playedCards1 = new List<Card>();
            playedCards2 = new List<Card>();
        }

        public bool MatchupIsAllowed(User user2)
        {
            if (CurrentBatteState != BattleState.QUE)
            {
                return false;
            }

            if (User2 != null)
            {
                return false;
            }

            if (User1.ID == user2.ID)
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
            CurrentBatteState = BattleState.RUNNING;
            User2 = user2;

            deck1 = ShuffleCards(User1.Deck.ToList());
            deck2 = ShuffleCards(User2.Deck.ToList());

            int roundCounter = 0;

            while (roundCounter < MAXROUNDS && User1.HP > 0 && User2.HP > 0)
            {
                roundCounter++;
                battleLog.Add($"TURN {roundCounter}");
                PlayRound();
            }

            if (User1.HP <= 0 && User2.HP <= 0)
            {
                User1.AddDraw();
                User2.AddDraw();
                battleLog.Add($"(DRAW) {User1.Username} VS {User2.Username} (DRAW)");
            }
            else if (User1.HP <= 0)
            {
                int user1Elo = User1.Elo;
                User1.AddLoss(User2.Elo);
                User2.AddWin(user1Elo);
                battleLog.Add($"(LOST) {User1.Username} VS {User2.Username} (WON)");
            }
            else if (User2.HP <= 0)
            {
                int user1Elo = User1.Elo;
                User1.AddWin(User2.Elo);
                User2.AddLoss(user1Elo);
                battleLog.Add($"(WON) {User1.Username} VS {User2.Username} (LOST)");
            }
            else
            {
                User1.AddDraw();
                User2.AddDraw();
                battleLog.Add($"(DRAW) {User1.Username} VS {User2.Username} (DRAW)");
            }

            UserRepo.Instance.UpdateStats(User1);
            UserRepo.Instance.UpdateStats(User2);

            CurrentBatteState = BattleState.ENDED;
        }

        public List<string> ReadBattleLog(int userID)
        {
            if (User1 != null && User1.ID == userID)
            {
                if (CurrentBatteState == BattleState.ENDED)
                {
                    User1 = null;
                }
            }
            else if (User2 != null && User2.ID == userID)
            {
                if (CurrentBatteState == BattleState.ENDED)
                {
                    User2 = null;
                }
            }
            else
            {
                return null;
            }

            if (User1 == null && User2 == null)
            {
                CurrentBatteState = BattleState.DELETABLE;
            }

            return battleLog;
        }

        public bool UserInBattle(int userID)
        {
            if (CurrentBatteState == BattleState.DELETABLE)
            {
                return false;
            }

            if (User1 != null && User1.ID == userID)
            {
                return true;
            }
            if (User2 != null && User2.ID == userID)
            {
                return true;
            }
            return false;
        }

        private void PlayRound()
        {
            if (deck1.Count > 0)
            {
                Card card = deck1.First();
                playedCards1.Add(card);
                deck1.Remove(card);
            }
            if (deck2.Count > 0)
            {
                Card card = deck2.First();
                playedCards2.Add(card);
                deck2.Remove(card);
            }

            foreach (Card card in playedCards1)
            {
                IHealth target = GetNextTarget(playedCards2) ?? User2;
                int dmg = card.Attack(target);
                AddBattleLogEntry(card, target, dmg);
            }

            foreach (Card card in playedCards2)
            {
                IHealth target = GetNextTarget(playedCards1) ?? User1;
                int dmg = card.Attack(target);
                AddBattleLogEntry(card, target, dmg);
            }

            ClearBattlefield();
        }

        private IHealth GetNextTarget(List<Card> cards)
        {
            Card target = cards.FirstOrDefault(c => c is Monster && (c as Monster).HP >= 0);
            if (target != null)
                return target as Monster;
            return null;
        }

        private void ClearBattlefield()
        {
            for (int i = playedCards1.Count - 1; i >= 0; i--)
            {
                Card card = playedCards1[i];
                if (card is Monster)
                {
                    if ((card as Monster).HP <= 0)
                    {
                        card.ResetStats();
                        deck2.Add(card);
                        playedCards1.RemoveAt(i);
                    }
                }
                else if (card is Spell)
                {
                    //Spell swaps team after uses up
                    if ((card as Spell).Uses > 0)
                    {
                        deck1.Add(card);
                    }
                    else
                    {
                        card.ResetStats();
                        deck2.Add(card);
                    }
                    playedCards1.RemoveAt(i);
                }
            }

            for (int i = playedCards2.Count - 1; i >= 0; i--)
            {
                Card card = playedCards2[i];
                if (card is Monster)
                {
                    if ((card as Monster).HP <= 0)
                    {
                        card.ResetStats();
                        deck1.Add(card);
                        playedCards2.RemoveAt(i);
                    }
                }
                else if (card is Spell)
                {
                    //Spell swaps team after uses up
                    if ((card as Spell).Uses > 0)
                    {
                        deck2.Add(card);
                    }
                    else
                    {
                        card.ResetStats();
                        deck1.Add(card);
                    }
                    playedCards2.RemoveAt(i);
                }
            }
        }

        private void AddBattleLogEntry(Card card, IHealth target, int dmg)
        {
            if (target is User)
            {
                battleLog.Add($"{card.Name} has dealt {dmg} DMG to {(target as User).Username} ({target.HP}/{target.MaxHP} HP left)");
            }
            else if (target is Monster)
            {
                battleLog.Add($"{card.Name} has dealt {dmg} DMG to {(target as Monster).Name} ({target.HP}/{target.MaxHP} HP left)");
            }
        }

        private static Random random = new Random();
        private List<Card> ShuffleCards(List<Card> cards)
        {
            // Fisher-Yates shuffle algorithm
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card card = cards[k];
                cards[k] = cards[n];
                cards[n] = card;
            }
            return cards;
        }
    }
}
