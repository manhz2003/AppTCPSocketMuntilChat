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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MuntiChatClient
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }
       
        IPEndPoint IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2222);
        Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        bool isConnected = false;

        // connect
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {                
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
                listen.Start();
            }
            else
            {
                MessageBox.Show("Bạn đã kết nối tới server từ trước  !", "Thông báo", MessageBoxButtons.OK);
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

        // biến global lưu tên.
        private string userName;

        // Hàm lưu giá trị textBox vào biến userName.
        private void btnName_Click(object sender, EventArgs e)
        {
            if (txtName.Text != "")
            {
                userName = txtName.Text;
                txtName.Clear();
                MessageBox.Show("Tên đã được lưu: " + userName);
            }
            else
            {
                MessageBox.Show("Bạn chưa nhập tên !");
            }
        }

        // gửi tin
        private void btnSend_Click_1(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Bạn chưa kết nối tới server !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Bạn chưa nhập tên !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtMessager.Text))
            {
                MessageBox.Show("Bạn chưa nhập nội dung chat !", "Thông báo", MessageBoxButtons.OK);
            }
            else
            {
                string message = string.Format("{0}: {1}", userName, txtMessager.Text);
                socketClient.Send(Serialize(message));
                addMessage(message);
            }
        }

        // add message vào listView
        void addMessage(string s)
        {
            lsvMesseger.Items.Add(new ListViewItem() { Text = s });
            txtMessager.Clear();
        }

        // Chuyển object thành mảng byte.
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter Formatter = new BinaryFormatter();
            Formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

      
        // Chuyển mảng byte thành object.
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter Formatter = new BinaryFormatter();
            return Formatter.Deserialize(stream);
        }

        // đóng kết nối client khi close form.
        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            socketClient.Close();
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không ?", "exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        
        
    }
}
