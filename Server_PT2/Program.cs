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
            //Listener that will listen to connections( IPAddress.Any means it will accept available connections on all net interfaces of PC (Ethernet, WI_FI , etc...)
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server started on port 5000...");

            while (true)// endless cycle that will accept incoming client and allocate a thread for each one 
            {
                TcpClient client = server.AcceptTcpClient(); // if client tries to connect, server's supposed to accept it here
                Thread clientThread = new Thread(HandleClient); // and he allocate a specified thread to this client difinitely
                clientThread.Start(client);
            }

        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj; //we give TcpClient here from the main method

            //Info about client himself(his IP and port)
            string clientEndPoint = client.Client.RemoteEndPoint.ToString();

            //Time when user has connected
            DateTime connectedTime = DateTime.Now;

            //here we edit connectionLogs list. There might more than one thread( means clients), so to not make a mess of log, we set a lock on it(synchronize.)
            lock (logLock)
            {
                connectionLogs.Add($"[{connectedTime.ToShortTimeString()}] {clientEndPoint} connected.");
                Console.WriteLine($"[{connectedTime.ToShortTimeString()}] {clientEndPoint} connected.");
            } //Each thread goes over this part of code in order, sitting in queue 'till the thread before 'em is done.


            //Stream of data of certain socket(network connection). Here it's client we accepted
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true }; // we can write data to stream with it (server wites to client)
                                                                          //AutoFlush makes sure that data is sent, we don't need to do it manually 

            StreamReader reader = new StreamReader(stream, Encoding.UTF8); // we can read data from stream with that( server reads from client) 

            try
            {
                string request;
                while ((request = reader.ReadLine()) != null) // HERE WE READ WHAT CLIENT HAS SENT 
                {
                    // if user sent 'exit' we'll kick him as asked for 
                    if (request.ToLower() == "exit")break;
                    
                    Random rand = new Random();
                    string quote = quotes[rand.Next(quotes.Count)];
                    writer.WriteLine(quote); // WRITER HERE send quote to client with 'writeline'
                }
            }
            finally // it must execute, no matter if there was an error or not
            {
                DateTime disconnectedTime = DateTime.Now;
                lock (logLock) // again we update loglist. Lots of threads can have acces to it, so we put in queue, where they pass the code in order.
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
