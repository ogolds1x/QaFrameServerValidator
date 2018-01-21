// PNP_Test_Multi

using System;
using QAFrameServerValidator.Abstract_Factory;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace QAFrameServerValidator.Tests
{
    public class PNP_Test_Multi : TestBase
    {
        public PNP_Test_Multi(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        public override void ExecuteTest()
        {
           
           
                foreach (BasicProfile profile in profilesList)
                {
                Thread streamThread = new Thread(delegate ()
                {
                    Task.Run(() =>
                    {
                        Stream stream = new Stream(profile);
                         stream.FLow();
                        Thread.Sleep(1000);
                        //Logger.AppendMessage($"PROFILE_NAME: {ProfilesToExclude.ProfileToString(profile)}");
                        if (stream.FramesArrived == null || stream.FramesArrived.Count <= 1)
                        {
                            Logger.AppendError("No frame in buffer");
                        }
                    }).Wait();
                 });

                
                ProcessStartInfo startInfo = new ProcessStartInfo();
                Process proc = new Process();
                try
                {

                    var wD = Environment.CurrentDirectory + @"\Run_Test\";

                    startInfo.FileName = "run_socwatch.bat";
                    //Console.WriteLine(wD);
                    startInfo.WorkingDirectory = wD;
                    string str = profile.DepthResolutionAsString + "x" + profile.DepthFps.ToString() + "x" + profile.Controls.Where(x => x.Name.ToUpper().Contains("LASER")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
                    startInfo.Arguments = str;
                    startInfo.UseShellExecute = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed: " + e.Message);

                }

                streamThread.Start();
                Thread.Sleep(Program.waitBetweenStreamAndPnp);

                if (Program.pnpTest == 1)
                {
                    proc.StartInfo = startInfo;
                    proc.Start();
                    proc.WaitForExit();
                }
                Program._waitHandle.WaitOne();
              
            }
                
           



            // ************************************
            //Console.WriteLine("Number of profile: " + profilesList.Count);
            //int i = 0;
            //foreach (BasicProfile profile in profilesList)
            //{
            //    MultiFrameReader mfr = new MultiFrameReader();
            //    mfr.SetConfig(profile);
            //    Console.WriteLine("Number of profile " + i);
            //    Thread streamThread = new Thread(delegate ()
            //    {
            //        Task.Run(async () =>
            //        {

            //            mfr.SetConfig(profile.ProfileType.ToString(), profile.DepthMode.ToString(), (uint)profile.DepthFps, (uint)profile.DepthResolution.Width, (uint)profile.DepthResolution.Height, profile.Controls);
            //            await mfr.SelectCameraSource();
            //            await mfr.CreateFrameReader();
            //            Thread.Sleep(1000);
            //            i++;
            //        }).Wait();
            //    });



            //    streamThread.Start();
            //    mfr.GetMediaFrameReaderReady().WaitOne();


            //    ProcessStartInfo startInfo = new ProcessStartInfo();
            //    Process proc = new Process();
            //    try
            //    {

            //        var wD = Environment.CurrentDirectory + @"\Run_Test\";

            //        startInfo.FileName = "run_socwatch.bat";
            //        Console.WriteLine(wD);
            //        startInfo.WorkingDirectory = wD;
            //        string str = profile.DepthResolutionAsString + "x" + profile.DepthFps.ToString() + "x" + profile.Controls.Where(x => x.Name.ToUpper().Contains("LASER")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
            //        startInfo.Arguments = str;
            //        startInfo.UseShellExecute = true;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Failed: " + e.Message);

            //    }

            //    proc.StartInfo = startInfo;
            //    proc.Start();
            //    proc.WaitForExit();
            //    mfr.GetFinishedCleaning().WaitOne();

         }
        
        //public override bool IsTestPassed()
        //{
        //    if (testStatus == Status.Passed)
        //        return true;
        //    return false;
        //}

    }
}

