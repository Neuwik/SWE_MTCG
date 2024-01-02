using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Data;

namespace MonsterTradingCardsGame
{
    public class MTCG_Server
    {
        public ConcurrentBag<string> LoggedInUsers;
        public static readonly int PORT = 10001;
        public static readonly string URL = "http://127.0.0.1:" + MTCG_Server.PORT;
        private Socket listener;
        private const int backlog = 10;

        private static MTCG_Server instance;
        public static MTCG_Server Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MTCG_Server();
                }
                return instance;
            }
        }

        private MTCG_Server()
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LoggedInUsers = new ConcurrentBag<string>();
        }

        public void Start()
        {
            Console.WriteLine(UserRepo.Instance.ToString());
            Console.WriteLine(CardRepo.Instance.ToString());
            listener.Bind(new IPEndPoint(IPAddress.Loopback, PORT));
            listener.Listen(backlog);
            Console.WriteLine("HTTP-Server gestartet.");
            while (true)
            {
                Socket client = listener.Accept();
                ClientHandler clientHandler = new ClientHandler(client);
                ThreadPool.QueueUserWorkItem(new WaitCallback(clientHandler.Start));
            }
        }

        public void Stop()
        {
            if (listener.Connected)
            {
                listener.Close();
            }
        }
    }
}
