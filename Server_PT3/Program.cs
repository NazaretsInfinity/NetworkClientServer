using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

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
        private static int MaxQuotesCount = 5; // max quantity of quotes user can ask
        private const int MaxConnections = 3; // max quantity of connections server can accept 
        private static int currentConnections = 0; //current active connections 

        //authentication for users
        private static Dictionary<string, string> users = new Dictionary<string, string>
        {
            {"admin", "admin" },
            {"user2", "pass2" },
            {"user3", "pass3" },
        };

        // quotes for users 
        private static List<string> quotes = new List<string>()
        {"fuk u", "understandable, have a great day", "I fart"};

        private static List<string> connectionLogs = new List<string>();
        private static object logLock = new object();
        #endregion


        static void Main(string[] args)
        {

#if true // TCP LISTENER 

            //creating listener
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server started on port 5000...");

            while (true)//listener is always active to accept connections 
            {
                TcpClient client = server.AcceptTcpClient();
                if (currentConnections >= MaxConnections) // if user is try to connect to full server:
                {
                    //then we get his connection stream
                    NetworkStream stream = client.GetStream();
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                    //and send him notification that this server is full
                    writer.WriteLine("Server is full. Please try again later.");
                    //after he's notified, we close his connection 
                    client.Close();

                    //and continue cycle anew(from the start) 
                    continue;
                }

                Interlocked.Increment(ref currentConnections); // as alternative to the lock 

                Thread clientThread = new Thread(HandleClient); //and creating a thread to trace a client. One client - one thread.
                clientThread.Start(client); 
            }

        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
       
            //After client is connected server takes his connection stream:
            try
            {
                // server send him requirement for login
                writer.WriteLine("login ");
                string userName = reader.ReadLine(); // and wait for his input, then read it

                //then server send him requirement for password of the login
                writer.WriteLine("pass ");
                string password = reader.ReadLine(); // it waits  for the client to send it message. When client does it, server reads the message

                //In dictionary 'users' we initialized, we search for error here 
                if (!users.ContainsKey(userName) || users[userName] != password) //(If the username's valid, then we check if the password's appropriate.
                {
                    writer.WriteLine("Invalid login or password."); // if it's not, server NOTIFICATES CLIENT about it
                    client.Close(); 
                    Interlocked.Decrement(ref currentConnections); // after closing any connection, do not forget to update the counter
                    return;
                }

                string clientEndPoint = client.Client.RemoteEndPoint.ToString(); //we get ip and port of the connected client in string type here.
                DateTime connectedTime = DateTime.Now; //and get the time when connection appeared for loglist
                lock (logLock)
                {
                    connectionLogs.Add($"[{connectedTime.ToShortTimeString()}] {clientEndPoint} connected.");
                    Console.WriteLine($"[{connectedTime.ToShortTimeString()}] {clientEndPoint} connected.");
                }

                writer.WriteLine("Autentification is valid. Type 'exit' to quit."); // server notificates client that his connection is allowed
                int quoteCount = 0; // we set the client his quote counter
                string request;

                while ((request = reader.ReadLine()) != null) // wait for another query from client and read it and process it 
                {
                    if (request.ToLower() == "exit") 
                        break;
                    
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


#if true2 // UDP LISTENER
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
#endif
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
