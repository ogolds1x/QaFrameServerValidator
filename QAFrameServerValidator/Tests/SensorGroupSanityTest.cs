using System;
using QAFrameServerValidator.Abstract_Factory;

using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace QAFrameServerValidator.Tests
{
    class SensorGroupSanityTest : TestBase
    {
        public SensorGroupSanityTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }

        public override void ExecuteTest()
        {
            //startTestTime = DateTime.Now;

            //if (profilesList?.Count > 0)
            //{
            //    bool status = true;
            //    foreach (BasicProfile profile in profilesList)
            //    {
            //        CameraUtil cu = new CameraUtil();

            //        status &= cu.Start(profile);
            //        System.Threading.Thread.Sleep(10000);
            //        status &= cu.FPS_Validation(profile.ProfileType.ToString(), profile.FishEyeMode.ToString(), (uint)profile.FishEyeFps, (uint)profile.FishEyeResolution.Width, (uint)profile.FishEyeResolution.Height, profile.Controls);
            //    }

            //    Logger.AppendMessage(Environment.NewLine);
            //    Console.WriteLine("\n\n\n");
            //}
            //endTestTime = DateTime.Now;
            //Logger.AppendInfo("End Test Time " + endTestTime);
            //Console.WriteLine("End Test Time " + endTestTime);

            FrameReaderUtil fr = new FrameReaderUtil();
            fr.SortGroups();
        }
        
    }
}
