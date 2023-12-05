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
    public class MotorControl
    {
        private SerialPort port1;
        private string Port_Name;
        #region 补助指令
        private string OpenPort(string Com_Name, int Baurate)
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
        private bool SendByte(string ComPort, string value1, int DelayTime)
        {
            try
            {
                if (OpenPort(ComPort, 19200) != true.ToString()) return false;
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
        
        public bool ConnectPort(string Comport)
        {
            Port_Name = Comport;
            return true;
        }
        public string S1_Reset()
        {
            string dataset = "01 05 00 0F FF 00 BC 39";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S1_P0D()
        {
            string dataset = "01 06 11 94 00 00 CD 1A";
            string dataControl = "01 05 00 50 FF 00 8C 2B";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            Thread.Sleep(3000);
            if (!SendByte(Port_Name, dataControl, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S1_P45D()
        {
            string dataset = "01 06 11 94 01 C2 4D 1B";
            string dataControl = "01 05 00 50 FF 00 8C 2B";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            Thread.Sleep(3000);
            if (!SendByte(Port_Name, dataControl, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S1_P90D()
        {
            string dataset = "01 06 11 96 03 84 6C 49";
            string dataControl = "01 05 00 51 FF 00 DD EB";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            Thread.Sleep(3000);
            if (!SendByte(Port_Name, dataControl, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S1_P135D()
        {
            string dataset = "01 06 11 98 05 46 8F BB";
            string dataControl = "01 05 00 52 FF 00 2D EB";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            Thread.Sleep(3000);
            if (!SendByte(Port_Name, dataControl, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S1_P180D()
        {
            string dataset = "01 06 11 9A 07 08 AF 2F";
            string dataControl = "01 05 00 53 FF 00 7C 2B";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            Thread.Sleep(3000);
            if (!SendByte(Port_Name, dataControl, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S1_P225D()
        {
            string dataset = "01 06 11 9C 08 CA CB 4F";
            string dataControl = "01 05 00 54 FF 00 CD EA";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            Thread.Sleep(3000);
            if (!SendByte(Port_Name, dataControl, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S1_P270D()
        {
            string dataset = "01 06 11 9E 0A 8C EA 1D";
            string dataControl = "01 05 00 55 FF 00 9C 2A";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            Thread.Sleep(3000);
            if (!SendByte(Port_Name, dataControl, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S1_P315D()
        {
            string dataset = "01 06 11 9E 0C 4E 68 2C";
            string dataControl = "01 05 00 55 FF 00 9C 2A";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            Thread.Sleep(3000);
            if (!SendByte(Port_Name, dataControl, 1000)) return false.ToString();
            return true.ToString();
        }
        #endregion

        #region Cylinder Area
        public string S2_P0_On()
        {
            string dataset = "01 05 00 67 FF 00 3D E5";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S2_P0_Off()
        {
            string dataset = "01 05 00 67 00 00 7C 15";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S2_P1_On()
        {
            string dataset = "01 05 00 65 FF 00 9C 25";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string S2_P1_Off()
        {
            string dataset = "01 05 00 65 00 00 DD D5";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        #endregion

        #region Backlight Control Area
        public string Stitching_Led_On()
        {
            string dataset = "01 05 00 69 FF 00 5C 26";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string Stitching_Led_Off()
        {
            string dataset = "01 05 00 69 00 00 1D D6";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string SFR_12_Led_On()
        {
            string dataset = "01 05 00 6B FF 00 FD E6";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string SFR_12_Led_Of()
        {
            string dataset = "01 05 00 6B 00 00 BC 16";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string SFR_07_Led_On()
        {
            string dataset = "01 05 00 6D FF 00 1D E7";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string SFR_07_Led_Of()
        {
            string dataset = "01 05 00 6D 00 00 5C 17";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string Shading_Led_On()
        {
            string dataset = "01 05 00 6F FF 00 BC 27";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        public string Shading_Led_Off()
        {
            string dataset = "01 05 00 6F 00 00 FD D7";
            if (!SendByte(Port_Name, dataset, 1000)) return false.ToString();
            return true.ToString();
        }
        #endregion


    }
}
