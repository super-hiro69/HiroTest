using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HiroTest
{
    public class HiroSocket
    {
        public Socket ClientSocket { get; set; }
        public string IP;
        public HiroSocket(Socket clientSocket)
        {
            ClientSocket = clientSocket;
            IP = GetIPStr();
        }
        public string GetIPStr()
        {
            var re = ClientSocket.RemoteEndPoint;
            string resStr = (re != null) ? ((IPEndPoint)re).Address.ToString() : string.Empty;
            return resStr;
        }
    }
    public class SocketConnection : IDisposable
    {
        public Byte[] msgBuffer = new byte[1024];
        private Socket? _clientSocket = null;
        public Socket? ClientSocket
        {
            get { return this._clientSocket; }
        }
        #region 构造
        public SocketConnection(Socket sock)
        {
            this._clientSocket = sock;
        }
        #endregion
        #region 连接
        public void Connect(IPAddress ip, int port)
        {
            if (ClientSocket != null)
                ClientSocket.BeginConnect(ip, port, ConnectCallback, ClientSocket);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.AsyncState != null)
                {
                    Socket handler = (Socket)ar.AsyncState;
                    handler.EndConnect(ar);
                }
            }
            catch
            {

            }
        }
        #endregion
        #region 发送数据
        public void Send(string data)
        {
            Send(Encoding.UTF8.GetBytes(data));
        }
        private void Send(byte[] byteData)
        {
            try
            {
                if (ClientSocket != null)
                    ClientSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), ClientSocket);
            }
            catch
            {

            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.AsyncState != null)
                {
                    Socket handler = (Socket)ar.AsyncState;
                    handler.EndSend(ar);
                    if (this != null)
                        OnDataSendCompleted(new());
                }
            }
            catch
            {

            }
        }
        #endregion
        #region 接收数据
        public void ReceiveData()
        {
            if (ClientSocket != null)
                ClientSocket.BeginReceive(msgBuffer, 0, msgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket != null)
                {
                    int REnd = ClientSocket.EndReceive(ar);
                    if (REnd > 0)
                    {
                        byte[] data = new byte[REnd];
                        Array.Copy(msgBuffer, 0, data, 0, REnd);
                        OnDataRecevieCompleted(new());
                        ClientSocket.BeginReceive(msgBuffer, 0, msgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
                    }
                    else
                    {
                        Dispose();
                    }
                }
                else
                {
                    Dispose();
                }

            }
            catch
            {

            }
        }
        public void Dispose()
        {
            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.Shutdown(SocketShutdown.Both);
                    ClientSocket.Close();
                }       
            }
            catch
            {

            }
        }
        protected virtual void OnDataSendCompleted(EventArgs e)
        {
            EventHandler<EventArgs>? handler = DataSendCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnDataRecevieCompleted(EventArgs e)
        {
            EventHandler<EventArgs>? handler = DataRecevieCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<EventArgs>? DataRecevieCompleted;
        public event EventHandler<EventArgs>? DataSendCompleted;
        #endregion
    }

    public class HiroSocketHelper
    {
        public static int Get_Port(String name)
        {
            System.Diagnostics.Process[] pros = System.Diagnostics.Process.GetProcesses(); //获取本机所有进程
            foreach (var pro in pros)
            {
                if (pro.ProcessName.Equals(name) || pro.ProcessName.Equals(name + ".vshost"))
                {
                    try
                    {
                        var mm = pro.MainModule;
                        if (mm != null && mm.FileName != null)
                        {
                            var str = mm.FileName;
                            str = str.Substring(0, str.LastIndexOf("\\"));
                            str = str + "\\users\\" + Environment.UserName + "\\config\\" + Environment.UserName + ".hus";
                            str = Read_Ini(str, "config", "port", "null");
                            int port = int.Parse(str);
                            return port;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return -1;
        }

        public static string Read_Ini(string iniFilePath, string Section, string Key, string defaultText)
        {
            if (System.IO.File.Exists(iniFilePath))
            {
                byte[] buffer = new byte[1024];
                int ret = GetPrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(defaultText), buffer, 1024, iniFilePath);
                return Encoding.GetEncoding("utf-8").GetString(buffer, 0, ret).Trim();
            }
            else
            {
                return defaultText;
            }
        }

        public static bool Write_Ini(string iniFilePath, string Section, string Key, string Value)
        {
            try
            {
                if (!System.IO.File.Exists(iniFilePath))
                    System.IO.File.Create(iniFilePath).Close();
                long OpStation = WritePrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(Value), iniFilePath);
                if (OpStation == 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }

        }

        [System.Runtime.InteropServices.DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);

        [System.Runtime.InteropServices.DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);
    }
}