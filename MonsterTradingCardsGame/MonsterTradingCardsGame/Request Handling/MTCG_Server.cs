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
        private readonly int LOGOUTTIMER = 120000; // 2min
        private Dictionary<string, DateTime> loggedInTokens;
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
            loggedInTokens = new Dictionary<string, DateTime>();
        }

        public void Start()
        {
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

        public bool TokenLoggedIn(string token, bool resetTimer = true)
        {
            lock (loggedInTokens)
            {
                if (!loggedInTokens.ContainsKey(token))
                {
                    return false;
                }
                if ((loggedInTokens[token] - DateTime.Now).TotalMilliseconds > LOGOUTTIMER)
                {
                    loggedInTokens.Remove(token);
                    return false;
                }
                if (resetTimer)
                {
                    loggedInTokens[token] = DateTime.Now;
                }
            }
            return true;
        }

        public bool LoginToken(string token)
        {
            if (TokenLoggedIn(token, false))
            {
                return false;
            }

            lock(loggedInTokens)
            {
                loggedInTokens.Add(token, DateTime.Now);
            }
            return true;
        }

        public bool LogoutToken(string token)
        {
            if (!TokenLoggedIn(token, false))
            {
                return false;
            }

            lock (loggedInTokens)
            {
                loggedInTokens.Remove(token);
            }
            return true;
        }
    }
}
