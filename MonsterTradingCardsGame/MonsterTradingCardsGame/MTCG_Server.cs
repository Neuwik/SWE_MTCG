using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    public class MTCG_Server
    {

        public static readonly int PORT = 10001;
        public static readonly string URL = "http://127.0.0.1:" + MTCG_Server.PORT;
        private Socket listener;
        private const int bufferSize = 1024;
        private const int backlog = 10;

        public MTCG_Server()
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            listener.Bind(new IPEndPoint(IPAddress.Loopback, PORT));
            listener.Listen(backlog);
            Console.WriteLine("HTTP-Server gestartet. Warte auf Verbindungen...");
            while (true)
            {
                Socket client = listener.Accept();
                ThreadPool.QueueUserWorkItem(ProcessRequest, client);
            }
        }

        public void Stop()
        {
            if (listener.Connected)
            {
                listener.Close();
            }
        }

        private void ProcessRequest(object obj)
        {
            Socket client = obj as Socket;
            if (client == null) return;

            byte[] buffer = new byte[bufferSize];
            int bytesRead = client.Receive(buffer);
            string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            if (request.StartsWith("POST"))
            {
                if (request.StartsWith("POST /users"))
                {
                    HandleUserRegistration(client, request);
                }
                else if (request.StartsWith("POST /sessions"))
                {
                    HandleLogin(client, request);
                }
                else if (request.StartsWith("POST /packages"))
                {
                    HandleCreatePackages(client, request);
                }
                else if (request.StartsWith("POST /transactions/packages"))
                {
                    HandleAcquirePackages(client, request);
                }
                else if (request.StartsWith("POST /tradings"))
                {
                    HandleCreateTradingDeal(client, request);
                }
                else if (request.StartsWith("POST /battles"))
                {
                    HandleBattle(client, request);
                }
                // Add more conditions to differentiate between different POST endpoints
                else
                {
                    SendResponse(client, "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nEndpoint not found");
                }
            }
            else if (request.StartsWith("GET"))
            {
                if (request.StartsWith("GET /cards"))
                {
                    HandleGetCards(client, request);
                }
                else if (request.StartsWith("GET /deck"))
                {
                    HandleGetDeck(client, request);
                }
                else if (request.StartsWith("GET /users"))
                {
                    HandleGetUserData(client, request);
                }
                else if (request.StartsWith("GET /stats"))
                {
                    HandleGetStats(client, request);
                }
                else if (request.StartsWith("GET /scoreboard"))
                {
                    HandleGetScoreboard(client, request);
                }
                else if (request.StartsWith("GET /tradings"))
                {
                    HandleGetTradingDeals(client, request);
                }
                // Add more conditions to differentiate between different GET endpoints
                else
                {
                    SendResponse(client, "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nEndpoint not found");
                }
            }
            else if (request.StartsWith("PUT"))
            {
                if (request.StartsWith("PUT /deck"))
                {
                    HandleConfigureDeck(client, request);
                }
                else if (request.StartsWith("PUT /users"))
                {
                    HandleEditUserData(client, request);
                }
                // Add more conditions to differentiate between different PUT endpoints
                else
                {
                    SendResponse(client, "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nEndpoint not found");
                }
            }
            else if (request.StartsWith("DELETE"))
            {
                if (request.StartsWith("DELETE /tradings"))
                {
                    HandleDeleteTrading(client, request);
                }
                // Add more conditions to differentiate between different DELETE endpoints
                else
                {
                    SendResponse(client, "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nEndpoint not found");
                }
            }
            else
            {
                SendResponse(client, "HTTP/1.1 400 Bad Request\r\nContent-Type: text/plain\r\n\r\nBad Request");
            }

            client.Close();
        }

        private void SendResponse(Socket client, string response)
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            client.Send(responseBytes);
        }

        private void HandleUserRegistration(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleUserRegistration";
            SendResponse(client, response);
        }

        private void HandleLogin(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleLogin";
            SendResponse(client, response);
        }

        private void HandleCreatePackages(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleCreatePackages";
            SendResponse(client, response);
        }

        private void HandleGetCards(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetCards";
            SendResponse(client, response);
        }

        private void HandleGetDeck(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetDeck";
            SendResponse(client, response);
        }

        private void HandleConfigureDeck(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleConfigureDeck";
            SendResponse(client, response);
        }

        private void HandleAcquirePackages(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleAcquirePackages";
            SendResponse(client, response);
        }

        private void HandleCreateTradingDeal(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleCreateTradingDeal";
            SendResponse(client, response);
        }

        private void HandleGetUserData(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetUserData";
            SendResponse(client, response);
        }

        private void HandleGetStats(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetStats";
            SendResponse(client, response);
        }

        private void HandleGetScoreboard(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetScoreboard";
            SendResponse(client, response);
        }

        private void HandleGetTradingDeals(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetTradingDeals";
            SendResponse(client, response);
        }

        private void HandleEditUserData(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleEditUserData";
            SendResponse(client, response);
        }

        private void HandleBattle(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleBattle";
            SendResponse(client, response);
        }

        private void HandleDeleteTrading(Socket client, string request)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleDeleteTrading";
            SendResponse(client, response);
        }
    }
}
