﻿using System;
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
#if true1 //for Tcp listener
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 5000);
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                Console.Write("login ");
                string userName = Console.ReadLine();
                writer.WriteLine(userName);

                Console.Write("pass ");
                string password = Console.ReadLine();
                writer.WriteLine(password);

                string response = reader.ReadLine();
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
        }
    }
}
