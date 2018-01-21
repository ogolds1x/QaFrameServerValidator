using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QAFrameServerValidator.Abstract_Factory;
using static QAFrameServerValidator.Tests.CheckDefaultControlValuesTest;
using System.Runtime.InteropServices;
//using Winusb.Cli;
//using UVC.Cli;
using System.Globalization;

//using ResetDS5;
namespace QAFrameServerValidator.Tests
{



    class ThermalAlarmWithoutStreamingTest : TestBase
    {


        private const uint numberOfFramesToCollect = 80;
        
        FrameReaderUtil fr;





        public ThermalAlarmWithoutStreamingTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        public ThermalAlarmWithoutStreamingTest()
        {

        }




        public override void ExecuteTest()
        {

            ///*
            // * Main test flows:
               
            //1. Thermal Alarm - no streaming
            //Test flow:
            //• Using the Windows API register the events of the errors.
            //• Set laser power to maximum
            //• Using Programmable Oven, heat the unit to > 50 C in case of AWG, 60 C in case of Not AWG.
            //• Check laser power value

            //Acceptance criteria:
            //• Laser power value, should be half from the original value was set before.
            //• Error Event (Thermal alarm) should be arrived from the Windows API 
            // * 
            //*/


            ////establishing connection to the camera- i will need this in order to use the hardware command
            //fr = new FrameReaderUtil();
            //fr.SetSettingsPropertiesTest("RGB");
            //Task.Run(async () =>
            //{
            //    await fr.SelectCameraSource(true);
            //}).GetAwaiter().GetResult();
            //Task.Run(async () =>
            //{
            //    await fr.InitializeMediaCapture(true);
            //}).GetAwaiter().GetResult();
            //Task.Run(async () =>
            //{
            //    await fr.SetNonSpecificFrameFormat(true);
            //}).GetAwaiter().GetResult();
            //fr.SetSettingsPropertiesTest("Depth");
            //Task.Run(async () =>
            //{
            //    await fr.SelectCameraSource(false);
            //    await fr.InitializeMediaCapture(false);
            //    await fr.SetNonSpecificFrameFormat(false);
            //}).GetAwaiter().GetResult();


            ////your code starts here!

            //// Set laser power to maximum
            //SetControl.setPropertyToSpecificValue(Types.ControlKey.DS5_DEPTH_LASER_ON_OFF, (int)SetControl.DS5LaserOnOffMax);


            //// Using Programmable Oven, heat the unit to > 50 C in case of AWG, 60 C in case of Not AWG.
            //#region General declerations
            ////IM.AgDmm34401A DMM1;
            //IM.Thermotron2800 oven;
            //#endregion

            //#region Test parameters init
            ////we use ds5 asr for now
            //double[] Temp_arr = new double[] { 61 };
            ////if awg use this instead:
            ////double[] Temp_arr = new double[] { 51 };


            //#endregion
            //#region Members
            //double[] m_DC_measure;

            //int m_DAC_BitsNum;
            //double[] m_DNL_all, m_INL_all;



            //#endregion

            //#region Oven
            //oven = new IM.Thermotron2800();
            //oven.Init(1);
            //#endregion


            //#region Init member variables
            //m_DAC_BitsNum = 12;
            //int DAC_Level_num = Convert.ToInt32(Math.Pow(2, m_DAC_BitsNum));
            //m_DNL_all = new double[DAC_Level_num - 1];
            //m_INL_all = new double[DAC_Level_num - 1];
            //m_DC_measure = new double[DAC_Level_num];
            //#endregion


            //#region Set temperature
            //double Temperature = Temp_arr[0];


            //oven.SetToDefaults();
            //oven.OvenState = 1;

            //double soak_Time_min = 1;




            //Console.WriteLine("oven warming up");
            //oven.OvenState = 1;

            //oven.TargetTemperature = Temperature;
            //double current_temp = oven.CurrentTemperature;
            //while ((current_temp > Temperature + 1) || (current_temp < Temperature - 1))
            //{
            //    Console.SetCursorPosition(0, Console.CursorTop - 1);
            //    Thread.Sleep(7000);
            //    current_temp = oven.CurrentTemperature;
            //    Console.WriteLine("current temp : " + current_temp);

            //}

 
            //Console.SetCursorPosition(0, Console.CursorTop - 1);

            // double time = soak_Time_min * 60;  //convert to seconds ;   5min = 300 sec

            // while (time != 0)
            //{
            //    Console.WriteLine("Soaking Time on : " + Temperature + "'c " + time + "  sec left  ");
            //    Console.SetCursorPosition(0, Console.CursorTop - 1);
            //    Thread.Sleep(1000); //sleep 1 sec
            //    time--;
            //}
            //Console.WriteLine("warming up: " + time + "  sec left  ");  //so 0 would appear




            //if (oven.OvenState == 1) { oven.StopOven(); }

            //#endregion


            ////Check laser power value
            //double laserPowerValue =  SetControl.GetControlValue(Types.ControlKey.DS5_DEPTH_LASER_ON_OFF );

            //// Laser power value, should be half from the original value was set before.
            //if(laserPowerValue == SetControl.DS5LaserOnOffMax / 2)
            //{
            //    Console.WriteLine("The ThermalAlarmWithoutStreamingTest was successfull! Horray!");
            //}else
            //{
            //    Console.WriteLine("The ThermalAlarmWithoutStreamingTest failed! Boo!");
            //}


            ////TODO: add the listener to the thermal event Using the Windows API - ofir and shadi


        }

        }
    }