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

                    writer.WriteLine(input); // then we send it to the server. SERVER IS THE ONE WHOM WE WRITE QUERIES(mostly)

                    // if the query was 'exit' we stop the client
                    if (input.ToLower() == "exit") // btw we bear in mind the registry 
                        break;

                    // if it's anything else, then server already received it and sent us another packet(from the line a bit upper). We read it
                    string quote = reader.ReadLine(); 
                    Console.WriteLine($"Received quote: {quote}");
                }

                //don't forget to close the connection after leaving the loop
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }  
        
    }
}
