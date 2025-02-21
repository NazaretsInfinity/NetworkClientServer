using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Client_HW2
{
    public partial class Form1 : Form
    {
        TcpClient client = null;
        StreamReader sr = null; StreamWriter sw = null;

        delegate void AddTextDelegate(String text);
        //==========extra members end============//

       
        void AddText(string message)
        {
            ChatTextBox.Text += $"\n{message}";
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = ipTextBox.Text;
                int port = Int32.Parse(portTextBox.Text);
                client = new TcpClient(ip, port);
                MessageBox.Show("Succesful connection", "User is connected", MessageBoxButtons.OK);
                NetworkStream stream = client.GetStream();
                sw = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                sr = new StreamReader(stream, Encoding.UTF8);

                Thread ClientUpdating = new Thread(() =>
                { 
                 while (true) 
                 this.Invoke(new AddTextDelegate(AddText), $"\r\n{client.Client.RemoteEndPoint}: {sr.ReadLine()}");
                });
                ClientUpdating.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK);
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if(client != null)
            {
                if(MessageTextBox.Text != "")
                {
                sw.WriteLine($"{MessageTextBox.Text}");
                ChatTextBox.Text += $"\r\n{MessageTextBox.Text}";
                    if (MessageTextBox.Text == "Bye".ToLower())
                    {
                        ChatTextBox.Text += $"\r\n{sr.ReadLine()}";
                        client.Close(); client = null;
                    }
                }
            }
        }
    }
}
