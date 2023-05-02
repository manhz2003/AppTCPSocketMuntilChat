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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MuntiChatClient
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;          
        }
        
        // gửi tin đi
        private void btnSend_Click(object sender, EventArgs e)
        {
            send();
            addMessage(txtMessager.Text);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            connect();
        }


        IPEndPoint IP;
        Socket socketClient;

        // kết nối
        void connect()
        {
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2023);
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socketClient.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Không thể kết nối tới server !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread listen = new Thread(receive);
            listen.IsBackground = true;
            listen.Start();
        }

        // đóng kết nối
        void close()
        {
            socketClient.Close();
        }

        // gửi tin
        void send()
        {
            if (txtMessager.Text != string.Empty)
            {
                socketClient.Send(Serialize(txtMessager.Text));
            }
        }

        // nhận tin
        void receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    socketClient.Receive(data);
                    string message = (string)Deserialize(data);
                    addMessage(message);
                }
            }
            catch
            {
                close();
            }
        }

        // add message vào khung chat
        void addMessage(string s)
        {
            lsvMesseger.Items.Add(new ListViewItem() { Text = s });
            txtMessager.Clear();
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

        // đóng kết nối khi đóng form
        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            close();
        }


        private void mnuDangKy_Click(object sender, EventArgs e)
        {
            DangKy dangKy = new DangKy();
            dangKy.Show();
            Hide();
        }

        private void mnuDangNhap_Click(object sender, EventArgs e)
        {
            DangNhap dangNhap = new DangNhap();
            dangNhap.Show();
            Hide();
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không ?", "exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
