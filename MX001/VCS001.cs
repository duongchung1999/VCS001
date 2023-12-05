using MerryTest.testitem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCS001;

namespace MerryDllFramework
{
    public class MerryDll : IMerryDll
    {
        public string[] GetDllInfo()
        {
            string dllname = "DLL 名称       ：VCS001";
            string dllfunction = "Dll功能说明 ：VCS001";
            string dllHistoryVersion = "历史Dll版本：0.0.0.1";
            string dllVersion = "当前Dll版本：MP 23.12.04.0";
            string dllChangeInfo = "Dll改动信息：";
            string dllChangeInfo3 = "MP 23.11.10.0： 2023/11/10 第一版开发程序";
            string[] info = { dllname,
                dllfunction,
                dllHistoryVersion,
                dllVersion,
                dllChangeInfo, dllChangeInfo3
       };
            return info;
        }
        CommandList CommandList = new CommandList();
        MotorControl motorControl = new MotorControl();
        FinishedCameraTestStation FinishedCameraTest = new FinishedCameraTestStation();
        Data data = new Data();
        /// <summary>
        /// 平台程序共享的参数
        /// </summary>
        public Dictionary<string, object> Config = new Dictionary<string, object>();
        /// <summary>
        /// 连扳程序特有的单线程平台共享参数
        /// </summary>
        Dictionary<string, object> OnceConfig = new Dictionary<string, object>();
        /// <summary>
        /// 用于判断是连扳还是单板程序
        /// </summary>
        bool MoreTestFlag;
        public object Interface(Dictionary<string, object> Config) => this.Config = Config;
        public string Run(string Command)
        {
            string SN = (string)Config["SN"];
            string BD号 = (string)Config["BitAddress"];
            var cmd = Command.Split('-');
            switch (cmd[0])
            {
                //产品FW及功能 Area
                case "Read_AST1220_FW": return CommandList.Read_AST1220_FW();
                case "Read_KL520_FW": return CommandList.Read_KL520_FW();
                case "Read_MCU_GD32F310_FW": return CommandList.Read_MCU_GD32F310_FW((string)Config["VCS001_ComPort"]);
                case "Read_MCU_GD32F310_FW_TEST": return CommandList.Read_MCU_GD32F310_FW("COM16");
                case "Read_MT9050_FW": return CommandList.Read_MT9050_FW();
                //pin mode
                case "ast1220_imageMode_pin": return CommandList.ast1220_imageMode_pin();
                case "ast1220_GPIO1_pin": return CommandList.ast1220_GPIO1_pin();
                //Flag and SN
                case "Write_Test_Flag": return TE_BZP() ? true.ToString() : CommandList.Write_Test_Flag(cmd[1]);
                case "Read_Test_Flag": return CommandList.Read_Test_Flag();
                case "Write_SN": return TE_BZP() ? true.ToString() : CommandList.Write_SN((string)Config["SN"]);
                case "Write_SN_Test": return CommandList.Write_SN("372317BAB00008");
                case "Check_SN": return CommandList.Read_SN((string)Config["SN"]);
                //Calibration
                case "Dowload_Calibration": return TE_BZP() ? true.ToString() : CommandList.Dowload_Calibration((string)Config["UID"]);
                case "Dowload_Calibration_Test": return CommandList.Dowload_Calibration("0807234648");
                case "DowloadDataFile_Test": return CommandList.DowloadDataFile("0807234648").ToString();
                case "UnzipFile_Test": return CommandList.UnzipFile().ToString();
                case "FOV360_image": return CommandList.FOV360_image();
                case "Check_4_Mic": return CommandList.Check_4_Mic();


                //Camera Check Picture Area
                case "Get_Image_Log_Path": return CommandList.Get_Image_Log_Path();
                case "Check_FOV360_Camera_After": return CommandList.OpenCamera("");
                case "CheckVideoDevices": return CommandList.OpenCamera("20000");
                //Cam 0 sfr
                case "PhotoGraph_cam0_sfr_12": return CommandList.Capture("cam0_sfr_1.2.jpg");
                case "Read_SFR12m_Cam0": return CommandList.ProcessingImage("opt_sfr_1.2m.exe", "cam0_sfr_1.2.jpg", "Img_Center");
                case "SFR12m_Cam0_Img_Center": return CommandList.GetImageCalculateResult_sfr("Img_Center", "2");
                case "SFR12m_Cam0_Img_TL": return CommandList.GetImageCalculateResult_sfr("Img_TL", "2");
                case "SFR12m_Cam0_Img_TR": return CommandList.GetImageCalculateResult_sfr("Img_TR", "2");
                case "SFR12m_Cam0_Img_BL": return CommandList.GetImageCalculateResult_sfr("Img_BL", "2");
                case "SFR12m_Cam0_Img_BR": return CommandList.GetImageCalculateResult_sfr("Img_BR", "2");

                //Cam 4 Isc
                case "PhotoGraph_cam4_lsc": return CommandList.Capture("cam4_lsc.jpg");
                case "Blemish_Cam4": return CommandList.ProcessingImage("opt_blemish.exe", "cam4_lsc.jpg", "blemish pass");
                case "Read_Color_Shading_Cam4": return CommandList.ProcessingImage("opt_color_shading.exe", "cam4_lsc.jpg", "color shading pass");
                case "Color_Shading_Cam4": return CommandList.GetImageCalculateResult_Isc("LR", "1", "1");
                case "Read_Shading_Cam4": return CommandList.ProcessingImage("m_shading.exe", "cam4_lsc.jpg", "shading pass");
                case "Shading_Cam4": return CommandList.GetImageCalculateResult_Isc("LR", "1", "2");

                //Cam 3 sfr
                case "PhotoGraph_cam3_sfr_12": return CommandList.Capture("cam3_sfr_1.2.jpg");
                case "Read_SFR12m_Cam3": return CommandList.ProcessingImage("opt_sfr_1.2m.exe", "cam3_sfr_1.2.jpg", "Img_Center");
                case "SFR12m_Cam3_Img_Center": return CommandList.GetImageCalculateResult_sfr("Img_Center", "2");
                case "SFR12m_Cam3_Img_TL": return CommandList.GetImageCalculateResult_sfr("Img_TL", "2");
                case "SFR12m_Cam3_Img_TR": return CommandList.GetImageCalculateResult_sfr("Img_TR", "2");
                case "SFR12m_Cam3_Img_BL": return CommandList.GetImageCalculateResult_sfr("Img_BL", "2");
                case "SFR12m_Cam3_Img_BR": return CommandList.GetImageCalculateResult_sfr("Img_BR", "2");

                //Cam 1 Isc
                case "PhotoGraph_cam1_lsc": return CommandList.Capture("cam1_lsc.jpg");
                case "Blemish_Cam1": return CommandList.ProcessingImage("opt_blemish.exe", "cam1_lsc.jpg", "blemish pass");
                case "Read_Color_Shading_Cam1": return CommandList.ProcessingImage("opt_color_shading.exe", "cam1_lsc.jpg", "color shading pass");
                case "Color_Shading_Cam1": return CommandList.GetImageCalculateResult_Isc("LR", "1", "3");
                case "Read_Shading_Cam1": return CommandList.ProcessingImage("m_shading.exe", "cam1_lsc.jpg", "shading pass");
                case "Shading_Cam1": return CommandList.GetImageCalculateResult_Isc("LR", "1", "2");

                //Cam 0 sfr
                case "PhotoGraph_cam4_sfr_12": return CommandList.Capture("cam4_sfr_1.2.jpg");
                case "Read_SFR12m_Cam4": return CommandList.ProcessingImage("opt_sfr_1.2m.exe", "cam4_sfr_1.2.jpg", "Img_Center");
                case "SFR12m_Cam4_Img_Center": return CommandList.GetImageCalculateResult_sfr("Img_Center", "2");
                case "SFR12m_Cam4_Img_TL": return CommandList.GetImageCalculateResult_sfr("Img_TL", "2");
                case "SFR12m_Cam4_Img_TR": return CommandList.GetImageCalculateResult_sfr("Img_TR", "2");
                case "SFR12m_Cam4_Img_BL": return CommandList.GetImageCalculateResult_sfr("Img_BL", "2");
                case "SFR12m_Cam4_Img_BR": return CommandList.GetImageCalculateResult_sfr("Img_BR", "2");

                //Cam 0 Isc
                case "PhotoGraph_cam0_lsc": return CommandList.Capture("cam0_lsc.jpg");
                case "Blemish_Cam0": return CommandList.ProcessingImage("opt_blemish.exe", "cam0_lsc.jpg", "blemish pass");
                case "Read_Color_Shading_Cam0": return CommandList.ProcessingImage("opt_color_shading.exe", "cam0_lsc.jpg", "color shading pass");
                case "Color_Shading_Cam0": return CommandList.GetImageCalculateResult_Isc("LR", "1", "1");
                case "Read_Shading_Cam0": return CommandList.ProcessingImage("m_shading.exe", "cam0_lsc.jpg", "shading pass");
                case "Shading_Cam0": return CommandList.GetImageCalculateResult_Isc("LR", "1", "2");

                //Cam 1 sfr
                case "PhotoGraph_cam1_sfr_12": return CommandList.Capture("cam1_sfr_1.2.jpg");
                case "Read_SFR12m_Cam1": return CommandList.ProcessingImage("opt_sfr_1.2m.exe", "cam1_sfr_1.2.jpg", "Img_Center");
                case "SFR12m_Cam1_Img_Center": return CommandList.GetImageCalculateResult_sfr("Img_Center", "2");
                case "SFR12m_Cam1_Img_TL": return CommandList.GetImageCalculateResult_sfr("Img_TL", "2");
                case "SFR12m_Cam1_Img_TR": return CommandList.GetImageCalculateResult_sfr("Img_TR", "2");
                case "SFR12m_Cam1_Img_BL": return CommandList.GetImageCalculateResult_sfr("Img_BL", "2");
                case "SFR12m_Cam1_Img_BR": return CommandList.GetImageCalculateResult_sfr("Img_BR", "2");

                //Cam 3 Isc
                case "PhotoGraph_cam3_lsc": return CommandList.Capture("cam3_lsc.jpg");
                case "Blemish_Cam3": return CommandList.ProcessingImage("opt_blemish.exe", "cam3_lsc.jpg", "blemish pass");
                case "Read_Color_Shading_Cam3": return CommandList.ProcessingImage("opt_color_shading.exe", "cam3_lsc.jpg", "color shading pass");
                case "Color_Shading_Cam3": return CommandList.GetImageCalculateResult_Isc("LR", "1", "1");
                case "Read_Shading_Cam3": return CommandList.ProcessingImage("m_shading.exe", "cam3_lsc.jpg", "shading pass");
                case "Shading_Cam3": return CommandList.GetImageCalculateResult_Isc("LR", "1", "2");


                //Constant Image Area
                case "Read_Stitching_Defect_Test": return CommandList.Read_Stitching_Defect_Test();
                case "Stitching_MaxSlope_ArucoDetec": return CommandList.Stitching_MaxSlope_ArucoDetec();
                case "Open_Constant_Photograph": return CommandList.OpenCameraConstant();
                case "CloseCamera": return CommandList.CloseCameraConstant();
                case "PhotoGraph_P135D": return CommandList.ConstantImage("P135D.jpg");
                case "PhotoGraph_P180D": return CommandList.ConstantImage("P180D.jpg");
                case "PhotoGraph_P225D": return CommandList.ConstantImage("P225D.jpg");
                case "PhotoGraph_P270D": return CommandList.ConstantImage("P270D.jpg");
                case "PhotoGraph_P315D": return CommandList.ConstantImage("P315D.jpg");
                case "PhotoGraph_P45D": return CommandList.ConstantImage("P45D.jpg");
                case "PhotoGraph_P90D": return CommandList.ConstantImage("P90D.jpg");



                //Change Camera Mode
                case "Cam0_Setup": return CommandList.Cam0_Setup();
                case "Cam1_Setup": return CommandList.Cam1_Setup();
                case "Cam3_Setup": return CommandList.Cam3_Setup();
                case "Cam4_Setup": return CommandList.Cam4_Setup();
                case "Cam360_Setup": return CommandList.Cam360_Setup();

                //曝光 AE target 調整
                case "AE_9": return CommandList.AE_9();
                case "AE_40": return CommandList.AE_40();
                case "AE_50": return CommandList.AE_50();
                case "AE_70": return CommandList.AE_70();
                //Motor Area
                case "ConnectPort": return motorControl.ConnectPort((string)Config["VCS001_ComPort"]).ToString();
                case "ConnectPort_TEST": return motorControl.ConnectPort("COM18").ToString();
                case "S1_Reset": return motorControl.S1_Reset();
                case "S1_P0D": return motorControl.S1_P0D();
                case "S1_P45D": return motorControl.S1_P45D();
                case "S1_P90D": return motorControl.S1_P90D();
                case "S1_P135D": return motorControl.S1_P135D();
                case "S1_P180D": return motorControl.S1_P180D();
                case "S1_P225D": return motorControl.S1_P225D();
                case "S1_P270D": return motorControl.S1_P270D();
                case "S1_P315D": return motorControl.S1_P315D();

                //Cylinder Area
                case "S2_P0_On": return motorControl.S2_P0_On();
                case "S2_P0_Off": return motorControl.S2_P0_Off();
                case "S2_P1_On": return motorControl.S2_P1_On();
                case "S2_P1_Off": return motorControl.S2_P1_Off();


                //Backlight Control Area
                case "Stitching_Led_On": return motorControl.Stitching_Led_On();
                case "Stitching_Led_Off": return motorControl.Stitching_Led_Off();
                case "SFR_12_Led_On": return motorControl.SFR_12_Led_On();
                case "SFR_12_Led_Off": return motorControl.SFR_12_Led_Of();
                case "SFR_07_Led_On": return motorControl.SFR_07_Led_On();
                case "SFR_07_Led_Off": return motorControl.SFR_07_Led_Of();
                case "Shading_Led_On": return motorControl.Shading_Led_On();
                case "Shading_Led_Off": return motorControl.Shading_Led_Off();

                    //  

                #region SMT
                case "Burn_FW": return TE_BZP() ? true.ToString() : CommandList.Burn_FW();
                case "Read_MT9050_FW_SMT": return CommandList.Read_MT9050_FW_SMT((string)Config["VCS001_ComPort"]);
                case "Read_MT9050_FW_SMT_TEST": return CommandList.Read_MT9050_FW_SMT("COM17");
                case "LoopBackTest_SMT": return CommandList.LoopBackTest_SMT((string)Config["VCS001_ComPort"]);
                case "LoopBackTest_SMT_TEST": return CommandList.LoopBackTest_SMT("COM17");
                case "AudioBoard_WriteFlag_TEST": return CommandList.AudioBoard_WriteFlag_SMT("COM17", cmd[1]);
                case "AudioBoard_WriteFlag_SMT": return CommandList.AudioBoard_WriteFlag_SMT((string)Config["VCS001_ComPort"], cmd[1]);
                case "AudioBoard_ReadFlag_TEST": return CommandList.AudioBoard_ReadFlag_SMT("COM17");
                case "AudioBoard_ReadFlag_SMT": return CommandList.AudioBoard_ReadFlag_SMT((string)Config["VCS001_ComPort"]);
                case "StartReadRespond_Test": return CommandList.StartReadRespond("COM16").ToString();
                case "StartReadRespond": return CommandList.StartReadRespond((string)Config["VCS001_ComPort"]).ToString();
                case "EndRespond": return CommandList.EndRespond();
                case "TP4162_Power_KEY": return CommandList.PowerButtonTest();
                case "TP4163_MUTE_KEY": return CommandList.MuteButtonTest();
                case "TP4165_VOL_DOWN_KEY": return CommandList.VolumeDownTest();
                case "TP4164_VOL_UP_KEY": return CommandList.VolumeUpTest();
                case "TP4165_Image_Mode_KEY": return CommandList.ImageButtonTest();
                #endregion


                case "GetDllFirmwareVersion": return "MP 22.3.9.1";
                default: return "Command Error False";
            }
        }

