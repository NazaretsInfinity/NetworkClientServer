using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;

namespace Client_PT2
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if true1
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 5000);
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                writer.WriteLine("Hello, server!");
                while (true)
                {
                    string input = reader.ReadLine();
                    writer.WriteLine(input);

                    if (input.ToLower() == "exit")
                    {
                        break;
                    }

                    string quote = reader.ReadLine();
                    Console.WriteLine($"Received quote: {quote}");
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        } 
#endif
            string IpAdd = "127.0.0.1"; // for server 
            int port = 1202;

            Socket clientToServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientToServer.Connect(new IPEndPoint(IPAddress.Parse(IpAdd), port));
            Console.WriteLine("Client is connected");


            while (true)
            {
                Console.WriteLine("Type query: "); string query = Console.ReadLine();
                if (query == "SendMessage")
                {
                    Console.Write("Enter ip: "); string ip = Console.ReadLine();
                    Console.Write("Enter port: "); string toport = Console.ReadLine();
                    Console.Write("message: "); string message = Console.ReadLine();
                    if (message.Contains("EXIT")) break;
                    clientToServer.Send(Encoding.ASCII.GetBytes($"{ip}\\{toport}\\{message}\\{query}"));
                }

                if (query == "Recieve messages")
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientToServer.Receive(buffer);
                    string response = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine(response);
                }
            }
        }
    }
}
