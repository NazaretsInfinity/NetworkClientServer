using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;

namespace Server_PT2
{
    internal class Program
    {
        private static List<string> connectionLogs = new List<string>();
        private static readonly object logLock = new object();


        private static List<string> quotes = new List<string>()
        {"fuk u", "understandable, have a great day", "I fart"};

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server started on port 5000...");
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }

        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            string clientEndPoint = client.Client.RemoteEndPoint.ToString();
            DateTime connectedTime = DateTime.Now;
            lock (logLock)
            {
                connectionLogs.Add($"[{connectedTime.ToShortTimeString()}] {clientEndPoint} connected.");
                Console.WriteLine($"[{connectedTime.ToShortTimeString()}] {clientEndPoint} connected.");
            }

            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            try
            {
                string request;
                while ((request = reader.ReadLine()) != null)
                {
                    if (request.ToLower() == "exit")break;
                    
                    Random rand = new Random();
                    string quote = quotes[rand.Next(quotes.Count)];
                    writer.WriteLine(quote);
                }
            }
            finally
            {
                DateTime disconnectedTime = DateTime.Now;
                lock (logLock)
                {
                    connectionLogs.Add($"[{disconnectedTime.ToShortTimeString()}] {clientEndPoint} disconnected.");
                    Console.WriteLine($"[{disconnectedTime.ToShortTimeString()}] {clientEndPoint} disconnected.");
                }

                Console.WriteLine("Client disconnected...");
                client.Close();
            }
        }
    }
}
