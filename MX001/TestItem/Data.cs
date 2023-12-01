using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MerryTest.testitem
{
    /// <summary>
    /// 存储信息(需要批量更改通道或者地址或者指令长度时，直接改变此类中的值)
    /// </summary>
    internal class Data
    {
        /// <summary>
        /// 赋值给属性
        /// </summary>
        internal Data()
        {
            DonglePID = GetValue("Config", "DonglePID");
            DongleVID = GetValue("Config", "DongleVID");
            HeadsetPID = GetValue("Config", "HeadsetPID");
            HeadsetVID = GetValue("Config", "HeadsetVID");
        }
        public string DonglePID;
        public string DongleVID;
        public string HeadsetPID;
        public string HeadsetVID;
        public string dllpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// 存储主程序传送参数
        /// 【0】SN
        /// 【1】LincenseKey
        /// 【2】BD
        /// 【3】SN_BCCode
        /// </summary>
        internal List<string> formsData = new List<string>();
        [DllImport("kernel32.dll")]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refVar, int size, string INIpath);
        /// <summary>
        /// 获取ini参数
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string section, string key)
        {

            StringBuilder var = new StringBuilder(512);
            int result = GetPrivateProfileString(section, key, "", var, 512, $@"{dllpath}\ConfigType.ini");
            if (result == 0) SetValue(section, key, "");
            return var.ToString();
        }
        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(string section, string key, string val, string filePath);
        /// <summary>
        /// 设置ini参数
        /// </summary>
        /// <param name="section"></param>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int SetValue(string section, string Key, string value)
        {
            return WritePrivateProfileString(section, Key, value, $@"{dllpath}\ConfigType.ini");
        }
    }
}
