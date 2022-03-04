using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace HiroTest
{
    public partial class MainWindow : Window
    {
        SocketConnection? socketConnection = null;
        int port = 415417;
        string host = "127.0.0.1";
        public MainWindow()
        {
            InitializeComponent();
            InitializePort();
        }

        private void InitializePort()
        {
            port = HiroSocketHelper.Get_Port("Hiro");
            HiroSocketHelper.Write_Ini(AppDomain.CurrentDomain.BaseDirectory + "\\send.ini", "App", "Port", port.ToString());
        }

        private void SendMessage()
        {
            IPAddress ip = IPAddress.Parse(host);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (socketConnection == null)
            {
                InitializePort();
                if (port < 0)
                {
                    Info.Content = "Hiro Not Found";
                    return;
                }
                socketConnection = new SocketConnection(clientSocket);
                socketConnection.Connect(ip, port);
            }
            HiroSocketHelper.Write_Ini(AppDomain.CurrentDomain.BaseDirectory + "send.ini", "App", "ID",ID_Input.Text);
            HiroSocketHelper.Write_Ini(AppDomain.CurrentDomain.BaseDirectory + "send.ini", "App", "Name", Name_Input.Text);
            HiroSocketHelper.Write_Ini(AppDomain.CurrentDomain.BaseDirectory + "send.ini", "App", "Package", Package_Input.Text);
            HiroSocketHelper.Write_Ini(AppDomain.CurrentDomain.BaseDirectory + "send.ini", "App", "Msg", Msg_Input.Text);
            HiroSocketHelper.Write_Ini(AppDomain.CurrentDomain.BaseDirectory + "send.ini", "App", "Location", AppDomain.CurrentDomain.BaseDirectory + "send.ini");
            socketConnection.Send(AppDomain.CurrentDomain.BaseDirectory + "send.ini");
            if (socketConnection != null)
                socketConnection.DataSendCompleted += delegate
                {
                    Info.Content = "Sent to Hiro - " + DateTime.Now.ToString("HH:mm:ss");
                    socketConnection.Dispose();
                    socketConnection = null;
                };
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
    }
}
