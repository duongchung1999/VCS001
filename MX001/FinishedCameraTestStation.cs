using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private string Photograph_Path = @"C:\VCS001\PhotographPath";
        #region 补助指令
        private Process p;
        private string ThreadStr;
        //private string _UseforImgProcess;
        public string Send_args(string path, string args, string CompareValue, int number)
        {
            string Resule;
            ThreadStr = "";
            Resule = CallCmd(path, true);
            if (Resule != "True") return path + "False";
            Resule = CallCmd(args, false, CompareValue, number);
            if (Resule != "True") return args + "False";
            if (ThreadStr.Contains("Not found")) return $"False {ThreadStr}";
            return ThreadStr;
        }
        public string Send_args_in_current_path(string args, string CompareValue)
        {
            string Resule;
            ThreadStr = "";
            Resule = CallCmd(args, false, CompareValue, 30);
            if (Resule != "True") return args + "False";
            if (ThreadStr.Contains("Not found")) return $"False {ThreadStr}";
            return ThreadStr;
        }
        private string CallCmd(string args, bool returnflag, string returnvalue = "Success", int TimiOut = 10)
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
                    p.StartInfo.CreateNoWindow = true;//hidden
                    p.StartInfo.RedirectStandardError = true;
                    p.Start();
                    p.StandardInput.WriteLine(@"C:");
                    p.StandardOutput.DiscardBufferedData();
                    Thread ReadThread = new Thread(readCMD);
                    ReadThread.Start();
                    Thread.Sleep(200);
                }
                ThreadStr = "";
                bool flag = false;
                p.StandardInput.WriteLine($"{args}");
                if (returnflag) return "True";
                //Thread.Sleep(200);
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
                    //string Command_Log_Path = $@"{ImageBackup_path}\CommandLog.txt";
                    //if (!Directory.Exists(ImageBackup_path)) Directory.CreateDirectory(ImageBackup_path);
                    //if (!File.Exists(Command_Log_Path)) File.Create(Command_Log_Path);
                    //string oldContants = File.ReadAllText(Command_Log_Path);
                    //File.WriteAllText(Command_Log_Path, $"{oldContants}Command: {args}\nRespond{_UseforImgProcess}\n\n");
                    Thread.Sleep(1000);
                }
                //Thread.Sleep(1000);
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
                ThreadStr += readstr+"\n";
            };
        }
        private bool OpenPort(string Com_Name, int Baurate)
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
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private bool OpenPort_2(string Com_Name, int Baurate)
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
        #region Cylinder Area
        private string PortSend(string Com_name, int Baurate, string cmd)
        {
            //string cmd = "outoff 0001";
            try
            {
                //var Baurate = int.Parse(baurate);
                if (!OpenPort(Com_name, Baurate)) return false.ToString();
                cmd += "\r\n";
                port1.Write(cmd);
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
        private string PortSend_2(string Com_name, int Baurate, string cmd)
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
        public string S1_OutOff(string PortName)
        {
            //string PortName = (string)Config["VCS001_S1Port"];
            string cmd = "outoff 0001";
            return PortSend(PortName, 115200,cmd);
        }
        public string S1_OutOn(string PortName)
        {
            string cmd = "outon 0001";
            return PortSend(PortName, 115200, cmd);
        }
        public string S1_P0D_Set(string PortName)
        {
            string cmd = "abs 0";
            return PortSend(PortName,115200,cmd);
        }

        #endregion
        #region Motor Area
        public string S2_P0(string PortName)
        {
            string cmd = "abs 110";
            return PortSend(PortName, 115200, cmd);
        }
        #endregion
        #region LED Area
        public string Stitching_Led_Off(string PortName)
        {
            string cmd = "Adjustn2_0000";
            return PortSend(PortName, 9600, cmd);
        }
        public string Stitching_Led_On(string PortName)
        {
            string cmd = "Adjustn2_0100";
            return PortSend(PortName, 9600, cmd);
        }
        public string SFR_Led_Off(string PortName)
        {
            string cmd = "Adjustn1_0000";
            return PortSend(PortName, 9600, cmd);
        }
        public string SFR_Led_On(string PortName)
        {
            string cmd = "Adjustn1_0100";
            return PortSend(PortName, 9600, cmd);
        }
        public string MCC_Led_Off(string PortName)
        {
            string cmd = "Adjustn3_0000";
            return PortSend(PortName, 9600, cmd);
        }
        public string MCC_Led_On(string PortName)
        {
            string cmd = "Adjustn3_0100";
            return PortSend(PortName, 9600, cmd);
        }
        public string Shading_Led_Off(string PortName)
        {
            string cmd = "Adjustn4_0000";
            return PortSend(PortName, 9600, cmd);
        }
        public string Shading_Led_On(string PortName)
        {
            string cmd = "Adjustn4_0100";
            return PortSend(PortName, 9600, cmd);
        }
        #endregion
        #region FinishedCameraTest 
        public string Motoint_Init()
        {
            string path = @"CD C:\VCS001\uart";
            string args = "motoint.exe.bat";
            string CompareValue = "";
            var Value = Send_args(path, args, CompareValue,60);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Photo_Capture_Test()
        {
            string path = @"CD C:\VCS001\clair_capture";

            string args = $"clair_capture_vcs001_V2.exe -d=0 -path={Photograph_Path} -disp=1";
            string CompareValue = "P45D.jpg";
            var Value = Send_args(path, args, CompareValue,300);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Read_SFR(string picture)
        {
            string path = @"CD C:\VCS001\merry_IQ_test";
            string args = $@"m_sfr_autoROI.exe {Photograph_Path}\{picture}.jpg";
            string CompareValue = "mtf50_spec";
            var Value = Send_args(path, args, CompareValue,10);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Read_MCC(string picture)
        {
            string path = @"CD C:\VCS001\merry_IQ_test";
            string args = $@"m_MCC_check.exe {Photograph_Path}\{picture}.jpg";
            string CompareValue = "PASS";
            var Value = Send_args(path, args, CompareValue,10);
            if (Value.Contains("False")) return Value;
            return GetThreadStrResult("Delta", 0);
            //return true.ToString();
        }
        public string Read_LSC(string picture)
        {
            string path = @"CD C:\VCS001\merry_IQ_test";
            string args = $@"m_shading.exe {Photograph_Path}\{picture}.jpg";
            string CompareValue = "shading pass";
            var Value = Send_args(path, args, CompareValue,10);
            if (Value.Contains("False")) return Value;
            return GetLSCResult("LR", 1,2);
        }
        public string Stitching_MaxSlope_and_ArucoDetec()
        {
            string path = @"CD C:\VCS001\merry_IQ_test";
            string args = $"m_stitching_aruco_det.exe \"{Photograph_Path}/\"";
            string CompareValue = "Merry stitching test2 Pass.";
            var Value = Send_args(path, args, CompareValue, 10);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Read_Stitching_Defect_Test()
        {
            string path = @"CD C:\VCS001\merry_IQ_test";
            string args = $"m_stitching_DL_3line.exe \"{Photograph_Path}/\"";
            string CompareValue = "stitching test pass";
            var Value = Send_args(path, args, CompareValue, 10);
            if (Value.Contains("False")) return Value;
            return GetThreadStrResult("distance_loss_max", 0).Split('%')[0];
        }
        public string GetThreadStrResult(string Confirm_Value, int position_number)
        {
            var imageValue = ThreadStr.Split('\n');
            try
            {
                for (int i = 0; i < imageValue.Length; i++)
                {
                    if (imageValue[i].Contains(Confirm_Value))
                    {
                        var result = imageValue[i + position_number].Split('=')[1].Replace("PASS", "").Trim();
                        return result;
                    }
                }
                MessageBox.Show($"Can not Find ConfirmValue: {Confirm_Value}. Respond Value = {ThreadStr}");
            }
            catch (IOException e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return false.ToString();
        }
        public string GetLSCResult(string Confirm_Value, int line_number, int position_split)
        {
            var imageValue = ThreadStr.Split('\n');
            try
            {
                for (int i = 0; i < imageValue.Length; i++)
                {
                    if (imageValue[i].Contains(Confirm_Value))
                    {
                        var result = imageValue[i + line_number].Trim().Split(' ')[position_split].Trim();
                        return result;
                    }
                }
                MessageBox.Show($"Can not Find ConfirmValue: {Confirm_Value}. Respond Value = {ThreadStr}");
            }
            catch (IOException e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return false.ToString();
        }
        public string Backup_Image()
        {
            try
            {
                var ImageBackup_path = $@"D:\VCS001-Log\{DateTime.Now.ToString("yyyy-MM-dd")}\{Config["SN"]}_{DateTime.Now.ToString("hh-yy-mm")}";
                if (!Directory.Exists(ImageBackup_path)) Directory.CreateDirectory(ImageBackup_path);
                string[] files = Directory.GetFiles(Photograph_Path);
                foreach(var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string targetPath = Path.Combine(ImageBackup_path, fileName);
                    File.Copy(file, targetPath, true);
                }
                Console.WriteLine($"Backup Image To {Photograph_Path} OK");
                return true.ToString();
            }
            catch (IOException e)
            {
                //Console.WriteLine("Error: " + e.Message);
                MessageBox.Show("Error: " + e.Message);
            }
            return false.ToString();
        }
        #endregion
    }
}
