using MerryTest.testitem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestItem
{
    public class TestMethod
    {
        /// <summary>
        /// 信息弹窗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool MessageBox(string msg, string title = "MessageBox")
            => MessageBoxs.messagebox(msg, title);

        /// <summary>
        /// 自定义方法按键测试
        /// </summary>
        /// <param name="func"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool ButtonTest(Func<bool> func, string name, string title = "标题")
            => ProgressBars.CountDown(func, name, title);

        /// <summary>
        /// 倒计时测试，自定义方法
        /// </summary>
        /// <param name="func"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool FuncTestDialog(Func<bool> func, string name, string title = "标题")
           => UpdataFirmware.CountDown(func, name, title);


        /// <summary>
        /// 扫码弹窗
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public bool BarCodeBox(string message, int length, out string barcode)
            => MessageBoxs.BarCodeBox(message, length, out barcode);


        /// <summary>
        /// 按键测试针对性读取返回值
        /// </summary>
        /// <param name="readdata"></param>
        /// <param name="indexs"></param>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool ButtonTest(string readdata, string indexs, string name, string handle, int length)
            => new OldButtonTest().Buttontest(readdata, indexs, name, handle, length);


    }
}
