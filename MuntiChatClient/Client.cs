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
        }

        IPEndPoint IP;
        Socket socketClient;
        bool isConnected = false;
        // connect
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2023);
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socketClient.Connect(IP);
                    MessageBox.Show("Kết nối thành công !", "Thông báo", MessageBoxButtons.OK);
                    isConnected = true;
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
            else
            {
                MessageBox.Show("Bạn đã kết nối từ trước rồi !", "Thông báo", MessageBoxButtons.OK);
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
                socketClient.Close();
            }
        }

        // gửi tin
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtMessager.Text == string.Empty)
            {
                MessageBox.Show("Bạn chưa nhập dữ liệu !", "Thông báo", MessageBoxButtons.OK);
            }
            else
            {
                socketClient.Send(Serialize(txtMessager.Text));
                addMessage(txtMessager.Text);
            }            
        }        

        // add message vào khung chat
        void addMessage(string s)
        {
            lsvMesseger.Items.Add(new ListViewItem() { Text = s });
            txtMessager.Clear();
        }

        // Chuyển mảng byte thành đối tượng.
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter Formatter = new BinaryFormatter();
            Formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        // Chuyển đối tượng thành mảng byte.
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter Formatter = new BinaryFormatter();
            return Formatter.Deserialize(stream);
        }

        // đóng kết nối client khi đóng form.
        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            socketClient.Close();
        }

        // menu
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
