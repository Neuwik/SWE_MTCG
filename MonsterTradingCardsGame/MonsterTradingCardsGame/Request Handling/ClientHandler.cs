using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    internal class ClientHandler
    {
        private Socket client;
        private List<string> requestHistory;
        private const int bufferSize = 1024;
        private Thread thread;

        public ClientHandler(Socket client)
        {
            this.client = client;
            requestHistory = new List<string>();
            thread = new Thread(HandleRequest);
            thread.Start();
        }

        private string ReadRequest()
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead = client.Receive(buffer);
            string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            return request;
        }

        private void HandleRequest()
        {
            try
            {
                string request = ReadRequest();
                Dictionary<string, string> requestParams = SplitRequestParams(request);

                Console.WriteLine(request);
                foreach (var param in requestParams)
                {
                    Console.WriteLine($"\t{param.Key}\t->\t{param.Value}");
                }

                if (request.StartsWith("POST"))
                {
                    if (request.StartsWith("POST /users"))
                    {
                        HandleUserRegistration(requestParams);
                    }
                    else if (request.StartsWith("POST /sessions"))
                    {
                        HandleLogin(requestParams);
                    }
                    else if (request.StartsWith("POST /packages"))
                    {
                        HandleCreatePackages(requestParams);
                    }
                    else if (request.StartsWith("POST /transactions/packages"))
                    {
                        HandleAcquirePackages(requestParams);
                    }
                    else if (request.StartsWith("POST /tradings"))
                    {
                        HandleCreateTradingDeal(requestParams);
                    }
                    else if (request.StartsWith("POST /battles"))
                    {
                        HandleBattle(requestParams);
                    }
                    // Add more conditions to differentiate between different POST endpoints
                    else
                    {
                        SendResponse("HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nEndpoint not found");
                    }
                }
                else if (request.StartsWith("GET"))
                {
                    if (request.StartsWith("GET /cards"))
                    {
                        HandleGetCards(requestParams);
                    }
                    else if (request.StartsWith("GET /deck"))
                    {
                        HandleGetDeck(requestParams);
                    }
                    else if (request.StartsWith("GET /users"))
                    {
                        HandleGetUserData(requestParams);
                    }
                    else if (request.StartsWith("GET /stats"))
                    {
                        HandleGetStats(requestParams);
                    }
                    else if (request.StartsWith("GET /scoreboard"))
                    {
                        HandleGetScoreboard(requestParams);
                    }
                    else if (request.StartsWith("GET /tradings"))
                    {
                        HandleGetTradingDeals(requestParams);
                    }
                    // Add more conditions to differentiate between different GET endpoints
                    else
                    {
                        SendResponse("HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nEndpoint not found");
                    }
                }
                else if (request.StartsWith("PUT"))
                {
                    if (request.StartsWith("PUT /deck"))
                    {
                        HandleConfigureDeck(requestParams);
                    }
                    else if (request.StartsWith("PUT /users"))
                    {
                        HandleEditUserData(requestParams);
                    }
                    // Add more conditions to differentiate between different PUT endpoints
                    else
                    {
                        SendResponse("HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nEndpoint not found");
                    }
                }
                else if (request.StartsWith("DELETE"))
                {
                    if (request.StartsWith("DELETE /tradings"))
                    {
                        HandleDeleteTrading(requestParams);
                    }
                    // Add more conditions to differentiate between different DELETE endpoints
                    else
                    {
                        SendResponse("HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nEndpoint not found");
                    }
                }
                else
                {
                    SendResponse("HTTP/1.1 400 Bad Request\r\nContent-Type: text/plain\r\n\r\nBad Request");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SendResponse("HTTP/1.1 400 Bad Request\r\nContent-Type: text/plain\r\n\r\nBad Request");
            }
            finally
            {
                client.Close();
            }
        }

        private Dictionary<string, string> SplitRequestParams(string request)
        {
            /*
            Split Request to Format:
            EMPTY STRING
            key
            :
            value
            ,
            key
            :
            value
            ,
            ....
            */
            string[] splitRequest = request.Split('{')[1].Split('}')[0].Split('\"');
            Dictionary<string, string> requestParams = new Dictionary<string, string>();

            for (int i = 3; i < splitRequest.Length; i+=4)
            {
                requestParams.Add(splitRequest[i-2], splitRequest[i]);
            }

            /*
            Console.WriteLine(request);
            foreach (var param in requestParams)
            {
                Console.WriteLine($"\t{param.Key}\t->\t{param.Value}");
            }
            */

            return requestParams;
        }

        private void SendResponse(string response)
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            client.Send(responseBytes);
        }

        private void HandleUserRegistration(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleUserRegistration";
            SendResponse(response);
        }

        private void HandleLogin(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleLogin";
            SendResponse(response);
        }

        private void HandleCreatePackages(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleCreatePackages";
            SendResponse(response);
        }

        private void HandleGetCards(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetCards";
            SendResponse(response);
        }

        private void HandleGetDeck(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetDeck";
            SendResponse(response);
        }

        private void HandleConfigureDeck(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleConfigureDeck";
            SendResponse(response);
        }

        private void HandleAcquirePackages(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleAcquirePackages";
            SendResponse(response);
        }

        private void HandleCreateTradingDeal(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleCreateTradingDeal";
            SendResponse(response);
        }

        private void HandleGetUserData(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetUserData";
            SendResponse(response);
        }

        private void HandleGetStats(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetStats";
            SendResponse(response);
        }

        private void HandleGetScoreboard(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetScoreboard";
            SendResponse(response);
        }

        private void HandleGetTradingDeals(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetTradingDeals";
            SendResponse(response);
        }

        private void HandleEditUserData(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleEditUserData";
            SendResponse(response);
        }

        private void HandleBattle(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleBattle";
            SendResponse(response);
        }

        private void HandleDeleteTrading(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleDeleteTrading";
            SendResponse(response);
        }
    }
}
