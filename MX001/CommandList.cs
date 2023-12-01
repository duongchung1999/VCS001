using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MerryTest.testitem;
using MerryDllFramework;
using System.IO;
using System.IO.Compression;

namespace VCS001
{
    public class CommandList
    {
        private string AstUsbTool_Path = @"CD C:\VCS001\AST_USB_Tool";
        private string BurnPath = @"CD C:\VCS001\VCS001_MCU_GD32_ISP_V2";
        private string _4MIC_path = @"CD C:\VCS001\VCS001_2MIC_path_test";
        private string Camera_path = @"CD C:\VCS001\OpenCameraTest";
        private string ComputerZipFile = @"C:\VCS001\CalibrationDowload\CalibrationData.zip";
        private string Calibration_Path = @"\\10.175.5.25\data";
        private string Image_Check_Path = @"C:\VCS001\POT_image_test\test";
        private string POT_Image_path = @"CD C:\VCS001\POT_image_test";
        #region 补助指令
        private Process p;
        private string ThreadStr;
        private SerialPort port1;
        MerryDll MEVN;
        
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

        #region PCBA area
        public string Burn_FW()
        {
            string path = BurnPath;
            string args = "GD32_ISP_CLI.exe -c --pn {0} --br 57600 --db 8 --pr EVEN --sb 1 --to 5000 -p --dwp -e --all -d --a 8000000 --fn {1} --v";
            string CompareValue = "Successful";
            var Value = Send_args(path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            MessageBox.Show(Value);
            //var FW = Value.Split('"')[2].Split(' ')[0].Replace("total", "");
            return true.ToString();
        }
        public string Read_MCU_GD32F310_FW(string Com_name)
        {
            //通过 UART 下 “ver”
            try
            {
                if (OpenPort(Com_name, 115200) != true.ToString()) return false.ToString();
                port1.WriteLine("ver");
                Thread.Sleep(1000);
                var respond = port1.ReadExisting();
                if (!respond.Contains("VCS001_MCU")) return $"False {respond}";
                var value = respond.Split(' ');
                foreach (var fw in value)
                {
                    if (!fw.Contains("VCS001")) continue;
                    return fw.Split('_', '\r', '\n')[2];
                }
                return false.ToString() + respond;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
                return $"False {ex}";

            }
            finally
            {
                if (port1.IsOpen) port1.Close();
                port1.Dispose();
                Thread.Sleep(50);
            }

        }
        public string respond = "";
        public bool StartReadRespond(string Com_name)
        {
            try
            {
                respond = "";
                if (OpenPort(Com_name, 115200) != true.ToString()) return false;
                port1.DataReceived += port1_DataReceived;
                Console.WriteLine("Press any key to stop reading...");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }

            //Thread.Sleep(2000);
            //if (respond.Contains(CompareValue)) return true;
            //return false;
        }
        public string EndRespond()
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
        private void port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            string data = serialPort.ReadExisting();
            var value = data.Split('\r', '\n');
            //foreach (var value1 in value)
            //{
            //    if (value1 == "") continue;
            //    respond += value1;
            //}
            respond += data;

            Console.WriteLine("Received data: " + data);
        }
        private string SMT_ButtonTest_Compare(string CompareValue)
        {
            int i = 0;
            bool flag = false;
            while (i < 20)
            {
                i++;
                Thread.Sleep(1000);
                if (!respond.Contains(CompareValue)) continue;
                Console.WriteLine($"TimeOut= {i * 1000}");
                flag = true;
                //EndRespond();
                break;

            }
            if (!flag) MessageBox.Show(respond);
            respond = "";
            return flag.ToString();
        }
        public string PowerButtonTest()
        {
            var CompareValue = "Countdown 179";
            return SMT_ButtonTest_Compare(CompareValue);
        }
        public string VolumeDownTest()
        {
            var CompareValue = "vol down key press";
            return SMT_ButtonTest_Compare(CompareValue);
        }
        public string VolumeUpTest()
        {
            var CompareValue = "vol up key press";
            return SMT_ButtonTest_Compare(CompareValue);
        }
        public string MuteButtonTest()
        {
            var CompareValue = "mute key press";
            return SMT_ButtonTest_Compare(CompareValue);
        }
        public string ImageButtonTest()
        {
            var CompareValue = "image mode key press";
            return SMT_ButtonTest_Compare(CompareValue);
        }
        public string Read_MT9050_FW_SMT(string Com_name)
        {
            try
            {
                if (OpenPort(Com_name, 115200) != true.ToString()) return false.ToString();
                port1.WriteLine("root");
                Thread.Sleep(1000);
                port1.WriteLine("cat /data/mtk_fw_info");
                Thread.Sleep(1000);
                var respond = port1.ReadExisting();
                var value = respond.Split('\r', '\n');
                foreach (var fw in value)
                {
                    if (fw.Contains("P1.")) return fw;
                }
                MessageBox.Show(respond);
                return false.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
                return $"False {ex}";

            }
            finally
            {
                if (port1.IsOpen) port1.Close();
                port1.Dispose();
                Thread.Sleep(50);
            }

        }
        public string LoopBackTest_SMT(string Com_name)
        {
            try
            {

                var cmd1 = "killall mtk_ec_server";
                var cmd2 = "killall mtk_alsa_route";
                var cmd3 = "arecord -f S16_LE -r 48000 -c 4 -D hw:0,8 --period-size=256 | aplay -D default -f S16_LE -r 48000 -c 4 --buffer-size=1024&";
                var confirm1 = "success";
                var confirm2 = "shutdown";
                var confirm3 = "mode";
                if (OpenPort(Com_name, 115200) != true.ToString()) return false.ToString();
                port1.WriteLine("root");
                Thread.Sleep(1000);
                port1.WriteLine(cmd1);
                Thread.Sleep(1000);
                var value = port1.ReadExisting();
                if (!value.Contains(confirm1))
                {
                    MessageBox.Show($"cmd1: {cmd1}\nrespond does not contains \"{confirm1}\":\n{value}");
                    return $"False";
                }
                port1.WriteLine(cmd2);
                Thread.Sleep(1000);
                value = port1.ReadExisting();
                if (!value.Contains(confirm2))
                {
                    MessageBox.Show($"cmd2: {cmd2}\nrespond does not contains \"{confirm2}\":\n{value}");
                    return $"False";
                }
                Thread.Sleep(4000);
                port1.WriteLine(cmd3);
                Thread.Sleep(1000);
                value = port1.ReadExisting();
                if (!value.Contains(confirm3))
                {
                    MessageBox.Show($"cmd3: {cmd3}\nrespond does not contains \"{confirm3}\":\n{value}");
                    return $"False";
                }
                return true.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
                return $"False {ex}";

            }
            finally
            {
                if (port1.IsOpen) port1.Close();
                port1.Dispose();
                Thread.Sleep(50);
            }

        }

