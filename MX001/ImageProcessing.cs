using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VCS001
{
    public class ImageProcessing
    {
        private string POT_Image_path = @"C:\VCS001\POT_image_test";
        private string Image_Check_Path = @"C:\VCS001\POT_image_test\test";
        #region 补助指令
        private Process p;
        private string ThreadStr;
        private SerialPort port1;

        private string CallCmd(string args, bool returnflag, string returnvalue = "Success", int TimiOut = 30)
        {
            try
            {

                if (p == null)
                {
                    p = new Process();
                    p.StartInfo.FileName = $"cmd";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.CreateNoWindow = false;//hidden
                    p.StartInfo.RedirectStandardError = true;
                    p.Start();
                    p.StandardInput.WriteLine(@"C:");
                    p.StandardOutput.DiscardBufferedData();
                    Thread ReadThread = new Thread(readCMD);
                    ReadThread.Start();
                    Thread.Sleep(2000);
                }
                ThreadStr = "";
                bool flag = false;
                p.StandardInput.WriteLine($"{args}");
                if (returnflag) return "True";
                for (int i = 0; i < TimiOut; i++)
                {

                    if (ThreadStr.Contains(returnvalue))
                    {
                        flag = true;
                        break;
                    }
                    else if (ThreadStr.Contains("Failed"))
                    {
                        flag = false;
                        break;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
                return flag ? "True" : $"False";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return $"{ex.Message} False";
            }
        }
        private void readCMD()
        {

            while (!p.StandardOutput.EndOfStream)
            {
                string readstr = p.StandardOutput.ReadLine();
                ThreadStr += readstr;
            };
        }
        private string OpenPort(string Com_Name, int Baurate)
        {
            port1 = new SerialPort();

            port1.PortName = Com_Name;
            port1.BaudRate = Baurate;
            port1.DataBits = 8;
            port1.StopBits = StopBits.One;
            port1.Parity = Parity.None;
            try
            {
                if (!port1.IsOpen)
                {
                    port1.Open();
                }
                else
                {
                    port1.Close();
                    Thread.Sleep(100);
                    port1.Open();
                }
                return true.ToString();
            }
            catch
            {
                return "Open has not successeed";
            }
        }
        private string ClosePort()
        {
            try
            {
                if (port1.IsOpen) port1.Close();
                port1.Dispose();
                Thread.Sleep(50);
                return true.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false.ToString();
            }
        }
        public string Send_args(string path, string args, string CompareValue)
        {
            string Resule;
            ThreadStr = "";
            Resule = CallCmd(path, true);
            if (Resule != "True") return path + "False";
            Resule = CallCmd(args, false, CompareValue, 30);
            if (Resule != "True") return args + "False";
            if (ThreadStr.Contains("Not found")) return $"False {ThreadStr}";
            return ThreadStr;
        }
        #endregion
        #region ImageProcess Area
        public string Read_SFR12m_Cam0()
        {
            string args = $"opt_sfr_1.2m.exe {Image_Check_Path}\\cam0_sfr_1.2.jpg";
            string CompareValue = "Img_Center";
            var Value = Send_args(POT_Image_path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        #endregion
    }
}
