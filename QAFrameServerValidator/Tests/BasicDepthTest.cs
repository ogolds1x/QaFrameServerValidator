using System;
using QAFrameServerValidator.Abstract_Factory;
using System.Configuration;

namespace QAFrameServerValidator.Tests
{
    /* Depth Test Class 
    * Flow : 1.Initialized CameraUtil  
    *        2.Extract profile configuration from profilesList
    *        3.Start test 
    *        4.Validate FPS 
    * */
    class BasicDepthTest : TestBase
    {
        public BasicDepthTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        
        public override void ExecuteTest()
        {
            bool testPassedFailed = true;
            bool status = true;
            startTestTime = DateTime.Now;
            foreach (BasicProfile profile in profilesList)
            {
                Logger.AppendInfo("**** Start Basic Depth Test ****");
                CameraUtil cu = new CameraUtil();
                status = true;
                do
                {
                    Logger.AppendInfo("Iteration number " + numberOfIteration);
                    status &= cu.Start(profile.ProfileType.ToString(), profile.DepthMode.ToString(), (uint)profile.DepthFps, (uint)profile.DepthResolution.Width, (uint)profile.DepthResolution.Height, profile.Controls, numberOfFramesToCollect, false);
                    System.Threading.Thread.Sleep(1000);
                    status &= cu.FPS_Validation(profile.ProfileType.ToString(), profile.DepthMode.ToString(), (uint)profile.DepthFps, (uint)profile.DepthResolution.Width, (uint)profile.DepthResolution.Height, profile.Controls);
                    numberOfIteration--;
                }
                while (numberOfIteration > 0);
                if (status == false)
                    testPassedFailed = false;
                Logger.AppendMessage(Environment.NewLine);
                Console.WriteLine("\n\n\n");
            }
            endTestTime = DateTime.Now;
            Logger.AppendDebug("Depth Test Status - {0}", testPassedFailed ? "Passed" : "Failed");
            Logger.AppendInfo("Test Time " + endTestTime.Subtract(startTestTime));
        }
      
    }
}
