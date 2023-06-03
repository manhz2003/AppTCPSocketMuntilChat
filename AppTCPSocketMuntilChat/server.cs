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
            connect();
        }

        IPEndPoint IP = new IPEndPoint(IPAddress.Any, 2222);
        Socket socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clientList = new List<Socket>();

        // kết nối
        void connect()
        {
            socketServer.Bind(IP);

            // luồng lắng nghe các kết nối
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
                            ListViewItem item = new ListViewItem(clientIP);

                            // listView là control của form vì vậy chỉ được phép ghi từ main
                            // hàm invoke giúp đưa các thay đổi từ thread về luồng main.
                            // tham số MethodInvoker là 1 delegate đưa biểu thức không
                            // có tham số vào luồng main chờ thực thi.
                            lsvIP.Invoke((MethodInvoker)(
                                () => lsvIP.Items.Add(item)
                                ));

                            // luồng nhận tin
                            Thread receive = new Thread(Receive);
                            receive.Start(client);
                        }
                    }
                    catch
                    {
                        // xử lý ngoại lệ này để đảm bảo khi các xử trên bị lỗi
                        // ip và socket sẽ được cấp phát lại tránh việc bị dừng chương trình
                        IP = new IPEndPoint(IPAddress.Any, 2222);
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

        // gửi tin cho client
        private void btnSend_Click_1(object sender, EventArgs e)
        {
            if (txtMessager.Text == string.Empty)
            {
                MessageBox.Show("Bạn chưa nhập tin nhắn !", "Thông báo", MessageBoxButtons.OK);
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
        // socket chứa client được nhận từ client được truyền vào hàm start của thread
        // sau đó chuyển sang cho hàm receive nhận vào 1 obj, ta chuyển obj thành socket.
        void Receive(object obj)
        {
            // chuyển đổi đối tượng từ object thành socket và gán vào biến client
            Socket client = obj as Socket;

            try
            {
                while (true)
                {                   
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    string message = (string)Deserialize(data);
                    addMessage(message);                   

                    // gửi tin nhắn của client đến tất cả các client khác.
                    foreach (Socket item in clientList)
                    {
                        // kiểm tra khác null và tránh gửi lặp lại tin nhắn client.
                        if (item != null && item != client)
                            item.Send(Serialize(message));
                    }
                }
            }
            catch
            {
                // xóa socket client khỏi clientList.
                clientList.Remove(client);

                // tìm và xóa địa chỉ IP của client khỏi lsvIP
                for (int i = 0; i < lsvIP.Items.Count; i++)
                {
                    // so sánh địa chỉ của mỗi client có khớp với địa chỉ vừa thoát ra k
                    if (lsvIP.Items[i].Text == client.RemoteEndPoint.ToString())
                    {
                        // xóa vị trí được chỉ định
                        lsvIP.Items.RemoveAt(i); 
                        break;
                    }
                }               
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

        // close server, khi đóng form.
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
