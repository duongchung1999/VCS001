using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MerryDllFramework.MerryDll dll = new MerryDllFramework.MerryDll();
            dll.Interface(new Dictionary<string, object>()
            {
                { "SN","TE_BZP"},
                { "BitAddress","F4B688C08F72"},
                { "Works","001100123456" },
                { "OrderNumberInformation","89M681002300"},
                { "PartNumberInfos",new Dictionary<string,string>()}
            }); ;
            dll.Start(null, IntPtr.Zero);
            dll.StartRun();
            string[] command =
            {
                "Open_Constant_Photograph",
               "PhotoGraph_P135D_1",
               //"Read_Stitching_Defect_Test",
               "CloseCamera",


            };
            foreach(string cmd in command)
            {
                Console.Write(cmd + "\t");
                Console.WriteLine(dll.Run(cmd));
            }
            
            Console.ReadKey();



        }
    }
}
