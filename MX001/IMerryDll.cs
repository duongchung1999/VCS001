

using System;
using System.Collections.Generic;

namespace MerryDllFramework
{
    internal interface IMerryDll
    {

        /// <summary>
        /// 记录dll信息
        /// </summary>
        /// <returns></returns>
        string[] GetDllInfo();
        /// <summary>
        /// 程序参数共享接口
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        object Interface(Dictionary<string, object> Config);
        /// <summary>
        /// 程序启动的时候触发
        /// </summary>
        /// <param name="formsData"></param>
        /// <param name="_handel"></param>
        /// <returns></returns>
        bool Start(List<string> formsData, IntPtr _handel);
        /// <summary>
        /// 程序每次开始运行时触发
        /// </summary>
        /// <returns></returns>
        bool StartRun();
        /// <summary>
        /// 连扳测试，开始测试触发接口
        /// </summary>
        /// <param name="Infomation"></param>
        /// <returns></returns>
        bool StartTest(Dictionary<string, object> OnceConfig);
        /// <summary>
        /// 方法1、2、3，测试项目触发接口
        /// </summary>
        /// <param name="message">指令，决定调用哪个方法</param>
        /// <returns>方法调用后回传值</returns>
        string Run(string Command);
        /// <summary>
        /// 连扳测试，当所有线程测试结束后触发的接口
        /// </summary>
        /// <param name="obj"></param>
        void TestsEnd(object obj);


    }
}
