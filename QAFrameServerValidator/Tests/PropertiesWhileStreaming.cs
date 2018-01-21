using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QAFrameServerValidator.Abstract_Factory;
//using ResetDS5;
namespace QAFrameServerValidator.Tests
{
    class PropertiesWhileStreamingTest : TestBase
    {
        const int MILLISECONDS_TO_SLEEP_BETWEEN_SET = 0;
        private const int numberOfFramesToCollect = 7000;
        //private const int numberOfFramesToCollect = 1000;

        public PropertiesWhileStreamingTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        public PropertiesWhileStreamingTest()
        {

        }
        public override void ExecuteTest()
        {
            //ArduinoReset.PerformCameraReset(13);//restart DS5 camera. 13 = using pin 13 on arduino. 
            //Thread.Sleep(10000); ///give 10 seconds for restart before starting test
            Console.WriteLine($"LOG_PATH: {Logger.path}");
            Logger.AppendInfo("Now starting Properties While Streaming Test");
            CameraUtil cu = new CameraUtil();
            SetControl.testPassed = true;
            Console.WriteLine("profilesList.Count() " + profilesList.Count());
            BasicProfile profile = (BasicProfile)profilesList[0];
            Thread frameReader = new Thread(delegate ()
            {
                if (profile.ProfileType.ToString() == "Color")
                {
                    cu.Start(profile.ProfileType.ToString(), profile.ColorMode.ToString(), (uint)profile.ColorFps, (uint)profile.ColorResolution.Width, (uint)profile.ColorResolution.Height, profile.Controls, (int)numberOfFramesToCollect, true);
                }
                else
                {
                    cu.Start(profile.ProfileType.ToString(), profile.DepthMode.ToString(), (uint)profile.DepthFps, (uint)profile.DepthResolution.Width, (uint)profile.DepthResolution.Height, profile.Controls, (int)numberOfFramesToCollect, true);
                }
            });
            Thread setProperties = new Thread(delegate ()
            {
                SetControl.setup(controlsList, cu.GetColorMediaCapture(), cu.GetDepthMediaCapture(),null);
                //SetControl.setPropertyToSpecificValue(Types.ControlKey.COLOR_EXPOSURE_PRIORITY, 0);
                //cu.GetColorMediaCapture().VideoDeviceController.Exposure.TrySetAuto(false);
                if (cu.getProductName().Contains("SR300"))
                {
                    if (profile.ProfileType.ToString().Contains("Depth"))
                    {

                        SetControl.setPropertyToSpecificValue(Types.ControlKey.DEPTH_MOTION_VS_RANGE_TRADE, (int)SetControl.SR300DepthMotionVSRangeTradeoffDefaultValue);
                        SetControl.setPropertyToSpecificValue(Types.ControlKey.DEPTH_ACCURACY, (int)SetControl.SR300DepthAccuracyDefaultValue);
                    }
                    else
                    {
                        cu.GetColorMediaCapture().VideoDeviceController.Exposure.TrySetAuto(false);
                        SetControl.setPropertyToSpecificValue(Types.ControlKey.COLOR_EXPOSURE, (int)SetControl.SR300ColorExposureDefaultValue);
                    }
                }
                else if ((cu.getProductName().Contains("DS5") || cu.getProductName().Contains("400") || cu.getProductName().Contains("410") || cu.getProductName().Contains("430"))&& profile.ProfileType.ToString().Contains("Depth"))
                {
                    SetControl.setPropertyToSpecificValue(Types.ControlKey.DS5_DEPTH_EXPOSURE, (int)SetControl.DS5ExposureDefaultValue);
                }
                else
                {
                    Logger.AppendError("UNRECOGNIZED CAMERA!");
                }
                for (int i = 0; i < controlsList.Count(); i++)
                {
                    if (controlsList[i].Control != Types.ControlKey.COLOR_EXPOSURE_PRIORITY)
                    {
                        SetControl.setPropertyRange(controlsList[i].Control, false, 0);
                    }
                }
                System.Console.WriteLine("DONE SETTING PROPERITES");
                cu.FinishedSettingControls();
            });
            frameReader.Start();
            cu.getMediaCaptureReady().WaitOne();//Don't start setting properties until mediacapture objects are initialized
            setProperties.Start();
            Console.WriteLine("before cu.GetFinishedSettingControls().WaitOne()");
            cu.GetFinishedSettingControls().WaitOne();
            Console.WriteLine("after cu.GetFinishedSettingControls().WaitOne()");
            cu.GetFinishedStreaming().WaitOne();
            Console.WriteLine("after cu.GetFinishedStreaming().WaitOne();");
        }

    }
}