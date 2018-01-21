using QAFrameServerValidator.Abstract_Factory;
using System;
using System.Linq;
using System.Threading;
using System.Configuration;

namespace QAFrameServerValidator.Tests
{
    /* Sanity Parallel Stream Test Class 
    *  Flow : 1.Initialized CameraUtil  
    *        2.Extract profile configuration from profilesList
    *        3.Start test 
    *        4.Validate FPS 
    * */
    public class SanityParallelStreamTest : TestBase
    {
        public SanityParallelStreamTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }

        public override void ExecuteTest()
        {
            FPS_Analysis analyser = new FPS_Analysis();
            bool status = true;
            do
            {
                
                foreach (BasicProfile profile in profilesList)
                {
                    try
                    {
                        Stream stream = new Stream(profile);
                        stream.FLow();
                        Thread.Sleep(1500);
                        Logger.AppendMessage($"PROFILE_NAME: {ProfilesToExclude.ProfileToString(profile)}");
                        analyser.profileName = ProfilesToExclude.ProfileToString(profile);
                        status =  analyser.Check_HW_SyncTimeStamp_Color_Depth(profile.DepthFps, stream.FramesArrived);
                        Logger.AppendDebug("Synced - {0}" , status?"Passed":"Failed");
                    }
                    catch(Exception e)
                    {
                        Logger.AppendException(e.Message);
                    }
                }
            }
            while (--numberOfIteration != 0);
            analyser.PrintSyncAvgAnalysis();
            Logger.AppendDebug("Synced Test - {0}", status ? "Passed" : "Failed");
            Thread.Sleep(3000);
        }
    }
}

