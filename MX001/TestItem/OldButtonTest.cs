using MerryTest.testitem;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.testitem
{
    /// <summary>
    /// 按键测试类
    /// </summary>
    public class OldButtonTest
    {
        #region 参数及外部引用
        Command command = new Command();
        string value = "";
        int[] arr;
        bool ErrorFlag = true;
        string type = "";
        private FileStream fsDeviceRead;
        private AsyncCallback AsyRead;
        static string str;
        static bool errorflag = true;
        static int lenght;
        static IntPtr intPtr;
        static bool commandflag;
        ProgressBars msgbox;
        string[] readdata;
        int[] flag;
        string formname = "";


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
            int Msg,            // 消息ID
            int wParam,         // 参数1
            int lParam            // 参数2
        );
        #endregion
        #region 创建异步读取
        private bool CT_CreateFile(string handle)
        {
            IntPtr _createFileHandle = CreateFile(handle,          //文件位置
                            0x40000000 | 0x80000000,          //允许对设备进行读写访问
                            0x1 | 0x2,    //允许对设备进行共享访问
                            IntPtr.Zero,                           //指向空指针（SECURITY_ATTRIBUTES定义文件的安全特性）
                            3,                         //文件必须已存在
                            0x40000000,                  //允许对文件进行重叠操作
                            IntPtr.Zero);                          //指向空指针（如果不为零，则指定一个文件句柄。新文件将从这个文件中复制扩展属性）

            fsDeviceRead = new FileStream(new SafeFileHandle(_createFileHandle, false), FileAccess.Read | FileAccess.Write, 36, true);
            if (fsDeviceRead == null) return false;
            else
            {
                AsyRead = ReadCompleted;
            }

            return true;
        }
        private void BeginAsyncRead()
        {
            byte[] arrInputReport = new byte[_length];
            if (fsDeviceRead != null)
            {
                fsDeviceRead.BeginRead(arrInputReport, 0, _length, AsyRead, arrInputReport);
            }
        }
        #endregion
        #region 进度条
        private bool MessgBox(string name)
        {
            try
            {
                msgbox = new ProgressBars();
                msgbox.label1.Text = name;
                if (msgbox.ShowDialog() == DialogResult.OK)
                {

                    return true;
                }
                else
                {

                    return false;
                }
            }
            catch (Exception err)
            {
                return false;
            }
        }
        #endregion
        #region 异步读取发生时执行的方法
        private void ReadCompleted(IAsyncResult iResult)
        {
            bool testflag = true;
            try
            {
                byte[] arrBuff = (byte[])iResult.AsyncState;
                if (fsDeviceRead != null)
                {
                    fsDeviceRead.EndRead(iResult);
                }
                for (int i = 0; i < readdata.Length; i++)
                {
                    if (Convert.ToByte(arrBuff[flag[i]].ToString(), 16) != Convert.ToByte(readdata[i], 16)) { testflag = false; break; }
                }
                if (testflag)
                {
                    IntPtr ptr = FindWindow(null, formname);
                    if (ptr != IntPtr.Zero)
                        Thread.Sleep(50);
                    msgbox.DialogResult = DialogResult.OK;
                    Thread.Sleep(50);
                    PostMessage(ptr, 0x10, 0, 0);
                    return;
                }
                BeginAsyncRead();
                if (!commandflag)
                {
                    Thread.Sleep(200);
                    command.WriteSend(str, lenght, intPtr);
                }
            }
            catch { }
        }
        #endregion
        int _length = 0;
        /// <summary>
        /// 按键测试
        /// </summary>
        /// <param name="readdata">按键操作对应指令返回值</param>
        /// <param name="indexs">按键操作对应指令返回下標</param>
        /// <param name="name">按键操作对应窗口名</param>
        /// <param name="handle">通道地址</param>
        /// <param name="length">句柄长度，默认为1000</param>
        /// <returns></returns>
        public bool Buttontest(string readdata, string indexs, string name, string handle, int length)
        {
            _length = length;
            string[] flag = indexs.Split(' ');
            commandflag = true;
            this.readdata = readdata.Split(' ');
            this.flag = new int[flag.Length];
            for (int i = 0; i < flag.Length; i++)
            {
                this.flag[i] = Convert.ToInt16(flag[i]);
            }
            if (!CT_CreateFile(handle)) return false;
            BeginAsyncRead();
            return MessgBox(name);

        }
        /// <summary>
        /// jack测试对外方法（一直下指令获取返回值，按键时返回值与不按键时不一致）
        /// </summary>
        /// <param name="str1">指令</param>
        /// <param name="lenght1">指令长度</param>
        /// <param name="intPtr1">通道指针对象</param>
        /// <param name="readdata">返回值</param>
        /// <param name="flag">返回值下标</param>
        /// <param name="name">弹出窗口名</param>
        /// <param name="handle">通道地址</param>
        /// <returns></returns>
        public bool jacktest(string str1, int lenght1, IntPtr intPtr1, string[] readdata, string[] flag, string name, string handle)
        {
            commandflag = false;
            str = str1;
            lenght = lenght1;
            intPtr = intPtr1;
            this.readdata = readdata;
            this.flag = new int[flag.Length];
            for (int i = 0; i < flag.Length; i++)
            {
                this.flag[i] = Convert.ToInt16(flag[i]);
            }
            if (!CT_CreateFile(handle)) return false;
            BeginAsyncRead();
            command.WriteSend(str, lenght, intPtr);
            return MessgBox(name);

        }
    }
}
