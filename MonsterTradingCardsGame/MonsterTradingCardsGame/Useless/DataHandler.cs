using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.Model;

namespace MonsterTradingCardsGame.Useless
{
    public class DataHandler
    {
        private static DataHandler _instance;
        public static DataHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataHandler();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        private IEnumerable<User> users;
        public IEnumerable<Card> cards;

        private DataHandler()
        {
            //Get all data from db
            users = new List<User>();
            cards = CardRepo.Instance.GetAll();
        }

        public void Request(string method, string name)
        {

        }

        private void PostNewCard()
        {

        }

        public void SendGetRequest()
        {
            // URL des Servers, an den der GET-Request gesendet wird

            // Erstellen und Konfigurieren der WebRequest
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MTCG_Server.URL);
            request.Method = "GET";

            try
            {
                // Senden des GET-Requests und Empfangen der Antwort
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            // Lesen und Anzeigen der Antwort
                            string responseText = reader.ReadToEnd();
                            Console.WriteLine("GET-Anfrage erfolgreich. Antwort:");
                            Console.WriteLine(responseText);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("Fehler bei der GET-Anfrage: " + ex.Message);
            }
        }
    }
}
