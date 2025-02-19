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
#if true
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
          
        }
    }
}
