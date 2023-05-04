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
using System.Data.SqlClient;

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

                            // Lấy địa chỉ IP của client
                            string clientIP = client.RemoteEndPoint.ToString();

                            // Thêm item mới vào lsvIP
                            ListViewItem item = new ListViewItem() { Name = clientIP, Text = clientIP };
                            lsvIP.Invoke((MethodInvoker)(() => lsvIP.Items.Add(item)));

                            Thread receive = new Thread(Receive);
                            receive.Start(client);
                        }
                    }
                    catch
                    {
                        IP = new IPEndPoint(IPAddress.Any, 2023);
                        socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }
                });
            listen.Start();
        }
        // gửi tin
        void send(Socket client)
        {

            if (client != null && txtMessager.Text != string.Empty)
            {
                string ServerChat = "Server: " + txtMessager.Text;
                client.Send(Serialize(ServerChat));
            }
        }

        // gửi tin cho tất cả client
        private void btnSend_Click_1(object sender, EventArgs e)
        {
            if (txtMessager.Text == string.Empty)
            {
                MessageBox.Show("Bạn chưa nhập dữ liệu !", "Thông báo", MessageBoxButtons.OK);
            }
            else
            {
                foreach (Socket item in clientList)
                {
                    send(item);
                }
                string ServerChat = "Server: " + txtMessager.Text;
                addMessage(ServerChat);
                txtMessager.Clear();
            }
        }

        // nhận tin
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

                    // Hiển thị IP của client lên ListView
                    string clientIP = client.RemoteEndPoint.ToString();
                    if (!lsvIP.Items.ContainsKey(clientIP))
                    {
                        // Thêm item mới vào lsvIP
                        lsvIP.Items.Add(new ListViewItem() { Name = clientIP, Text = clientIP });
                    }

                    foreach (Socket item in clientList)
                    {
                        if (item != null && item != client)
                            item.Send(Serialize(message));
                    }
                }
            }
            catch
            {
                clientList.Remove(client);

                // Xóa địa chỉ IP của client khỏi lsvIP của server
                for (int i = 0; i < lsvIP.Items.Count; i++)
                {
                    if (lsvIP.Items[i].Text == client.RemoteEndPoint.ToString())
                    {
                        lsvIP.Items.RemoveAt(i);
                        break;
                    }
                }

                client.BeginDisconnect(false, null, null);
                client.Close();
            }
        }

        // add message vào khung chat
        void addMessage(string s)
        {            
            lsvMesseger.Items.Add(new ListViewItem() { Text = s });
        }

        // chuyển object sang byte
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter Formatter = new BinaryFormatter();
            Formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        // Chuyển byte sang object
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter Formatter = new BinaryFormatter();
            return Formatter.Deserialize(stream);
        }       

        // đóng kết server nối, khi đóng form.
        private void server_FormClosed(object sender, FormClosedEventArgs e)
        {
            socketServer.Close();
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
