using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MerryTest.testitem
{
    /// <summary>
    /// 指令帮助类
    /// </summary>
    public class Command
    {
        /// <summary>
        /// 存储回传值
        /// </summary>
        public string ReturnValue;



        #region 引入方法 参数
        private BaseConversion bc = new BaseConversion();
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);

        [DllImport("hid.dll", SetLastError = true)]
        static extern Boolean HidD_GetFeature(IntPtr HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength);
        [DllImport("hid.dll", SetLastError = true)]
        static extern Boolean HidD_SetFeature(IntPtr HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength);
        [DllImport("hid.dll", SetLastError = true)]
        internal static extern bool HidD_SetOutputReport(IntPtr hDev, //设备句柄，即CreateFile的返回值
                                                       byte[] reportBuf,//存有待发送数据的buffer
                                                       int OUT_REPORT_LEN); //buffer的长度
        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(
                                        IntPtr hFile,
                                        byte[] lpBuffer,
                                        uint nNumberOfBytesToWrite,
                                        ref uint lpNumberOfBytesWritten,
                                        IntPtr lpOverlapped
                                        );

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetInputReport(
                                               IntPtr HidDeviceObject,
                                               byte[] lpReportBuffer,
                                               int ReportBufferLength);


        #endregion
        /// <summary>
        /// 使用write下下指令
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="lenght">指令长度</param>
        /// <param name="intPtr">句柄</param>
        /// <returns>指令是否下成功</returns>
        public bool WriteSend(string command, int lenght, IntPtr intPtr)
        {
            if (intPtr == IntPtr.Zero) return false;

            Thread.Sleep(50);
            var commandarr = bc.GetByteArray(command, lenght);
            uint numberofbyteWriten = 0;
            try
            {
                return WriteFile(intPtr, commandarr, (uint)lenght, ref numberofbyteWriten, IntPtr.Zero);
            }
            catch (IOException)
            {
                return false;
            }
        }
        /// <summary>
        /// 使用getreport下下指令對外方法 
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="lenght">指令长度</param>
        /// <param name="intPtr">通道</param>
        /// <returns>指令是否下成功</returns>
        public bool GetReportSend(string command, int lenght, IntPtr intPtr)
        {
            if (intPtr == IntPtr.Zero) return false;

            Thread.Sleep(20);
            var commandarr = bc.GetByteArray(command, lenght);
            try
            {
                return HidD_GetInputReport(intPtr, commandarr, lenght);
            }
            catch (IOException)
            {
                return false;
            }
        }
        /// <summary>
        /// 使用setReport下下指令對外方法 
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="lenght">指令长度</param>
        /// <param name="intPtr">通道</param>
        /// <returns></returns>
        public bool SetReportSend(string command, int lenght, IntPtr intPtr)
        {
            if (intPtr == IntPtr.Zero) return false;

            Thread.Sleep(20);
            var commandarr = bc.GetByteArray(command, lenght);
            try
            {
                return HidD_SetOutputReport(intPtr, commandarr, lenght);
            }
            catch (IOException)
            {
                return false;
            }


        }
        /// <summary>
        /// 使用SetFeature下下指令對外方法 
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="lenght">指令长度</param>
        /// <param name="intPtr">通道</param>
        /// <returns>指令是否下成功</returns>
        public bool SetFeatureSend(string command, int lenght, IntPtr intPtr)
        {
            if (intPtr == IntPtr.Zero) return false;
            Thread.Sleep(20);
            var commandarr = bc.GetByteArray(command, lenght);
            try
            {       //       通道     指令                指令長度               
                return HidD_SetFeature(intPtr, commandarr, lenght);
            }
            catch (IOException)
            {
                return false;
            }


        }

        /// <summary>
        /// 使用write下下指令并且存储回传值到ReturnValue
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="lenght">指令长度</param>
        /// <param name="readdata">指令返回值标识值</param>
        /// <param name="Indexes">指令返回值索引</param>
        /// <param name="handle">句柄通道地址</param>
        /// <param name="intPtr">句柄通道指针</param>
        /// <returns>指令是否下成功</returns>
        public bool WriteReturn(string command, int lenght, string readdata, string Indexes, string handle, IntPtr intPtr)
        {
            ReturnValue = "False";
            if (handle == "" || intPtr == IntPtr.Zero) return false;
            Thread.Sleep(50);
            var _createFileHandle = CreateFile(handle,          //文件位置
                           0x40000000 | 0x80000000,          //允许对设备进行读写访问
                           0x1 | 0x2,    //允许对设备进行共享访问
                           IntPtr.Zero,                           //指向空指针（SECURITY_ATTRIBUTES定义文件的安全特性）
                           3,                         //文件必须已存在
                           0x40000000,                  //允许对文件进行重叠操作
                           IntPtr.Zero);                          //指向空指针（如果不为零，则指定一个文件句柄。新文件将从这个文件中复制扩展属性）
            var readflag = true;
            var values = "";
            var arrInputReport = new byte[lenght];
            var endflag = false;
            FileStream fs = null;
            try
            {
                fs = new FileStream(new SafeFileHandle(_createFileHandle, false), FileAccess.Read | FileAccess.Write, lenght, true);
                if (fs == null)
                {
                    readflag = false;
                    return false;
                }
                #region 监听通道回传值
                AsyncCallback AsyRead = ((IAsyncResult iResult) =>
                {
                    byte[] arrBuff = (byte[])iResult.AsyncState;
                    if (fs != null)
                    {
                        try
                        {
                            fs.EndRead(iResult);
                        }
                        catch
                        {
                            fs.Close();
                        }
                    }
                    else
                    {
                        readflag = false; return;
                    }
                    var arrReaddata = readdata.Split(' ');
                    for (var i = 0; i < arrReaddata.Length; i++)
                    {
                        if (arrBuff[i] != Convert.ToInt32(arrReaddata[i], 16))
                        {
                            readflag = false;
                            return;
                        }
                    }

                    var arrIndexes = Indexes.Split(' ');
                    foreach (var a in arrIndexes)
                    {
                        values = values + string.Format("{0:X2}", arrBuff[Convert.ToInt32(a)]) + " ";
                    }
                    var valuesall = "";
                    for (var i = 0; i < lenght; i++)
                    {
                        valuesall = values + string.Format("{0:X2}", arrBuff[Convert.ToInt32(i)]) + " ";
                    }
                    values = values.Substring(0, values.Length - 1);
                    endflag = true;
                });
                #endregion
                fs.BeginRead(arrInputReport, 0, lenght, AsyRead, arrInputReport);
                Thread.Sleep(20);
                var nums = 0;
                while (true)
                {
                    WriteSend(command, lenght, intPtr);
                    if (endflag)
                    {
                        if (readflag)
                        {
                            ReturnValue = values.Trim();

                            return true;
                        }
                        return false;
                    }
                    Thread.Sleep(100);
                    nums++;
                    if (nums > 40)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }

        }

        /// <summary>
        /// 使用setReport下下指令并且存储回传值到ReturnValue
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="lenght">指令长度</param>
        /// <param name="readdata">指令返回值标识值</param>
        /// <param name="Indexes">指令返回值索引</param>
        /// <param name="handle">句柄通道地址</param>
        /// <param name="intPtr">句柄通道指针</param>
        /// <returns></returns>
        public bool SetReportReturn(string command, int lenght, string readdata, string Indexes, string handle, IntPtr intPtr)
        {
            ReturnValue = "False";
            if (handle == "" || intPtr == IntPtr.Zero) return false;
            var _createFileHandle = CreateFile(handle,          //文件位置
                           0x40000000 | 0x80000000,          //允许对设备进行读写访问
                           0x1 | 0x2,    //允许对设备进行共享访问
                           IntPtr.Zero,                           //指向空指针（SECURITY_ATTRIBUTES定义文件的安全特性）
                           3,                         //文件必须已存在
                           0x40000000,                  //允许对文件进行重叠操作
                           IntPtr.Zero);                          //指向空指针（如果不为零，则指定一个文件句柄。新文件将从这个文件中复制扩展属性）
            var readflag = true;
            var values = "";
            var arrInputReport = new byte[lenght];
            var endflag = false;
            FileStream fs = null;
            try
            {
                fs = new FileStream(new SafeFileHandle(_createFileHandle, false), FileAccess.Read | FileAccess.Write, lenght, true);
                if (fs == null)
                {
                    readflag = false;
                    return false;
                }
                #region 监听通道回传值
                AsyncCallback AsyRead = ((IAsyncResult iResult) =>
                {
                    try
                    {
                        byte[] arrBuff = (byte[])iResult.AsyncState;
                        if (fs != null)
                        {
                            try
                            {
                                fs.EndRead(iResult);
                            }
                            catch
                            {
                                fs.Close();
                            }
                        }
                        else
                        {
                            readflag = false; return;
                        }
                        var arrReaddata = readdata.Split(' ');
                        for (var i = 0; i < arrReaddata.Length; i++)
                        {
                            if (arrBuff[i] != Convert.ToInt32(arrReaddata[i], 16))
                            {
                                readflag = false;
                                return;
                            }
                        }

                        var arrIndexes = Indexes.Split(' ');
                        foreach (var a in arrIndexes)
                        {
                            values = values + string.Format("{0:X2}", arrBuff[Convert.ToInt32(a)]) + " ";
                        }
                        var valuesall = "";
                        for (var i = 0; i < lenght; i++)
                        {
                            valuesall = values + string.Format("{0:X2}", arrBuff[Convert.ToInt32(i)]) + " ";
                        }
                        values = values.Substring(0, values.Length - 1);
                        endflag = true;
                    }
                    catch (IOException)
                    {

                        return;
                    }

                });
                #endregion
                fs.BeginRead(arrInputReport, 0, lenght, AsyRead, arrInputReport);
                Thread.Sleep(20);
                var nums = 0;
                while (true)
                {
                    SetReportSend(command, lenght, intPtr);
                    if (endflag)
                    {
                        if (readflag)
                        {
                            ReturnValue = values.Trim();

                            return true;
                        }
                        return false;
                    }
                    Thread.Sleep(100);
                    nums++;
                    if (nums > 40)
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                fs?.Close();
                fs?.Dispose();

            }
        }

        /// <summary>
        /// 使用GetFeature下下指令對外方法并且存储回传值到ReturnValue
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="lenght">指令長度</param>
        /// <param name="intPtr">通道</param>
        /// <param name="indexes">返回下標值</param>
        /// <returns>返回读取的值</returns>
        public bool GetFeatureReturn(string command, int lenght, IntPtr intPtr, string indexes)
        {
            ReturnValue = "False";
            if (intPtr == IntPtr.Zero) return false;
            Thread.Sleep(20);
            var arr = bc.GetintArray(indexes);
            Thread.Sleep(20);
            var commandarr = bc.GetByteArray(command, lenght);
            try
            {
                if (HidD_GetFeature(intPtr, commandarr, lenght))
                {
                    string ver = "";
                    foreach (int a in arr)
                    {
                        ver = ver + string.Format("{0:X2}", commandarr[a]) + " ";
                    }
                    var values = "";
                    for (var i = 0; i < lenght; i++)
                    {
                        values = values + string.Format("{0:X2}", commandarr[i]) + " ";
                    }
                    ReturnValue = ver.Trim();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 使用getReport下指令并且存储回传值到 ReturnValue
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="lenght">指令長度</param>
        /// <param name="intPtr">通道</param>
        /// <param name="indexes">返回下標值</param>
        /// <returns></returns>
        public bool GetReportReturn(string command, int lenght, IntPtr intPtr, string indexes)
        {
            ReturnValue = "False";
            if (intPtr == IntPtr.Zero) return false;
            Thread.Sleep(20);
            var arr = bc.GetintArray(indexes);
            Thread.Sleep(20);
            var commandarr = bc.GetByteArray(command, lenght);
            try
            {
                if (HidD_GetInputReport(intPtr, commandarr, lenght))
                {
                    string ver = "";
                    foreach (var a in arr)
                    {
                        ver = ver + string.Format("{0:X2}", commandarr[a]) + " ";
                    }
                    var values = "";
                    for (var i = 0; i < lenght; i++)
                    {
                        values = values + string.Format("{0:X2}", commandarr[i]) + " ";
                    }
                    ReturnValue = ver.Trim();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 直接获取指定句柄回传值
        /// </summary>
        /// <param name="handle">句柄地址</param>
        /// <param name="length">句柄流数组长度</param>
        /// <returns></returns>
        public string IsReturnValue(string handle, string index, int length = 100, int Timi = 300)
        {
            if (handle.Length < 1)
                return "False";

            IntPtr _createFileHandle = CreateFile(handle,          //文件位置
                            0x40000000 | 0x80000000,          //允许对设备进行读写访问
                            0x1 | 0x2,    //允许对设备进行共享访问
                            IntPtr.Zero,                           //指向空指针（SECURITY_ATTRIBUTES定义文件的安全特性）
                            3,                         //文件必须已存在
                            0x40000000,                  //允许对文件进行重叠操作
                            IntPtr.Zero);                          //指向空指针（如果不为零，则指定一个文件句柄。新文件将从这个文件中复制扩展属性）

            FileStream fsDeviceRead = null;
            string result = "False";
            try
            {
                fsDeviceRead = new FileStream(new SafeFileHandle(_createFileHandle, false), FileAccess.Read | FileAccess.Write, 36, true);
                if (fsDeviceRead != null)
                {
                    AsyncCallback AsyRead = (IAsyncResult iResult) =>
                    {
                        try
                        {
                            byte[] arrBuff = (byte[])iResult.AsyncState;
                            fsDeviceRead.EndRead(iResult);
                            result = bc.GetString(arrBuff, index);
                        }
                        catch
                        {
                            return;
                        }
                    };
                    byte[] arrInputReport = new byte[length];
                    fsDeviceRead.BeginRead(arrInputReport, 0, length, AsyRead, arrInputReport);
                    //等待句柄获取回传值
                    for (var i = 0; i < Timi; i++)
                    {
                        if (result != "False") break;
                        if (result == "Null") return result;
                        Thread.Sleep(50);
                    }
                }
            }

            catch
            {
            }
            finally
            {
                fsDeviceRead?.Close();
                fsDeviceRead?.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 读取通道获取返回值
        /// </summary>
        /// <param name="lenght"></param>
        /// <param name="readdata"></param>
        /// <param name="path"></param>
        /// <param name="timeOut"></param>
        /// <param name="outIndix"></param>
        /// <param name="ResultStr"></param>
        /// <param name="readDatas"></param>
        /// <returns></returns>
        public bool ReadPeerData(int lenght, string readdata, string path, int timeOut, string outIndix, out string ResultStr, out string[] readDatas)
        {

            readDatas = null;
            string str = ResultStr = "Read Data False";
            int readflag = 0;
            List<string> listDatas = new List<string>();
            List<byte> peerData = new List<byte>();
            byte[] arrInputReport = new byte[lenght];
            foreach (var item in readdata.Split(' '))
                peerData.Add(byte.Parse(item, System.Globalization.NumberStyles.HexNumber));
            IntPtr _createFileHandle = CreateFile(path,          //文件位置
                      0x40000000 | 0x80000000,          //允许对设备进行读写访问
                      0x1 | 0x2,    //允许对设备进行共享访问
                      IntPtr.Zero,                           //指向空指针（SECURITY_ATTRIBUTES定义文件的安全特性）
                      3,                         //文件必须已存在
                      0x40000000,                  //允许对文件进行重叠操作
                      IntPtr.Zero);  //指向空指针（如果不为零，则指定一个文件句柄。新文件将从这个文件中复制扩展属性）
            FileStream fs = null;
            try
            {
                fs = new FileStream(new SafeFileHandle(_createFileHandle, false), FileAccess.Read | FileAccess.Write, lenght, true);
                if (fs == null)
                    return false;
                AsyncCallback AsyRead = null;
                #region 异步读取句柄返回值
                AsyRead = ((IAsyncResult iResult) =>
                {
                    try
                    {
                        byte[] arrBuff = (byte[])iResult.AsyncState;
                        fs.EndRead(iResult);
                        string readData = "";
                        str = "";
                        foreach (var item in arrBuff)
                            readData += $"{item:X2} ";
                        listDatas.Add(readData);
                        foreach (var item in outIndix.Split(' '))
                            str += $"{arrBuff[int.Parse(item)]:X2} ";
                        str += "False";
                        for (var i = 0; i < peerData.Count; i++)
                            if (arrBuff[i] != peerData[i])
                            {
                                fs.BeginRead(arrInputReport, 0, lenght, AsyRead, arrInputReport);
                                return;
                            }
                        readflag = 1;
                    }
                    catch (Exception ex)
                    {
                        readflag = -1;
                        return;

                    }
                });
                #endregion
                fs.BeginRead(arrInputReport, 0, lenght, AsyRead, arrInputReport);
                Thread.Sleep(20);

                #region 监听委托是否完成
                for (int i = 0; i < timeOut; i++)
                {
                    Thread.Sleep(1000);

                    if (readflag == 1)
                    {
                        str = str.Replace(" False", "");
                        return true;
                    }
                    else if (readflag == -1)
                    {
                        break;
                    }
                }
                return false;
                #endregion
            }
            catch
            {
                return false;
            }
            finally
            {

                fs?.Close();
                fs?.Dispose();
                ResultStr = str;
                readDatas = listDatas.ToArray();
            }

        }


    }
}
