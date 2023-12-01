using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerryTest.testitem
{
    /// <summary>
    /// 进制转换帮助类
    /// </summary>
    public class BaseConversion
    {
        #region 通用
        /// <summary>
        /// 将字符串转int数组
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
        public int[] GetintArray(string ar)
        {
            string[] strArray = ar.Split(' ');
            var intArray = Array.ConvertAll(strArray, s => int.Parse(s));
            return intArray;
        }
        #endregion
        #region 16进制转其他
        /// <summary>
        /// 將16進制字符串轉換為16进制byte數組并且根据数组长度自动补0
        /// </summary>
        /// <param name="shex">要转换的16进制字符串</param>
        /// <param name="lenght">要转换的Byte数组长度</param>
        /// <returns>转换后的Byte数组，自动补0</returns>
        public byte[] GetByteArray(string shex, int lenght)
        {
            string[] ssArray = shex.Split(' ');
            var bytList = new List<byte>();
            int i = 0;
            foreach (var s in ssArray)
            {   //将十六进制的字符串转换成数值  
                bytList.Add(Convert.ToByte(s, 16));
                i++;
            }
            for (int j = i; j < lenght; j++)
            {
                bytList.Add(Convert.ToByte("0"));
            }
            return bytList.ToArray();
        }
        /// <summary>
        /// Byte数组转16进制字符串
        /// </summary>
        /// <param name="bytes">Byte数组</param>
        /// <returns>16进制字符串</returns>
        public string GetString(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2") + " ";
                }
            }
            return returnStr.Trim();
        }
        /// <summary>
        /// Byte数组取指定索引值转16进制字符串 
        /// </summary>
        /// <param name="bytes">Byte数组</param>
        /// <param name="index">Byte数组索引数组</param>
        /// <returns>16进制字符串</returns>
        public string GetString(byte[] bytes, string index)
        {
            string returnStr = "";
            if (bytes != null)
            {
                var arr = index.Split(' ');
                foreach (var item in arr)
                {
                    returnStr += bytes[Convert.ToInt32(item)].ToString("X2") + " ";
                }
            }
            return returnStr.Trim();
        }
        #endregion
    }
}
