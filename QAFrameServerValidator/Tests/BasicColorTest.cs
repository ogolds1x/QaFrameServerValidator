using System;
using System.Threading;
using QAFrameServerValidator.Abstract_Factory;

namespace QAFrameServerValidator.Tests
{
    /* Color Test Class 
     * Flow : 1.Initialized CameraUtil  
     *        2.Extract profile configuration from profilesList
     *        3.Start test 
     *        4.Validate FPS 
     * */
    public class BasicColorTest : TestBase
    {
        public BasicColorTest(Factory _factory)
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
                Logger.AppendInfo("**** Start Basic Color Test ****");

                CameraUtil cu = new CameraUtil();
                 status = true;
                do
                {
                    Logger.AppendInfo("Iteration number " + numberOfIteration);
                    status &= cu.Start("RGB", profile.ColorMode.ToString(), (uint)profile.ColorFps, (uint)profile.ColorResolution.Width, (uint)profile.ColorResolution.Height, profile.Controls, numberOfFramesToCollect, false);
                    Thread.Sleep(10000);
                    status &= cu.FPS_Validation("RGB", profile.ColorMode.ToString(), (uint)profile.ColorFps, (uint)profile.ColorResolution.Width, (uint)profile.ColorResolution.Height, profile.Controls);
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
            Logger.AppendInfo("Test Time "+ endTestTime.Subtract(startTestTime));
        }
        
    }
}
