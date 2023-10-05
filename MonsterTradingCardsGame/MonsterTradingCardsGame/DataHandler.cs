using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    public class DataHandler
    {
        public static DataHandler Instance
        {
            get
            {
                if (Instance == null)
                {
                    Instance = new DataHandler();
                }
                return Instance;
            }
            private set
            {
                Instance = value;
            }
        }

        private List<User> users;
        private List<Card> cards;

        private DataHandler()
        {
            //Get all data from db
            users = new List<User>();
            cards = new List<Card>();
        }

        public void Request(string method, string name)
        {

        }

        private void PostNewCard()
        {

        }
    }
}
