using System;
using System.IO;
using System.Configuration;
using System.Threading;

namespace QAFrameServerValidator
{
    public class Program
    {
        public static int streamTimeInMilliseconds;
        public static int waitBetweenStreamAndPnp;
        public static int pnpTest;
        public static string sku;
        public static EventWaitHandle _waitHandle = new AutoResetEvent(false);
        static int Main(string[] args)
        {
            streamTimeInMilliseconds = int.Parse(ConfigurationSettings.AppSettings["streamTimeInMilliseconds"].ToString());
            waitBetweenStreamAndPnp = int.Parse(ConfigurationSettings.AppSettings["waitBetweenStreamAndPnp"].ToString());
            pnpTest = int.Parse(ConfigurationSettings.AppSettings["pnpTest"].ToString());

            TestManager testManager = new TestManager();            
            string strPath = Path.Combine("WOSLog\\", "WindowsFrameServerValidatorLog_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            using (Logger.logFile = new System.IO.StreamWriter(strPath))
            {
                try
                {
                    SystemInfo.GetInstance();
                    testManager.CreateTest();
                    Factory.Instance.ExecuteAllTests();
                    //testManager.CreateTest(Types.SubArea.SanityParallelStreamTest);
                    //testManager.testBase.ExecuteTest();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Logger.AppendException(e.Message);
                }
                finally
                {
                    Logger.CloseFile();
                }
            }  
            return 0;
        }


    }

}
