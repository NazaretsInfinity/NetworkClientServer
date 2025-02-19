using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client_PT3
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if true //for Tcp listener
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 5000); // creating client and connecting to server which has IP and port we set in parameters.

                //Getting the stream of connection for reader and writer
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                Console.Write(reader.ReadLine()); // server sends string where client inputs login (check server_PT3 proj)
                string userName = Console.ReadLine(); 
                writer.WriteLine(userName);

                Console.Write(reader.ReadLine()); // then server sends string where client inputs password
                string password = Console.ReadLine();
                writer.WriteLine(password);

                string response = reader.ReadLine(); // then server sends us message with result of autentification 
                if (response != "Autentification is valid. Type 'exit' to quit.")
                {
                    Console.WriteLine("Access denied");
                    client.Close();
                    return;
                }
                Console.WriteLine(response);
                Console.WriteLine("input or exit");
                

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
                    if (quote == "Quotes limit reached. Please try again later.")
                    {
                        Console.WriteLine(quote);
                        break;
                    }
                    Console.WriteLine($"Received quote: {quote}");
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            } 
#endif
#if true2 // for Udp listener
            string serverAdress = "127.0.0.1";
            int port = 11000;
            UdpClient udpClient = new UdpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAdress), port);

            Console.WriteLine("client active, enter query.");
            while (true)
            {
                Console.Write("query: ");
                string query = Console.ReadLine();
                if (query == "exit")
                {
                    break;
                }
                byte[] requestedBytes = Encoding.UTF8.GetBytes(query);
                udpClient.Send(requestedBytes, requestedBytes.Length, serverEndPoint);

                byte[] responseBytes = udpClient.Receive(ref serverEndPoint);
                string response = Encoding.UTF8.GetString(responseBytes);
                Console.WriteLine($"Server response: {response}");

            }
            udpClient.Close(); 
#endif
        }
    }
}
