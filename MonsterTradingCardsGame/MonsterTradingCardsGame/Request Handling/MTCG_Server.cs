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
        private const int backlog = 10;
        private List<ClientHandler> clients;

        public MTCG_Server()
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<ClientHandler>();
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
            clients.Add(new ClientHandler(client));
        }
    }
}