        public string AudioBoard_WriteFlag_SMT(string Com_name, string flag)
        {
            //通过 UART 下 “ver”
            try
            {
                if (OpenPort(Com_name, 115200) != true.ToString()) return false.ToString();
                //port1.WriteLine("root");
                //Thread.Sleep(1000);
                port1.WriteLine($"echo {flag} > /data/FT_T1.2");//echo 1 > /data/FT_T1.2
                Thread.Sleep(1000);
                var fw = port1.ReadExisting();
                if (!fw.Contains("#")) return $"False {fw}";
                return true.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
                return $"False {ex}";

            }
            finally
            {
                if (port1.IsOpen) port1.Close();
                port1.Dispose();
                Thread.Sleep(50);
            }
        }
        public string AudioBoard_ReadFlag_SMT(string Com_name)
        {
            //通过 UART 下 “ver”
            try
            {
                if (OpenPort(Com_name, 115200) != true.ToString()) return false.ToString();
                //port1.WriteLine("root");
                //Thread.Sleep(1000);
                port1.WriteLine($"cat /data/FT_T1.2");
                Thread.Sleep(1000);
                var respond = port1.ReadExisting();
                if (!respond.Contains("#")) return $"False {respond}";
                var fw = respond.Split('\r', '\n');
                return fw[2];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
                return $"False {ex}";

            }
            finally
            {
                if (port1.IsOpen) port1.Close();
                port1.Dispose();
                Thread.Sleep(50);
            }
        }
        #endregion

