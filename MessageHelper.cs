using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;

public class MessageHelper
{
    public const int WM_COPYDATA = 0x004A;


    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PostThreadMessageA(int threadId, uint msg, IntPtr wParam, ref string lParam);

    public static bool Call_Notification(string process, string strMsg)
    {
        System.Diagnostics.Process[] pros = System.Diagnostics.Process.GetProcesses(); //获取本机所有进程
        for (int i = 0; i < pros.Length; i++)
        {
            if (pros[i].ProcessName == process || pros[i].ProcessName == process + ".vshost") //名称为ProcessCommunication的进程
            {
                HiroMsg cds;
                cds.appID = "com.kentaro.hiro.pants";
                cds.message = strMsg;
                cds.appName = "Hiro's Kentaro";
                IntPtr fromWindowHandler = (IntPtr)0;
                int hWnd = pros[i].Threads[0].Id; //获取ProcessCommunication.
                return PostThreadMessageA(hWnd, 0xA417, fromWindowHandler, ref cds.message); //点击该按钮，以文本框数据为参数，向Form1发送WM_KEYDOWN消息
            }
        }
        return false;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HiroMsg
    {
        public string appID;
        public string appName;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string message;
        
    }
}