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

        public static readonly int PORT = 10101;
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

            // Beispiel: HTTP GET-Anfrage
            if (request.Contains("GET"))
            {
                HandleGetRequest(client);
            }
            // Beispiel: HTTP PUT-Anfrage
            else if (request.Contains("PUT"))
            {
                HandlePutRequest(client);
            }
            // Beispiel: HTTP POST-Anfrage
            else if (request.Contains("POST"))
            {
                HandlePostRequest(client);
            }
            // Beispiel: HTTP PATCH-Anfrage
            else if (request.Contains("PATCH"))
            {
                HandlePatchRequest(client);
            }
            // Beispiel: HTTP DELETE-Anfrage
            else if (request.Contains("DELETE"))
            {
                HandleDeleteRequest(client);
            }

            client.Close();
        }

        private void SendResponse(Socket client, string response)
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            client.Send(responseBytes);
        }

        private void HandleGetRequest(Socket client)
        {
            // Verarbeite die GET-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nGET-Anfrage erfolgreich verarbeitet.";
            SendResponse(client, response);
        }

        private void HandlePutRequest(Socket client)
        {
            // Verarbeite die PUT-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nPUT-Anfrage erfolgreich verarbeitet.";
            SendResponse(client, response);
        }

        private void HandlePostRequest(Socket client)
        {
            // Verarbeite die POST-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nPOST-Anfrage erfolgreich verarbeitet.";
            SendResponse(client, response);
        }

        private void HandlePatchRequest(Socket client)
        {
            // Verarbeite die PATCH-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nPATCH-Anfrage erfolgreich verarbeitet.";
            SendResponse(client, response);
        }

        private void HandleDeleteRequest(Socket client)
        {
            // Verarbeite die DELETE-Anfrage und erstelle die Antwort
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nDELETE-Anfrage erfolgreich verarbeitet.";
            SendResponse(client, response);
        }
    }
}
