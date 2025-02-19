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
            try
            {
                //Creating client that will connec to the node with ip 127.0.0.1 and port 5000
                TcpClient client = new TcpClient("127.0.0.1", 5000); 

                //Get network stream to read from it or write in it something
                NetworkStream stream = client.GetStream(); // we use 'stream' in inicialization of writer and reader

                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                writer.WriteLine("Hello, server!"); // that's the first thing what we send to the server 
                while (true)
                {
                    Console.Write("Enter query, ya momo: ");
                    string input = Console.ReadLine(); // then we enter query
                    writer.WriteLine(input); // then we send it back to it 

                    if (input.ToLower() == "exit") // if it was exit we stop the client
                    {
                        break;
                    }

                    string quote = reader.ReadLine(); // if it's not then we read what server sent to us next, it's supposed to be a quote.
                    Console.WriteLine($"Received quote: {quote}");
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }  
        
    }
}
