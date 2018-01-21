using System;
using QAFrameServerValidator.Abstract_Factory;

namespace QAFrameServerValidator.Tests
{
    class BasicFishEyeTest : TestBase
    {
        /* FishEye Test Class 
         * Flow : 1.Initialized CameraUtil  
         *        2.Extract profile configuration from profilesList
         *        3.Start test 
         *        4.Validate FPS 
         * */
        public BasicFishEyeTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }

        public override void ExecuteTest()
        {
            Logger.AppendInfo("**** Start Basic FishEye Test ****");
            bool status = true;         
            if (profilesList.Count > 0)
            {                
                foreach (BasicProfile profile in profilesList)
                {                   
                    CameraUtil cu = new CameraUtil();
                    status = cu.Start(profile.ProfileType.ToString(), profile.FishEyeMode.ToString(), (uint)profile.FishEyeFps, (uint)profile.FishEyeResolution.Width, (uint)profile.FishEyeResolution.Height, profile.Controls, numberOfFramesToCollect, false);
                    System.Threading.Thread.Sleep(10000);
                    status &= cu.FPS_Validation(profile.ProfileType.ToString(), profile.FishEyeMode.ToString(), (uint)profile.FishEyeFps, (uint)profile.FishEyeResolution.Width, (uint)profile.FishEyeResolution.Height, profile.Controls);
                    Logger.AppendDebug("Status: " + status);                    
                }
                Logger.AppendMessage(Environment.NewLine);
                Console.WriteLine("\n\n\n");
            }
            endTestTime = DateTime.Now;
            Logger.AppendInfo("Test Time " + endTestTime.Subtract(startTestTime));
        }

    }
}
