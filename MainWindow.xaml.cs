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
        string sfile = AppDomain.CurrentDomain.BaseDirectory + "\\send.hmsg";
        string hiro = "Hiro";
        public MainWindow()
        {
            InitializeComponent();
            InitializeInfo();
        }

        private void InitializeInfo()
        {
            ID_Input.Text = HiroSocketHelper.Read_Ini(sfile, "App", "ID", String.Empty);
            Name_Input.Text = HiroSocketHelper.Read_Ini(sfile, "App", "Name", String.Empty);
            Package_Input.Text = HiroSocketHelper.Read_Ini(sfile, "App", "Package", String.Empty);
            Msg_Input.Text = HiroSocketHelper.Read_Ini(sfile, "App", "Msg", String.Empty);
            InitializePort();
        }

        private void InitializePort()
        {
            var file = HiroSocketHelper.Get_FileLocation("Hiro");
            port = int.Parse(HiroSocketHelper.Read_Ini(file, "config", "port", "-1"));
            hiro = HiroSocketHelper.Read_Ini(file, "config", "customhiro", "Hiro");
            Title = String.Format("Send to {0}", hiro);
        }

        private void SendMessage()
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            InitializePort();
            if (port < 0)
            {
                Info.Content = String.Format("{0} Not Found", hiro); ;
                return;
            }
            socketConnection = new SocketConnection(clientSocket);
            socketConnection.Connect(IPAddress.Parse(host), port);
            HiroSocketHelper.Write_Ini(sfile, "App", "ID",ID_Input.Text);
            HiroSocketHelper.Write_Ini(sfile, "App", "Name", Name_Input.Text);
            HiroSocketHelper.Write_Ini(sfile, "App", "Package", Package_Input.Text);
            HiroSocketHelper.Write_Ini(sfile, "App", "Msg", Msg_Input.Text);
            socketConnection.Send(sfile);
            Info.Content = String.Format("Sent to {0} - {1}", hiro, DateTime.Now.ToString("HH:mm:ss"));
            socketConnection.Dispose();
            socketConnection = null;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
    }
}
