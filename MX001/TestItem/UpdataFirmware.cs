using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestItem
{
    public partial class UpdataFirmware : Form
    {
        public UpdataFirmware(string title = "标题")
        {
            InitializeComponent();
            Text = title;
        }
        public static string Title;

        public static bool CountDown(Func<bool> func, string name, string title = "标题")
        {

            UpdataFirmware box = new UpdataFirmware();
            box.Text = Title = title;
            box.label1.Text = name;
            Task.Run(() =>
            {
                bool Result = func.Invoke();
                try
                {
                    box.Invoke(new Action(() => box.label1.Text = $"{name}： {Title}"));
                    box.DialogResult = Result ? DialogResult.Yes : DialogResult.No;
                }
                catch
                {

                }
            });
            bool result = box.ShowDialog() == DialogResult.Yes;

            return result;
        }

        private void UpdataFirmware_Load(object sender, EventArgs e)
        {
            i = 0;
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
            this.TopMost = true;
        }
        int i = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            i++;
            progressBar1.Value = i;
            //如果執行時間超過，則this.DialogResult = DialogResult.No;
            if (i >= 74)
            {
                DialogResult = DialogResult.No;
                timer1.Enabled = false;
                this.Close();
            }
        }
    }
}
