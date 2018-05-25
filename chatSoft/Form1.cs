using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace chatSoft
{
    public partial class Form1 : Form
    {

        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string receive;
        public String text_to_send;



        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    textBox3.Text = address.ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)    //uruchomienie Servera
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(textBox4.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;


            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void button1_Click(object sender, EventArgs e)   //przycisk Wyslij
        {
            if (textBox1.Text != "")
            {
                text_to_send = textBox1.Text;
                backgroundWorker2.RunWorkerAsync();

            }
            textBox1.Text = "";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)   //odbieranie danych
        {
            while (client.Connected)
            {
                try
                {
                    receive = STR.ReadLine();
                    this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("You : " + receive + "\n"); }));
                    receive = "";

                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)  //wysylanie danych
        {
            if (client.Connected)
            {
                STW.WriteLine(text_to_send);
                this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("Me : " + text_to_send + "\n"); }));
            }
            else
            {
                MessageBox.Show("Send failed!");
            }
            backgroundWorker2.CancelAsync();
        }

    }
}
