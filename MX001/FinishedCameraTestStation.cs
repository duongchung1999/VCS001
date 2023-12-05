using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VCS001
{
    public class FinishedCameraTestStation
    {
        private SerialPort port1;
        private string Port_Name;
        public Dictionary<string, object> Config = new Dictionary<string, object>();
        #region 补助指令
        private bool OpenPort(string Com_Name, int Baurate)
        {
            port1 = new SerialPort();

            port1.PortName = Com_Name;
            port1.BaudRate = Baurate;
            port1.DataBits = 8;
            port1.StopBits = StopBits.One;
            port1.Parity = Parity.Even;
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
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
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
        private bool SendByte(string ComPort, string value1, int DelayTime)
        {
            try
            {
                if (OpenPort(ComPort, 19200) != true) return false;
                string[] values = value1.Split(' ');
                byte[] byteArray = new byte[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    byteArray[i] = Convert.ToByte(values[i], 16);
                }
                port1.Write(byteArray, 0, byteArray.Length);
                byte[] responseData = { };
                int num = 0;
                //Thread.Sleep(1000);
                //while (num < 10)
                //{
                //    responseData = new byte[port1.BytesToRead];
                //    if (responseData.Length > 2) break;
                //    Thread.Sleep(500);
                //}


                //string result = string.Join(" ", responseData.Select(b => b.ToString()));
                //if (!result.Contains(value1))
                //{
                //    MessageBox.Show($"Respond not matching datasend {result}");
                //    return false;
                //}
                //return (Convert.ToInt32(hex, 16)).ToString();
                return true;
            }
            catch (IOException e)
            {
                //Console.WriteLine("Error: " + e.Message);
                MessageBox.Show("Error: " + e.Message);
                return false;
            }
            finally
            {
                if (port1.IsOpen) port1.Close();
                port1.Dispose();
                Thread.Sleep(50);
            }

        }


        #endregion
        #region Motor Area
        private string PortSend(string Com_name, int Baurate, string cmd)
        {
            //string cmd = "outoff 0001";
            try
            {
                //var Baurate = int.Parse(baurate);
                if (!OpenPort(Com_name, Baurate)) return false.ToString();
                port1.WriteLine(cmd);
                return true.ToString();
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
                return false.ToString();
            }
            finally
            {
                if (port1.IsOpen) port1.Close();
                port1.Dispose();
                Thread.Sleep(50);
            }

        }
        public string S1_OutOff()
        {
            string PortName = (string)Config["VCS001_ComPort"];
            string cmd = "outoff 0001";
            return PortSend(PortName, 115200,cmd);
        }
        public string S1_OutOn()
        {
            string PortName = (string)Config["VCS001_ComPort"];
            string cmd = "outon 0001";
            return PortSend(PortName, 115200, cmd);
        }
        public string S1_P0D_Set()
        {
            string PortName = (string)Config["VCS001_ComPort"];
            string cmd = "abs 0";
            return PortSend(PortName,115200,cmd);
        }
        #endregion
        #region LED Area
        public string Stitching_Led_Off()
        {
            string PortName = (string)Config["VCS001_LedPort"];
            string cmd = "Adjustn2_0000";
            return PortSend(PortName, 9600, cmd);
        }
        public string Stitching_Led_On()
        {
            string PortName = (string)Config["VCS001_LedPort"];
            string cmd = "Adjustn2_0100";
            return PortSend(PortName, 9600, cmd);
        }
        public string SFR_Led_Off()
        {
            string PortName = (string)Config["VCS001_LedPort"];
            string cmd = "Adjustn1_0000";
            return PortSend(PortName, 9600, cmd);
        }
        public string SFR_Led_On()
        {
            string PortName = (string)Config["VCS001_LedPort"];
            string cmd = "Adjustn1_0100";
            return PortSend(PortName, 9600, cmd);
        }
        public string MCC_Led_Off()
        {
            string PortName = (string)Config["VCS001_LedPort"];
            string cmd = "Adjustn3_0000";
            return PortSend(PortName, 9600, cmd);
        }
        public string MCC_Led_On()
        {
            string PortName = (string)Config["VCS001_LedPort"];
            string cmd = "Adjustn3_0100";
            return PortSend(PortName, 9600, cmd);
        }
        public string Shading_Led_Off()
        {
            string PortName = (string)Config["VCS001_LedPort"];
            string cmd = "Adjustn4_0000";
            return PortSend(PortName, 9600, cmd);
        }
        public string Shading_Led_On()
        {
            string PortName = (string)Config["VCS001_LedPort"];
            string cmd = "Adjustn4_0100";
            return PortSend(PortName, 9600, cmd);
        }
        #endregion
    }
}