        #region 成品Area
        #region FW Area
        public string Read_AST1220_FW()
        {
            string path = AstUsbTool_Path;
            string args = "ast_usb_ctrl.exe s g \"ast_param g version:merry_firmware:release_version\"";
            string CompareValue = "P1";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            var FW = Value.Split('"')[2].Split(' ')[0].Replace("total", "");
            return FW;
        }
        public string Read_KL520_FW()
        {
            //string path = AstUsbTool_Path;
            string args = "ast_usb_ctrl.exe s g \"ast_cli -u 4 | grep version\"";
            string CompareValue = "NV12";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            var FW = Value.Split('=')[1].Split(' ')[1].Replace("total", "");
            return FW;

        }
        public string Read_MT9050_FW()
        {
            //string path = AstUsbTool_Path;
            string args = "ast_usb_ctrl.exe s g \"cat /tmp/mtk_fw_info\"";
            string CompareValue = "P1";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            var fw = Value.Split('"')[2].Split(' ')[0].Replace("total", "");
            return fw;
        }
        #endregion
        #region 功能Area
        public string FOV360_image()
        {
            string args = "ast_usb_ctrl.exe s g \"ast_tcpclient fov_mode\"";
            string CompareValue = "successfully";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        #endregion
        #region Button Test Area
        public string ast1220_imageMode_pin()//按一下image button AST1220:
        {
            //string path = AstUsbTool_Path;
            string args = "ast_usb_ctrl.exe s g \"cat /sys/class/gpio/gpio8/value\"";
            string CompareValue = " ";
            try
            {
                Func<bool> func = () =>
                {
                    while (true)
                    {
                        var Value = Send_args(AstUsbTool_Path, args, CompareValue);
                        var status = Value.Split('"')[2].Split(' ')[0].Replace("total", "");
                        return status == "1";
                    }
                };
                return ProgressBars.CountDown(func, "Vui lòng nhấn nút Image/请按下 Image按键", "Check Image Button").ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "False" + ex;
            }

        }
        public string ast1220_GPIO1_pin()//再按一下image button AST1220:
        {
            //string path = AstUsbTool_Path;
            string args = "ast_usb_ctrl.exe s g \"cat /sys/class/gpio/gpio5/value\"";
            string CompareValue = " ";
            try
            {
                Func<bool> func = () =>
                {
                    while (true)
                    {
                        var Value = Send_args(AstUsbTool_Path, args, CompareValue);
                        var status = Value.Split('"')[2].Split(' ')[0].Replace("total", "");
                        return status == "1";
                    }
                };
                return ProgressBars.CountDown(func, "Vui lòng nhấn lại nút Image/请再次按下 Image按键", "Check Image Button").ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "False" + ex;
            }
        }
        #endregion
        #region 写入Flag Area
        public string Write_Test_Flag(string flag)
        {
            try
            {
                //string path = AstUsbTool_Path;
                //写入
                int flag_number = int.Parse(flag);
                string args = $"ast_usb_ctrl.exe s g \"ast_param mfs merry:factory_test {flag}\"";
                string CompareValue = " ";
                var Value = Send_args(AstUsbTool_Path, args, CompareValue);
                if (Value.Contains("False")) return Value;
                //保存
                args = "ast_usb_ctrl.exe s g \"ast_param mfsave\"";
                Value = Send_args(AstUsbTool_Path, args, CompareValue);
                if (Value.Contains("False")) return Value;
                return true.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"False 写入Flag = {flag} 异常了\n{ex}");
                return $"False";
            }

        }
        public string Read_Test_Flag()
        {
            string args = "ast_usb_ctrl.exe s g \"ast_param mfg merry:factory_test\"";
            string CompareValue = " ";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            var test_flag = Value.Split('"')[2].Split(' ')[0].Replace("total", "");
            return test_flag;
        }
        #endregion
        #region 写入SN Area
        public string Write_SN(string SN)
        {
            if (SN.Contains("TE_BZP")) return true.ToString();
            string args = $"ast_usb_ctrl.exe s g \"ast_param mfs ipevo:TOTEM360:sn {SN}\"";
            string CompareValue = " ";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            args = $"ast_usb_ctrl.exe s g \"ast_param mfsave\"";
            CompareValue = "records";
            Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Read_SN(string SN)
        {
            string args = "ast_usb_ctrl.exe s g \"ast_param mfg ipevo:TOTEM360:sn\"";
            string CompareValue = " ";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            var sn = Value.Split('"')[2].Split(' ')[0].Replace("total", "");
            if (!sn.Contains(SN)&&!SN.Contains("TE_BZP")) return $"False {sn}";
            return sn;
        }
        #endregion
        #region Camera Calibartion
       
        public string Dowload_Calibration(string CameraSN)
        {
            if (!DowloadDataFile(CameraSN)) return false.ToString();
            if (!UnzipFile()) return false.ToString();
            string args = $@"ast_usb_ctrl.exe s c C:\VCS001\CalibrationDowload";
            string CompareValue = "Updating calibration data";
            //string CompareValue = "invalid calibration data";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public bool DowloadDataFile(string CameraSN)
        {
            string From_Path = $@"{Calibration_Path}\003025-{CameraSN}.zip";
            string To_Path = ComputerZipFile;
            try
            {
                if (File.Exists(To_Path)) File.Delete(To_Path);
                File.Copy(From_Path, To_Path);
                Console.WriteLine("Dowload File OK");
                return true;
            }
            catch (IOException e)
            {
                //Console.WriteLine("Error: " + e.Message);
                MessageBox.Show("Error: " + e.Message);
            }
            return false;
        }

        public bool UnzipFile()
        {
            string zipPath = ComputerZipFile;
            string extractPath = @"C:\VCS001\CalibrationDowload\calibration_database";
            try
            {
                DirectoryInfo directory = new DirectoryInfo(extractPath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
                Console.WriteLine($"Delete {extractPath} OK");
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                Console.WriteLine("Extract File OK");
                File.Delete(zipPath);
                return true;
            }
            catch (IOException e)
            {
                //Console.WriteLine("Error: " + e.Message);
                MessageBox.Show("Error: " + e.Message);
            }
            return false;
        }
        public string Reboot()
        {
            string args = $"ast_usb_ctrl.exe s g \"reboot\"";
            string CompareValue = "Updating calibration data";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            var sn = Value.Split('"')[2].Split(' ')[0].Replace("total", "");
            return sn;
        }
        #endregion

        #region 其他功能Area
      
        public string Check_4_Mic()
        {
            string path = _4MIC_path;
            string args = "VCS001_2MIC_path_test.exe";
            string CompareValue = "PASS";
            var Value = Send_args(path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        #endregion
        #region Change Camera Mode Area
        public string Cam0_Setup()
        {
            string args = $"ast_usb_ctrl.exe s g \"echo \\\"1 0\\\" > /sys/kernel/ast_cam/dewarp_cfg\"";
            string CompareValue = "total cost";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Cam1_Setup()
        {
            string args = $"ast_usb_ctrl.exe s g \"echo \\\"1 1\\\" > /sys/kernel/ast_cam/dewarp_cfg\"";
            string CompareValue = "total cost";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Cam3_Setup()
        {
            string args = $"ast_usb_ctrl.exe s g \"echo \\\"1 2\\\" > /sys/kernel/ast_cam/dewarp_cfg\"";
            string CompareValue = "total cost";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Cam4_Setup()
        {
            string args = $"ast_usb_ctrl.exe s g \"echo \\\"1 3\\\" > /sys/kernel/ast_cam/dewarp_cfg\"";
            string CompareValue = "total cost";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string Cam360_Setup()
        {
            string args = $"ast_usb_ctrl.exe s g \"echo \\\"0 0\\\" > /sys/kernel/ast_cam/dewarp_cfg\"";
            string CompareValue = "total cost";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        #endregion
        #region 曝光 AE target 調整
        public string AE_9()//MCC 
        {
            string args = $"ast_usb_ctrl.exe s g \"mw 902000a8 9\"";
            string CompareValue = "total cost";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            args = $"ast_usb_ctrl.exe s g \"mw 90200020 8\"";
            Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string AE_50()//360 stitching chart
        {
            string args = $"ast_usb_ctrl.exe s g \"mw 902000a8 50\"";
            string CompareValue = "total cost";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            args = $"ast_usb_ctrl.exe s g \"mw 90200020 8\"";
            Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string AE_70()//LSC、SFR 
        {
            string args = $"ast_usb_ctrl.exe s g \"mw 902000a8 70\"";
            string CompareValue = "total cost";
            var Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            args = $"ast_usb_ctrl.exe s g \"mw 90200020 8\"";
            Value = Send_args(AstUsbTool_Path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        #endregion
        #region ImageProcess Area
        public string OpenCamera(string AddArgs)
        {
            string path = Camera_path;
            string args = $"OpenCameraDisplay.exe {AddArgs}";
            string CompareValue = "OK";
            var Value = Send_args(path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }

        public string Capture(string pictureName)
        {
            string path = Camera_path;
            string args = $"OpenCameraDisplay.exe 3000 {Image_Check_Path}\\{pictureName}";
            string CompareValue = "OK";
            var Value = Send_args(path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            string From_Path = $"{Image_Check_Path}\\{pictureName}";
            string folder_path = $@"C:\VCS001\Image-Log\{DateTime.Now.ToString("yyyy-MM-dd hh-mm")}";
            string To_Path = $"{folder_path}\\{pictureName}";
            try
            {
                if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);
                if (File.Exists(To_Path)) File.Delete(To_Path);
                File.Copy(From_Path, To_Path);
                Console.WriteLine("Capture and Save OK");
                return true.ToString();
            }
            catch (IOException e)
            {
                //Console.WriteLine("Error: " + e.Message);
                MessageBox.Show("Error: " + e.Message);
            }
            return false.ToString();
        }
        public string ProcessingImage(string AppName,string pictureName,string CompareValue)
        {
            string args = $@"{AppName} {Image_Check_Path}\{pictureName}";
            //string CompareValue = "Img_Center";
            var Value = Send_args(POT_Image_path, args, CompareValue);
            if (Value.Contains("False")) return Value;
            return true.ToString();
        }
        public string GetImageCalculateResult_sfr(string Confirm_Value, string position_number)
        {
            var imageValue = ThreadStr.Split('\n');
            int num = int.Parse(position_number);
            try
            {
                for (int i = 0; i < imageValue.Length; i++)
                {
                    if (imageValue[i].Contains(Confirm_Value))
                    {
                        var result = imageValue[i + num].Split('=')[1].Replace("PASS", "").Trim();
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
        public string GetImageCalculateResult_Isc(string Confirm_Value, string line_number, string position_split)
        {
            var imageValue = ThreadStr.Split('\n');
            int num = int.Parse(line_number);
            int split_number = int.Parse(position_split);
            try
            {
                for (int i = 0; i < imageValue.Length; i++)
                {
                    if (imageValue[i].Contains(Confirm_Value))
                    {
                        var result = imageValue[i + num].Split(' ')[split_number].Trim();
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
        #endregion
        #endregion



    }
}
