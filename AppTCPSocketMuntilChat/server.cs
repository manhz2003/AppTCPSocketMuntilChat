using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace AppTCPSocketMuntilChat
{
    public partial class server : Form
    {
        public server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            connect();
        }

        // gửi tin cho tất cả client
        private void btnSend_Click_1(object sender, EventArgs e)
        {
            foreach (Socket item in clientList)
            {
                send(item);

            }
            addMessage(txtMessager.Text);
            txtMessager.Clear();
        }


        IPEndPoint IP;
        Socket socketServer;
        List<Socket> clientList;

        // kết nối
        void connect()
        {
            clientList = new List<Socket>();
            IP = new IPEndPoint(IPAddress.Any, 2023);
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            socketServer.Bind(IP);
            Thread listen = new Thread(
                () =>
                {
                   try
                    {
                        while (true)
                        {
                            socketServer.Listen(100);
                            Socket client = socketServer.Accept();
                            clientList.Add(client);

                            Thread receive = new Thread(Receive);
                            receive.IsBackground = true;
                            receive.Start(client);
                        }
                    }
                    catch
                    {
                        IP = new IPEndPoint(IPAddress.Any, 2023);
                        socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }
                });
            listen.IsBackground = true;
            listen.Start();
        }

        // đóng kết nối
        void close()
        {
            socketServer.Close();
        }

        // gửi tin
        void send(Socket client)
        {
            if ( client != null && txtMessager.Text != string.Empty)
            {
                client.Send(Serialize(txtMessager.Text));
            }

        }

        // nhận tin
        void Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    string message = (string)Deserialize(data);
                    addMessage(message);

                    foreach(Socket item in clientList)
                    {
                        if(item != null && item != client)
                        item.Send(Serialize(message));
                    }

                }
            }
            catch
            {
                clientList.Remove(client);
                client.Close();
            }
        }

        // add message vào khung chat
        void addMessage(string s)
        {
            lsvMesseger.Items.Add(new ListViewItem() { Text = s });
        }

        // phân mảnh
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter Formatter = new BinaryFormatter();
            Formatter.Serialize(stream, obj);

            return stream.ToArray();
        }

        // gom mảnh
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter Formatter = new BinaryFormatter();
            return Formatter.Deserialize(stream);


        }

        // đóng kết nối
        private void server_FormClosed(object sender, FormClosedEventArgs e)
        {
            close();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát server không ?", "exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
