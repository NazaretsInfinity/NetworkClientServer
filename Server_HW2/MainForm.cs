using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Server_HW2
{
    public partial class MainForm : Form
    {
        //========== extra members ===========//
        string[] RandomMessages = { "yoyyoynoiyoy", "hi bi@##", "!($@$%!!!!!!0", "YOU SMELL LIKE A PLAY DOH"};
        private bool empty = true; // see if server is free to connect 

        TcpClient client = null;
        StreamReader sr = null; StreamWriter sw= null;

        Random rnd = new Random();
        delegate void AddTextDelegate(String text);
        //==========extra members end============//

        //FOR ADDING TEXTS FROM THREADS( for invoke)
        void AddText(string message)
        {
            ChatTextBox.Text += $"\n{message}";
        }

        
        public MainForm()
        {
            InitializeComponent();
            TcpListener server = new TcpListener(IPAddress.Any, 1202);
            server.Start(); MessageBox.Show("Serves started", "launcher", MessageBoxButtons.OK);

            Thread clientsConnection = new Thread(() =>
            {
                while (true)
                {
                    client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    sw = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                    if (!empty)
                    {
                        sw.WriteLine("Server is taken by another client right now");
                        client.Close();
                        sw.Close();
                        stream.Close();
                        continue;
                    }
                    empty = !empty;


                    sr = new StreamReader(stream, Encoding.UTF8);
                    while(client  != null)
                    {
                        string message = sr.ReadLine();
                        this.Invoke(new AddTextDelegate(AddText), $"\r\n{client.Client.RemoteEndPoint}: {message}");
                        if (message == "Bye"){ sw.WriteLine("Disconnect."); client.Close(); }
                    }
                }

            });
            clientsConnection.IsBackground = true;
            clientsConnection.Start();

        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            if (client != null) 
            {
                if (MessageTextBox.Text == "") // message from system 
                {
                    string mes = RandomMessages[rnd.Next(4)];
                    sw.WriteLine(mes);
                    ChatTextBox.Text += $"\r\n{mes}";
                }
                else // user types the message
                {
                    sw.WriteLine(MessageTextBox.Text);
                    ChatTextBox.Text += $"\r\n{MessageTextBox.Text}";
                }
            }

        }
    }
}
