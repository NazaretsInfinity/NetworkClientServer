using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_PT3
{
    internal class Program
    {
        #region For Udp
        static Dictionary<string, List<DateTime>> clientsRequests = new Dictionary<string, List<DateTime>>();
        static int maxRequestedPerHour = 10;
        static TimeSpan timeFrame = TimeSpan.FromHours(1);

       
        #endregion

        #region For Tcp
        private static int MaxQuotesCount = 5;
        private const int MaxConnections = 3;
        private static int currentConnections = 0;

        private static Dictionary<string, string> users = new Dictionary<string, string>
        {
            {"admin", "admin" },
            {"user2", "pass2" },
            {"user3", "pass3" },
        };
        private static List<string> connectionLogs = new List<string>();
        private static object logLock = new object();
        #endregion


        static void Main(string[] args)
        {

#if true1 // TCP LISTENER 
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server started on port 5000...");
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                if (currentConnections >= MaxConnections)
                {
                    NetworkStream stream = client.GetStream();
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                    writer.WriteLine("Server is full. Please try again later.");
                    client.Close();
                    continue;
                }
                Interlocked.Increment(ref currentConnections);

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }

        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            /*
            
            */

            try
            {
                writer.WriteLine("login ");
                string userName = reader.ReadLine();
                writer.WriteLine("pass ");
                string password = reader.ReadLine();

                if (!users.ContainsKey(userName) || users[userName] != password)
                {
                    writer.WriteLine("Invalid login or password.");
                    client.Close();
                    Interlocked.Decrement(ref currentConnections);
                    return;
                }

                string clientEndPoint = client.Client.RemoteEndPoint.ToString();
                DateTime connectedTime = DateTime.Now;
                lock (logLock)
                {
                    connectionLogs.Add($"[{connectedTime.ToShortTimeString()}] {clientEndPoint} connected.");
                    Console.WriteLine($"[{connectedTime.ToShortTimeString()}] {clientEndPoint} connected.");
                }

                writer.WriteLine("Autentification is valid. Type 'exit' to quit.");
                int quoteCount = 0;
                string request;

                while ((request = reader.ReadLine()) != null)
                {
                    if (request.ToLower() == "exit")
                    {
                        break;
                    }
                    if (quoteCount >= MaxQuotesCount)
                    {
                        writer.WriteLine("Quotes limit reached. Please try again later.");
                        break;
                    }
                    Random rand = new Random();
                    string quote = quotes[rand.Next(quotes.Count)];
                    writer.WriteLine(quote);
                    quoteCount++;
                }
            }
            finally
            {
                Interlocked.Decrement(ref currentConnections);
                Console.WriteLine("Client disconnected...");
                client.Close();
            }
#endif

            
            int port = 11000;
            UdpClient udpServer = new UdpClient(port);
            Console.WriteLine("server active");
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
            while (true)
            {
                try
                {
                    byte[] recivedBytes = udpServer.Receive(ref remoteEndPoint);
                    string recivedData = Encoding.UTF8.GetString(recivedBytes);
                    Console.WriteLine("Received from {0}:{1}: {2}", remoteEndPoint.Address, remoteEndPoint.Port, recivedData);

                    if (!isRequestAllowed(remoteEndPoint.ToString()))
                    {
                        string response = "limit is out of range";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        udpServer.Send(responseBytes, responseBytes.Length, remoteEndPoint);
                        Console.WriteLine("Sent to {0}:{1}: {2}", remoteEndPoint.Address, remoteEndPoint.Port, response);
                    }
                    else
                    {
                        string response = GetComponentPrice(recivedData);
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        udpServer.Send(responseBytes, responseBytes.Length, remoteEndPoint);
                        Console.WriteLine("Sent to {0}:{1}: {2}", remoteEndPoint.Address, remoteEndPoint.Port, response);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static bool isRequestAllowed(string clientAddress)
        {
            if (!clientsRequests.ContainsKey(clientAddress))
            {
                clientsRequests[clientAddress] = new List<DateTime>();
            }
            clientsRequests[clientAddress].RemoveAll(time => DateTime.Now - time > timeFrame);
            if (clientsRequests[clientAddress].Count >= maxRequestedPerHour)
            {
                return false;
            }
            clientsRequests[clientAddress].Add(DateTime.Now);
            return true;
        }


        private static string GetComponentPrice(string componentName)
        {
            switch (componentName.ToLower())
            {
                case "cpu":
                    return "CPU price: $1000";
                case "ram":
                    return "RAM price: $500";
                case "motherboard":
                    return "Motherboard price: $2000";
                default:
                    return "Component not found";
            }
        }
    }
}
