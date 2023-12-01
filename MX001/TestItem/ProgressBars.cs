using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.testitem
{
    public partial class ProgressBars : Form
    {
        public static bool CountDown(Func<bool> func, string name, string title = "标题")
        {
            var flag = true;
            ProgressBars box = new ProgressBars();
            box.Text = title;
            box.label1.Text = name;

            Thread proThread = new Thread(() =>
            {
                while (flag)
                {
                    if (box.IsDisposed) return;
                    if (func.Invoke())
                    {
                        box.DialogResult = DialogResult.Yes;
                    }
                    Thread.Sleep(150);
                }
            });
            proThread.Start();
            bool result = box.ShowDialog() == DialogResult.Yes;
            flag = false;
            if (proThread.IsAlive) proThread.Abort();
            return result;
        }
        #region 窗体部分

        public ProgressBars()
        {
            InitializeComponent();
        }
        private void ProgressBars_Load(object sender, EventArgs e)
        {
            i = 0;
            timer1.Interval = 500;
            timer1.Enabled = true;
        }
        int i;
        bool flag = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            i = i + 5;
            progressBar1.Value = i;
            //如果執行時間超過，則this.DialogResult = DialogResult.No;
            if (i >= 100)
            {
                DialogResult = DialogResult.No;
                timer1.Enabled = false;
                flag = true;
                Thread.Sleep(100);
                this.Close();
            }
        }



        /// <summary>
        /// 控制窗体
        /// </summary>
        /// <param name="lpClassName">传null</param>
        /// <param name="lpWindowName">需要控制窗体得名字</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        /// <summary>
        /// 结束窗体
        /// </summary>
        /// <param name="hWnd">信息发往的窗口的句柄</param>
        /// <param name="Msg">消息ID ：0x10</param>
        /// <param name="wParam">参数1 ：</param>
        /// <param name="lParam">参数2 ：0</param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        extern static int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        #endregion

    }
}
