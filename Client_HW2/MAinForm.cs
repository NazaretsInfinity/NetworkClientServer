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

namespace Client_HW2
{
    public partial class Form1 : Form
    {
        TcpClient client = null;
        StreamReader sr = null; StreamWriter sw = null;

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
                sw.WriteLine($"{MessageTextBox.Text}");
                ChatTextBox.Text += $"\r\n{MessageTextBox.Text}";
            }
        }
    }
}
