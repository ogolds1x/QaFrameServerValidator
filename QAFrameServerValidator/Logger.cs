using System;
using System.IO;

namespace QAFrameServerValidator
{
    public static class Logger
    {
        public static string path;
        public static string directory;
        public static string logFileName;
        public static StreamWriter logFile;

        public static void GetStreamWriter()
        {
            string strPath = Path.Combine("WOSLog\\", "WindowsFrameServerValidatorLog_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            path = Path.Combine(Directory.GetCurrentDirectory(), "WOSLog");
            Directory.CreateDirectory(path);
            logFile = new System.IO.StreamWriter(strPath);
        }

        public static void AppendInfo(string message, params object[] args)
        {
            logFile.Write(DateTime.Now.ToString("MM/dd/yy  H:mm:ss.f  "));
            logFile.Write("Info  ");
            if (args.Length > 0)
            {
                logFile.WriteLine(message, args);
            }
            else
            {
                logFile.WriteLine(message);
            }
        }

        public static void AppendDebug(string message, params object[] args)
        {
            logFile.Write(DateTime.Now.ToString("MM/dd/yy  H:mm:ss.f  "));
            logFile.Write("Debug  ");
            if (args.Length > 0)
            {
                logFile.WriteLine(message, args);
            }
            else
            {
                logFile.WriteLine(message);
            }
        }

        public static void AppendException(string message, params object[] args)
        {
            logFile.Write(DateTime.Now.ToString("MM/dd/yy  H:mm:ss.f  "));
            logFile.Write("An Exception Has Occurred:  ");
            if (args.Length > 0)
            {
                logFile.WriteLine(message, args);
            }
            else
            {
                logFile.WriteLine(message);
            }
            FrameReaderUtil.exceptionError = true;
        }

        public static void AppendError(string message, params object[] args)
        {
            logFile.Write(DateTime.Now.ToString("MM/dd/yy  H:mm:ss.f  "));
            logFile.Write("An Error Has Occurred:  ");
            if (args.Length > 0)
            {
                logFile.WriteLine(message, args);
            }
            else
            {
                logFile.WriteLine(message);
            }
        }

        public static void AppendMessage(string message, params object[] args)
        {
            Console.WriteLine(message);
            if (args.Length > 0)
            {
                logFile.WriteLine(message, args);             
            }
            else
            {
                logFile.WriteLine(message);
            }
        }

        public static void NewLine()
        {
            logFile.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
        }

        public static void CloseFile()
        {
            logFile.Close();
        }
    }

}