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
    class PropertiesTest : TestBase
    {
        const int MILLISECONDS_TO_SLEEP_BETWEEN_SET = 0;
        public PropertiesTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        public PropertiesTest()
        {

        }
        FrameReaderUtil fr;
        public override void ExecuteTest()
        {
            //ArduinoReset.PerformCameraReset(13);//restart DS5 camera. 13 = using pin 13 on arduino. 
            //Thread.Sleep(10000); ///give 10 seconds for restart before starting test
            Logger.AppendMessage($"LOG_PATH: {Logger.path}");
            Logger.AppendMessage(Environment.NewLine);

            Logger.AppendMessage("AREA_NAME: Properties");
            Logger.AppendMessage("SUB_AREA_NAME: PropertiesTest");
            Logger.AppendInfo("Now starting Properties Test");
            //System.Console.WriteLine(profilesList);
            //Choose camera type and which test to run
            fr = new FrameReaderUtil();
            Console.WriteLine("before SelectCameraSource");
            fr.SetSettingsPropertiesTest("RGB");
            Task.Run(async () =>
            {
                await fr.SelectCameraSource(true);
            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                await fr.InitializeMediaCapture(true);
            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                await fr.SetNonSpecificFrameFormat(true);
            }).GetAwaiter().GetResult();
            fr.SetSettingsPropertiesTest("Depth");
            Task.Run(async () =>
            {
                await fr.SelectCameraSource(false);
                await fr.InitializeMediaCapture(false);
                await fr.SetNonSpecificFrameFormat(false);
            }).GetAwaiter().GetResult();
            SetControl.setup(controlsList, fr.GetColorMediaCapture(), fr.GetDepthMediaCapture(),fr.GetFishEyeMediaCapture());
            SetControl.testPassed = true;
            //SetControl.printAllControlValues(true);
            for (int i = 0; i < controlsList.Count(); i++)
            {
                SetControl.setPropertyRange(controlsList[i].Control, false, 0);
                Logger.AppendMessage(Environment.NewLine);
                //System.Console.WriteLine(controlsList[i].ArrayValues[0]);
                //System.Console.WriteLine(controlsList[i].ListValues.Count);
                //controlsList[i].
                //System.Console.WriteLine(controlsList[i].Control);
            }
           
        }
     
    }
}