using MonsterTradingCardsGame.Model;
using MonsterTradingCardsGame.Request_Handling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MonsterTradingCardsGame
{
    internal class ClientHandler
    {
        private Socket client;
        private List<string> requestHistory;
        private const int bufferSize = 1024;

        List<string> requestPath;
        Dictionary<string, string> requestGetParams;
        Dictionary<string, string> requestHeaders;
        List<string> requestData;

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
                Console.WriteLine(request);

                //string requestType = request.Split(" ")[0];
                requestPath = SplitRequestPath(request);
                requestGetParams = SplitRequestGetParams(request);
                requestHeaders = SplitRequestHeaders(request);
                requestData = SplitRequestData(request, requestHeaders);

                if (requestPath.Count <= 0)
                {
                    throw new BadRequestException();
                }

                if (request.StartsWith("POST"))
                {
                    if (request.StartsWith("POST /users"))
                    {
                        HandleUserRegistration();
                    }
                    else if (request.StartsWith("POST /sessions"))
                    {
                        HandleLogin();
                    }
                    else if (request.StartsWith("POST /packages"))
                    {
                        HandleCreatePackages();
                    }
                    else if (request.StartsWith("POST /transactions/packages"))
                    {
                        HandleAcquirePackages();
                    }
                    else if (request.StartsWith("POST /tradings"))
                    {
                        HandleCreateTradingDeal();
                    }
                    else if (request.StartsWith("POST /battles"))
                    {
                        HandleBattle();
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
                        HandleGetCards();
                    }
                    else if (request.StartsWith("GET /deck"))
                    {
                        HandleGetDeck();
                    }
                    else if (request.StartsWith("GET /users"))
                    {
                        HandleGetUserData();
                    }
                    else if (request.StartsWith("GET /stats"))
                    {
                        HandleGetStats();
                    }
                    else if (request.StartsWith("GET /scoreboard"))
                    {
                        HandleGetScoreboard();
                    }
                    else if (request.StartsWith("GET /tradings"))
                    {
                        HandleGetTradingDeals();
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
                        HandleConfigureDeck();
                    }
                    else if (request.StartsWith("PUT /users"))
                    {
                        HandleEditUserData();
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
                        HandleDeleteTrading();
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
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }

        private List<string> SplitRequestPath(string request)
        {
            List<string> requestPath = new List<string>();

            string[] lines = request.Split('\n');
            if (lines.Length > 0)
            {
                string[] splitRequest = lines[0].Split(" ");

                if (splitRequest.Length >= 2)
                {
                    splitRequest = splitRequest[1].Split("HTTP");
                    if (splitRequest.Length > 0)
                    {
                        splitRequest = splitRequest[0].Split("?");
                        if (splitRequest.Length > 0)
                        {
                            string[] pathParts = splitRequest[0].Split("/");

                            foreach (string pathPart in pathParts)
                            {
                                if (pathPart != "")
                                {
                                    requestPath.Add(pathPart);
                                }
                            }
                        }
                    }
                }
            }

            return requestPath;
        }

        private Dictionary<string, string> SplitRequestGetParams(string request)
        {
            Dictionary<string, string> requestGetParams = new Dictionary<string, string>();

            string[] lines = request.Split('\n');
            if (lines.Length > 0)
            {
                string[] splitRequest = lines[0].Split("?");

                if (splitRequest.Length == 2)
                {
                    splitRequest = splitRequest[1].Split("HTTP");
                    if (splitRequest.Length > 0)
                    {
                        string[] getParamPairs = splitRequest[0].Split("&");
                        foreach (string getParamPair in getParamPairs)
                        {
                            string[] pair = getParamPair.Split("=");
                            if (pair.Length == 2)
                            {
                                requestGetParams.Add(pair[0].Trim(), pair[1].Trim());
                            }
                        }
                    }
                }
            }
            
            return requestGetParams;
        }

        private Dictionary<string, string> SplitRequestHeaders(string request)
        {
            Dictionary<string, string> requestHeaders = new Dictionary<string, string>();

            // Extract headers
            string[] headers = request.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string header in headers)
            {
                string[] headerPair = header.Split(":");
                if (headerPair.Length >= 2)
                {
                    requestHeaders.Add(headerPair[0].Trim(), headerPair[1].Trim());
                }
            }

            return requestHeaders;
        }

        private List<string> SplitRequestData(string request, Dictionary<string, string> requestHeaders)
        {
            List<string> requestData = new List<string>();

            // Check if Content-Type is application/json
            if (requestHeaders.ContainsKey("Content-Type") && !string.IsNullOrEmpty(requestHeaders["Content-Type"]) && requestHeaders["Content-Type"].ToLower().Contains("application/json"))
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
                                requestData.Add("{" + splitData[i].Trim('{', '}', ' ') + "}");
                            }
                        }
                        else
                        {
                            string[] splitData = data.Split(',');
                            for (int i = 0; i < splitData.Length; i++)
                            {
                                requestData.Add(splitData[i].Trim('\"', ' '));
                            }
                        }
                    }
                    else
                    {
                        if(data != "")
                            requestData.Add(data);
                    }
                }
            }

            return requestData;
        }

        private string GetTokenFromHeaders()
        {
            //"Authorization: Bearer altenhof-mtcgToken"
            string key = "Authorization";
            if (requestHeaders.ContainsKey(key) && !string.IsNullOrEmpty(requestHeaders[key]))
            {
                string[] splitToken = requestHeaders[key].Split(" ");
                for (int i = splitToken.Length - 1; i >= 0; i--)
                {
                    return splitToken[i].Trim();
                }
            }
            return null;
        }

        private void SendResponse(string response)
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            client.Send(responseBytes);
        }

        private void SendResponseMessage(string message, string responseCode = "200 OK")
        {
            string response = $"HTTP/1.1 {responseCode}\r\nContent-Type: text/plain\r\n\r\n\"{message}\"\r\n";
            SendResponse(response);
        }

        private void SendResponseMessageArray(string[] messageArray, string responseCode = "200 OK")
        {
            string response = $"HTTP/1.1 {responseCode}\r\nContent-Type: application/json -d \r\n\r\n[";
            foreach (string message in messageArray)
            {
                response += $"\"{message}\",";
            }
            if (messageArray.Length > 0)
            {
                response = response.Remove(response.Length - 1, 1);
            }
            response += "]\r\n";
            SendResponse(response);
        }

        private void SendResponseJSON(string json, string responseCode = "200 OK")
        {
            string response = $"HTTP/1.1 {responseCode}\r\nContent-Type: application/json -d \r\n\r\n {json}\r\n";
            SendResponse(response);
        }

        private void SendResponseJSONArray(string[] jsonArray, string responseCode = "200 OK")
        {
            string response = $"HTTP/1.1 {responseCode}\r\nContent-Type: application/json -d \r\n\r\n[";
            foreach (string json in jsonArray)
            {
                response += $"{json},";
            }
            if (jsonArray.Length > 0)
            {
                response = response.Remove(response.Length - 1, 1);
            }
            response += "]\r\n";
            SendResponse(response);
        }

        private void SendResponseExcetion(HttpResponseExcetion responseExcetion)
        {
            string response = $"HTTP/1.1 {responseExcetion.ResponseCode}\r\nContent-Type: text/plain\r\n\r\n\"{responseExcetion.Message}\"\r\n";
            SendResponse(response);
        }

        private void HandleUserRegistration()
        {
            string jsonUserString = requestData.FirstOrDefault();
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

            user = UserRepo.Instance.GetByUsernamePassword(username, password);
            if (user == null)
            {
                throw new InternalServerErrorException("Registration Failed");
            }

            user.AddNewCards(Package.GetStarterPackage());
            foreach (Card card in user.Cards)
            {
                CardRepo.Instance.Add(card);
            }
            string message = user.Username + " was registered";
            SendResponseMessage(message, "201 Created");
        }

        private void HandleLogin()
        {
            string jsonUserString = requestData.FirstOrDefault();
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
                throw new WrongParametersException("Wrong Parameters");
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

        private void HandleCreatePackages()
        {
            string message = "Useless, Cardpacks are random";
            SendResponseMessage(message);
        }

        private void HandleGetCards()
        {
            string token = GetTokenFromHeaders();

            if (token == null)
            {
                throw new NotLoggedInException("Authorization Required (Token)");
            }
            else if (!MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new NotLoggedInException("Not Logged In");
            }

            User user = UserRepo.Instance.GetByToken(token);
            if (user == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            List<Card> cards = CardRepo.Instance.GetCardsOfUser(user.ID);
            if (cards == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            string[] jsonArray = new string[cards.Count];
            for (int i = 0; i < jsonArray.Length; i++)
            {
                if(cards[i] is Monster)
                {
                    jsonArray[i] = JsonSerializer.Serialize(cards[i] as Monster);
                }
                else if (cards[i] is Spell)
                {
                    jsonArray[i] = JsonSerializer.Serialize(cards[i] as Spell);
                }
                else
                {
                    jsonArray[i] = JsonSerializer.Serialize(cards[i]);
                }
            }
            SendResponseJSONArray(jsonArray);
        }

        private void HandleGetDeck()
        {
            string token = GetTokenFromHeaders();

            if (token == null)
            {
                throw new NotLoggedInException("Authorization Required (Token)");
            }
            else if (!MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new NotLoggedInException("Not Logged In");
            }

            User user = UserRepo.Instance.GetByToken(token);
            if (user == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            List<Card> cards = CardRepo.Instance.GetDeckOfUser(user.ID);
            if (cards == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            if (requestGetParams.ContainsKey("format") && requestGetParams["format"] == "plain")
            {
                string[] messageArray = new string[cards.Count];
                for (int i = 0; i < messageArray.Length; i++)
                {
                    messageArray[i] = cards[i].ToString();
                }
                SendResponseMessageArray(messageArray);
            }
            else
            {
                string[] jsonArray = new string[cards.Count];
                for (int i = 0; i < jsonArray.Length; i++)
                {
                    if (cards[i] is Monster)
                    {
                        jsonArray[i] = JsonSerializer.Serialize(cards[i] as Monster);
                    }
                    else if (cards[i] is Spell)
                    {
                        jsonArray[i] = JsonSerializer.Serialize(cards[i] as Spell);
                    }
                    else
                    {
                        jsonArray[i] = JsonSerializer.Serialize(cards[i]);
                    }
                }
                SendResponseJSONArray(jsonArray);
            }
        }

        private void HandleConfigureDeck()
        {
            string token = GetTokenFromHeaders();

            if (token == null)
            {
                throw new NotLoggedInException("Authorization Required (Token)");
            }
            else if (!MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new NotLoggedInException("Not Logged In");
            }

            if(requestData.Count != 4)
            {
                throw new WrongParametersException("Wrong Parameters");
            }

            List<int> newDeckIDs = new List<int>();
            foreach (string idString in requestData)
            {
                int id;
                try
                {
                    id = Convert.ToInt32(idString);
                }
                catch(Exception e)
                {
                    throw new WrongParametersException("Wrong Parameters");
                }
                newDeckIDs.Add(id);
            }

            User user = UserRepo.Instance.GetByToken(token);
            if (user == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            List<Card> cards = CardRepo.Instance.GetCardsOfUser(user.ID);
            if (cards == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            user = new User(user, cards);

            bool comethingChanged = user.ChangeDeck(newDeckIDs);

            foreach (Card card in user.Cards)
            {
                CardRepo.Instance.Update(card);
            }

            if(!comethingChanged)
            {
                throw new InputNotAllowedException("Nothing was changed");
            }

            string message = user.Username + " has changed Deck.";
            SendResponseMessage(message);
        }

        private void HandleAcquirePackages()
        {
            string token = GetTokenFromHeaders();

            if (token == null)
            {
                throw new NotLoggedInException("Authorization Required (Token)");
            }
            else if (!MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new NotLoggedInException("Not Logged In");
            }

            User user = UserRepo.Instance.GetByToken(token);
            if (user == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            List<Card> cards = CardRepo.Instance.GetCardsOfUser(user.ID);
            if (cards == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            user = new User(user, cards);

            //TODO Packet type at requestParams["0"]
            List<Card> newCards = user.BuyPackage();
            if (newCards == null)
            {
                throw new NothingFoundException("Not enough Coins");
            }

            foreach (Card card in newCards)
            {
                CardRepo.Instance.Add(card);
            }
            UserRepo.Instance.Update(user);

            string message = user.Username + " has " + newCards.Count + " new Cards.";
            SendResponseMessage(message, "201 Created");
        }

        private void HandleCreateTradingDeal()
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleCreateTradingDeal";
            SendResponse(response);
        }

        private void HandleGetUserData()
        {
            string token = GetTokenFromHeaders();

            if (token == null)
            {
                throw new NotLoggedInException("Authorization Required (Token)");
            }
            else if (!MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new NotLoggedInException("Not Logged In");
            }

            if (requestPath.Count < 2)
            {
                throw new BadRequestException("Incomplete Path");
            }

            string username = requestPath[1];

            User user = UserRepo.Instance.GetByUsername(username, token);
            if (user == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            SendResponseJSON(JsonSerializer.Serialize(user));
        }

        private void HandleGetStats()
        {
            string token = GetTokenFromHeaders();

            if (token == null)
            {
                throw new NotLoggedInException("Authorization Required (Token)");
            }
            else if (!MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new NotLoggedInException("Not Logged In");
            }

            User user = UserRepo.Instance.GetByToken(token);
            if (user == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            SendResponseJSON(user.GetStatsAsJsonString());
        }

        private void HandleGetScoreboard()
        {
            string token = GetTokenFromHeaders();

            if (token == null)
            {
                throw new NotLoggedInException("Authorization Required (Token)");
            }
            else if (!MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new NotLoggedInException("Not Logged In");
            }

            List<User> users = UserRepo.Instance.GetAll().ToList();
            if (users == null)
            {
                throw new NothingFoundException("No Users found");
            }

            users.OrderByDescending(u => u.Elo);

            string[] jsonArray = new string[users.Count];
            for (int i = 0; i < jsonArray.Length; i++)
            {
                jsonArray[i] = users[i].GetStatsAsJsonString();
            }
            SendResponseJSONArray(jsonArray);
        }

        private void HandleGetTradingDeals()
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleGetTradingDeals";
            SendResponse(response);
        }

        private void HandleEditUserData()
        {
            string token = GetTokenFromHeaders();

            if (token == null)
            {
                throw new NotLoggedInException("Authorization Required (Token)");
            }
            else if (!MTCG_Server.Instance.LoggedInUsers.Contains(token))
            {
                throw new NotLoggedInException("Not Logged In");
            }

            if (requestPath.Count < 2)
            {
                throw new BadRequestException("Incomplete Path");
            }

            string username = requestPath[1];

            User user = UserRepo.Instance.GetByUsername(username, token);
            if (user == null)
            {
                throw new NothingFoundException("User does not Exists");
            }

            string jsonUserString = requestData.FirstOrDefault();
            JsonElement jsonUser = JsonDocument.Parse(jsonUserString).RootElement;

            username = null;
            string bio = null;
            string image = null;

            if (jsonUser.TryGetProperty("Username", out JsonElement usernameElement) && usernameElement.ValueKind != JsonValueKind.Null)
            {
                username = usernameElement.GetString();
            }

            if (jsonUser.TryGetProperty("Bio", out JsonElement bioElement) && bioElement.ValueKind != JsonValueKind.Null)
            {
                bio = bioElement.GetString();
            }

            if (jsonUser.TryGetProperty("Image", out JsonElement imageElement) && imageElement.ValueKind != JsonValueKind.Null)
            {
                image = imageElement.GetString();
            }

            if (UserRepo.Instance.UsernameExists(username))
            {
                throw new InputNotAllowedException("Username Already Exists");
            }

            user.ChangeUserData(username, bio, image);

            UserRepo.Instance.Update(user);

            string message = user.Username + " was changed";
            SendResponseMessage(message);
        }

        private void HandleBattle()
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleBattle";
            SendResponse(response);
        }

        private void HandleDeleteTrading()
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n HandleDeleteTrading";
            SendResponse(response);
        }
    }
}
