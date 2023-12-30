using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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

                //Console.WriteLine(request);
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
            Dictionary<string, string> requestParams = new Dictionary<string, string>();

            // Extract headers
            var headers = request.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string contentType = "";
            string authorizationHeader = "";

            foreach (var header in headers)
            {
                if (header.StartsWith("Content-Type:"))
                {
                    contentType = header.Replace("Content-Type:", "").Trim();
                }
                else if (header.StartsWith("Authorization:"))
                {
                    authorizationHeader = header.Replace("Authorization:", "").Trim();
                    requestParams.Add("Token", authorizationHeader);
                }
            }


            // Check if Content-Type is application/json
            if (!string.IsNullOrEmpty(contentType) && contentType.ToLower().Contains("application/json"))
            {
                // Extract JSON data
                int databodyStartIndex = request.IndexOf("\r\n\r\n"); // Find the start of the body
                if (databodyStartIndex >= 0)
                {
                    databodyStartIndex += 4;
                    string data = request.Substring(databodyStartIndex);

                    if (data.StartsWith("[") && data.EndsWith("]"))
                    {
                        data = data.Trim('[', ']');
                        if (data.StartsWith("{") && data.EndsWith("}"))
                        {
                            string[] splitData = data.Split("},");
                            for (int i = 0; i < splitData.Length; i++)
                            {
                                requestParams.Add("JSON" + i.ToString(), "{" + splitData[i].Trim('{', '}', ' ') + "}");
                            }
                        }
                        else
                        {
                            string[] splitData = data.Split(',');
                            for (int i = 0; i < splitData.Length; i++)
                            {
                                requestParams.Add(i.ToString(), splitData[i].Trim('\"', ' '));
                            }
                        }
                    }
                    else
                    {

                        if (data.StartsWith("{") && data.EndsWith("}"))
                        {
                            requestParams.Add("JSON0", data);
                        }
                        else
                        {
                            requestParams.Add("0", data);
                        }
                    }
                }
            }

            return requestParams;
        }

        private string[] ParseJsonArray(string jsonString)
        {
            // Assuming JSON array is represented as an array of strings
            // For simplicity, a basic parsing approach is shown here.
            // This might need further enhancement to handle edge cases.

            // Remove the square brackets and split by comma to get individual elements
            string[] elements = jsonString.Trim('[', ']').Split(',');

            // Trim extra spaces and quotes from each element
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = elements[i].Trim(' ', '"');
            }

            return elements;
        }

        private void SendResponse(string response)
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            client.Send(responseBytes);
        }

        private void SendResponse200OKMessage(string message)
        {
            string response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nMessage: {message}\r\n";
            SendResponse(response);
        }

        private void SendResponse200OKData(Dictionary<string,string> data)
        {
            string response = "HTTP/1.1 200 OK\r\nContent-Type: application/json -d \r\n\r\n{";
            foreach (var item in data)
            {
                response += $"{item.Key}:{item.Value},";
            }
            response = response.Remove(response.Length - 1, 1) + "}\r\n";
            SendResponse(response);
        }

        private void HandleUserRegistration(Dictionary<string, string> requestParams)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string message = "HandleUserRegistration";
            SendResponse200OKMessage(message);
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
