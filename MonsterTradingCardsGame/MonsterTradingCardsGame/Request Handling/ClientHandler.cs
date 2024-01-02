using MonsterTradingCardsGame.Model;
using MonsterTradingCardsGame.Request_Handling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MonsterTradingCardsGame
{
    internal class ClientHandler
    {
        private Socket client;
        private List<string> requestHistory;
        private const int bufferSize = 1024;

        public ClientHandler(Socket client)
        {
            this.client = client;
            requestHistory = new List<string>();
        }

        public void Start(object state)
        {
            HandleRequest();
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
                        throw new BadRequestException();
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
                        throw new BadRequestException();
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
                        throw new BadRequestException();
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
                        throw new BadRequestException();
                    }
                }
                else
                {
                    throw new BadRequestException();
                }
            }
            catch (HttpResponseExcetion e)
            {
                Console.WriteLine(e.ToString());
                SendResponseExcetion(e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Some unexpected Error: " + e.Message);
                if(!(client.Poll(1000, SelectMode.SelectRead) && client.Available == 0))
                {
                    SendResponseExcetion(new InternalServerErrorException(e.Message));
                }
            }
            finally
            {
                //Console.WriteLine("CLOSE");
                client.Shutdown(SocketShutdown.Both);
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

        private void SendResponseMessage(string message, string responseCode = "200 OK")
        {
            string response = $"HTTP/1.1 {responseCode}\r\nContent-Type: text/plain\r\n\r\nMessage: {message}\r\n";
            SendResponse(response);
        }

        private void SendResponseData(Dictionary<string,string> data, string responseCode = "200 OK")
        {
            string response = $"HTTP/1.1 {responseCode}\r\nContent-Type: application/json -d \r\n\r\n{"{"}";
            foreach (var item in data)
            {
                response += $"{item.Key}:{item.Value},";
            }
            response = response.Remove(response.Length - 1, 1) + "}\r\n";
            SendResponse(response);
        }

        private void SendResponseExcetion(HttpResponseExcetion responseExcetion)
        {
            string response = $"HTTP/1.1 {responseExcetion.ResponseCode}\r\nContent-Type: text/plain\r\n\r\n{responseExcetion.Message}\r\n";
            SendResponse(response);
        }

        private void HandleUserRegistration(Dictionary<string, string> requestParams)
        {
            string jsonUserString = requestParams.Where(pair => pair.Key.StartsWith("JSON")).FirstOrDefault().Value;
            JsonElement jsonUser = JsonDocument.Parse(jsonUserString).RootElement;

            string username = null;
            string password = null;

            if (jsonUser.TryGetProperty("Username", out JsonElement usernameElement) && usernameElement.ValueKind != JsonValueKind.Null)
            {
                username = usernameElement.GetString();
            }

            if (jsonUser.TryGetProperty("Password", out JsonElement passwordElement) && passwordElement.ValueKind != JsonValueKind.Null)
            {
                password = passwordElement.GetString();
            }

            if (username == null || password == null)
            {
                throw new WrongParametersException("Registration Failed");
            }
            if(UserRepo.Instance.UsernameExists(username))
            {
                throw new InputNotAllowedException("Username Already Exists");
            }

            User user = new User(username, password);
            UserRepo.Instance.Add(user);
            string message = user.Username + " was registered";
            SendResponseMessage(message, "201 Created");
        }

        private void HandleLogin(Dictionary<string, string> requestParams)
        {
            string jsonUserString = requestParams.Where(pair => pair.Key.StartsWith("JSON")).FirstOrDefault().Value;
            JsonElement jsonUser = JsonDocument.Parse(jsonUserString).RootElement;

            string username = null;
            string password = null;

            if (jsonUser.TryGetProperty("Username", out JsonElement usernameElement) && usernameElement.ValueKind != JsonValueKind.Null)
            {
                username = usernameElement.GetString();
            }

            if (jsonUser.TryGetProperty("Password", out JsonElement passwordElement) && passwordElement.ValueKind != JsonValueKind.Null)
            {
                password = passwordElement.GetString();
            }

            if (username == null || password == null)
            {
                throw new WrongParametersException("Login Failed");
            }

            string token = UserRepo.Instance.GetTokenByUsernamePassword(username, password);
            if (token == null) 
            {
                throw new NothingFoundException("Wrong Username or Password");
            }
            else if(MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new AlreadyLoggedInException("Login Failed");
            }
            else
            {
                MTCG_Server.Instance.LoggedInUsers.Add(token);
            }

            string message = "User logged in: " + token;
            SendResponseMessage(message);
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