        public bool Start(List<string> formsData, IntPtr _handel)
        {
            /*
                程序启动是触发方法
                写下你的代码 
                Console.WriteLine("Hello Word");
             */
            //料号
            string OrderNumberInformation = (string)Config["OrderNumberInformation"];
            //工单
            string OrderNumber = (string)Config["Works"];
            //根据料号索引的后台的参数
            Dictionary<string, string> PartNumberInfos = (Dictionary<string, string>)Config["PartNumberInfos"];
            CommandList.Config = Config;
            FinishedCameraTest.Config = Config;

            return true;
        }

        public bool StartRun()
        {
            /*
               单板开始测试是触发方法
               写下你的代码 
            */
            MoreTestFlag = false;

            return true;
        }

        public bool StartTest(Dictionary<string, object> OnceConfig)
        {
            /*
               连扳程序当开始测试后触发方法  OnceConfig是线程独立参数
               写下你的代码 
               Console.WriteLine("Hello Word");
            */
            this.OnceConfig = OnceConfig;
            MoreTestFlag = true;

            return true;
        }
        public void TestsEnd(object obj)
        {
            /*
                连扳程序当所有线程测试结束后触发方法
                写下你的代码 
                Console.WriteLine("Hello Word");
           */
        }
        private bool TE_BZP()
        {
            string sn = Config["SN"].ToString();
            if (sn.Contains("TE_BZP")) return true;
            return false;
        }
      
    }
}
