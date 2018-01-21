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
    
    class ThermalAlarmWhileStreamingTest : TestBase
    {


        private const uint numberOfFramesToCollect = 80;
        bool isStreamingPassed;
        private ManualResetEvent StartWhileMeasurementTempStreaming;





        public ThermalAlarmWhileStreamingTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        public ThermalAlarmWhileStreamingTest()
        {

        }


        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 1, Size = 25)]
        struct KSPROPERTY_SR300_CONTROL_HW_COMMAND
        {
            [FieldOffset(0)]
            public UInt32 opCode;
            [FieldOffset(4)]
            public UInt32 bufferLength;
            [FieldOffset(8)]
            public UInt32 param1;
            [FieldOffset(12)]
            public UInt32 param2;
            [FieldOffset(16)]
            public UInt32 param3;
            [FieldOffset(20)]
            public UInt32 param4;
            [FieldOffset(24)]
            public byte buffer;
        };


        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //struct KSPROPERTY_SR300_CONTROL_HW_COMMAND
        //{
        //    public UInt32 opCode;
        //    public UInt32 bufferLength;
        //    public UInt32 param1;
        //    public UInt32 param2;
        //    public UInt32 param3;
        //    public UInt32 param4;
        //    public Byte[] buffer = new Byte[1];
        //};




        public override void ExecuteTest()
        {

            /*
             * Thermal Alarm – while streaming
	            Test flow:
	            •	Start streaming of depth Z/HD/90Fps -> not supported format
	            •	Using the API of Microsoft Register the events of the errors. -> TODO talk to Ofir and Shadi
	            •	Set laser power to maximum
	            •	Using Programmable Oven, heat the unit to > 50 C in case of AWG, 60 C in case of Not AWG.
	            •	Check laser power value
	            Acceptance criteria:
	            -	Laser power value, should be half from the original value was set before.
	            -	Streaming should not stuck.
	            -	Error Event (Thermal alarm) should be arrived from the Windows API 	
             * 
             */


            /*
             *  in order to check that the temperature is correct we can use HWD commands: GTEMP and MM_TEMP to use them we need the following code: 
             *   //we will use Yaniv dll for now (until we will learn how to send hw commands using the realsense.api)
            Guid DSGUID = new Guid("08090549-CE78-41DC-A0FB-1BD66694BB0C "); //the guid i got from Yaniv
            var winUSBDevice = new HWMonitorDevice("8086", "0AD2", DSGUID, 1); //setting of the command.. like Sam's code
            var parser = new CommandsXmlParser("Commands.xml"); //the xml file where you find all commands availble
            CommandResult result = parser.SendCommand(winUSBDevice, "GTEMP"); //insert the HW command, the same as typing in the Terminal
            string val = result.FormatedString; //the result of the command, returns the the result if the command was succesfull
            val = GetTempFromResult(val); //leaving only the number, changed from hexadecimal to decimal number- should be between 0 to 70
            Console.WriteLine("temp = " + val);
            */






            Console.WriteLine("ThermalAlarmWhileStreamingTest started!");




            // Set laser power to maximum
            SetControl.setPropertyToSpecificValue(Types.ControlKey.DS5_DEPTH_LASER_ON_OFF, (int)SetControl.DS5LaserOnOffMax);


            //SetControl.setPropertyToSpecificValue(Types.ControlKey.DS5_DEPTH_LASER_POWER, (int)SetControl.DS5LaserPowerMax);


            //start streaming using my function, register to thermal events in the callback.

            StartWhileMeasurementTempStreaming = new ManualResetEvent(false);

            Console.WriteLine($"LOG_PATH: {Logger.path}");
            Logger.AppendInfo("Now starting temperature while streaming test");
            CameraUtil cu = new CameraUtil();
            SetControl.testPassed = true;
            Console.WriteLine("profilesList.Count() " + profilesList.Count());
            BasicProfile profile = (BasicProfile)profilesList[0];
            Thread frameReader_frameReader_Temperature = new Thread(delegate ()
            {
                if (profile.ProfileType.ToString() == "Color")
                {
                    isStreamingPassed = cu.StartWhileMeasurementTempStreaming(profile.ProfileType.ToString(), profile.ColorMode.ToString(), (uint)profile.ColorFps, (uint)profile.ColorResolution.Width, (uint)profile.ColorResolution.Height, profile.Controls, (int)numberOfFramesToCollect, false);
                    //cu.Start(profile.ProfileType.ToString(), profile.ColorMode.ToString(), (uint)profile.ColorFps, (uint)profile.ColorResolution.Width, (uint)profile.ColorResolution.Height, profile.Controls, (int)numberOfFramesToCollect, true);
                }
                else //run depth
                {
                    isStreamingPassed = cu.StartWhileMeasurementTempStreaming(profile.ProfileType.ToString(), profile.DepthMode.ToString(), (uint)profile.DepthFps, (uint)profile.DepthResolution.Width, (uint)profile.DepthResolution.Height, profile.Controls, (int)numberOfFramesToCollect, false);
                    //cu.Start(profile.ProfileType.ToString(), profile.DepthMode.ToString(), (uint)profile.DepthFps, (uint)profile.DepthResolution.Width, (uint)profile.DepthResolution.Height, profile.Controls, (int)numberOfFramesToCollect, true);
                }
                StartWhileMeasurementTempStreaming.Set();
            });

            frameReader_frameReader_Temperature.Start();
            cu.getMediaCaptureReady().WaitOne();//Don't start setting properties until mediacapture objects are initialized
            StartWhileMeasurementTempStreaming.WaitOne();













            //Check laser power value
            double laserPowerValue = SetControl.GetControlValue(Types.ControlKey.DS5_DEPTH_LASER_ON_OFF);

            // Laser power value, should be half from the original value was set before.
            if (laserPowerValue == SetControl.DS5LaserOnOffMax / 2)
            {
                Console.WriteLine("The ThermalAlarmWhileStreamingTest was successfull! Horray!");
            }
            else
            {
                Console.WriteLine("The ThermalAlarmWhileStreamingTest failed! Boo!");
            }


            //TODO: add the listener to the thermal event Using the Windows API - ofir and shadi




            Thread.Sleep(10000);






            return;
        }



            /*
             * this code is for using the oven:
            #region General declerations
            //IM.AgDmm34401A DMM1;
            IM.Thermotron2800 oven;
            #endregion

            #region Test parameters init
            //double[] Temp_arr = new double[] { 999 };
            //double[] Temp_arr = new double[] { 5, 25, 45, 65 };
            double[] Temp_arr = new double[] { 25 };


            #endregion
            #region Members
            double[] m_DC_measure;

            int m_DAC_BitsNum;
            //protected int m_Dig_Word;
            //protected int[,] DC_Wave;
            //protected double[] m_DC_measure_1;
            //protected double[] m_DC_measure_2;
            double m_Dynamic_Range;
            double m_LSB;
            double[] m_DNL_all, m_INL_all;
            double m_INL, m_DNL;
            //protected int m_SampleBits;
            double[] code_index;

            int m_Dig_Word;

            string RegsPwmRatio, RegsPwmPeriodSlow, RegsPwmPeriodFast, RegsPwmLowMin, RegsPwmLowMinHalf, RegsPwmClkRatio, BrkTonTime, BrkToffTime;


            #endregion

            #region Oven
            oven = new IM.Thermotron2800();
            oven.Init(1);
            #endregion


            #region Init member variables
            m_DAC_BitsNum = 12;
            int DAC_Level_num = Convert.ToInt32(Math.Pow(2, m_DAC_BitsNum));
            m_DNL_all = new double[DAC_Level_num - 1];
            m_INL_all = new double[DAC_Level_num - 1];
            m_DC_measure = new double[DAC_Level_num];
            //m_DC_measure_1 = new double[DAC_Level_num];
            //m_DC_measure_2 = new double[DAC_Level_num];
            #endregion


            #region Set temperature
            double Temperature = Temp_arr[0];


            oven.SetToDefaults();
            oven.OvenState = 1;


            //Set_Oven(Temperature, 1);
            double soak_Time_min = 1;




            Console.WriteLine("oven warming up");
            oven.OvenState = 1;

            oven.TargetTemperature = Temperature;
            double current_temp = oven.CurrentTemperature;
            while ((current_temp > Temperature + 1) || (current_temp < Temperature - 1))
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Thread.Sleep(7000);
                current_temp = oven.CurrentTemperature;
                Console.WriteLine("current temp : " + current_temp);

            }

            //oven.SetTemperatureWithSoakTime(temp, 0);
            Console.SetCursorPosition(0, Console.CursorTop - 1);

            double time = soak_Time_min * 60;  //convert to seconds ;   5min = 300 sec

            while (time != 0)
            {
                Console.WriteLine("Soaking Time on : " + Temperature + "'c " + time + "  sec left  ");
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Thread.Sleep(1000); //sleep 1 sec
                time--;
            }
            Console.WriteLine("warming up: " + time + "  sec left  ");  //so 0 would appear




            if (oven.OvenState == 1) { oven.StopOven(); }

            #endregion








        }
        */


        /*
         * this code is used for getting the temperature (GTEMP MM_TEMP):
         *  private string GetTempFromResult(string val)
        {
            int Start;
            if (val.Contains('>'))
            {
                Start = val.IndexOf('>', 0) + 1;
                val = val.Substring(Start);
            }
            else
            {
                return "the line does not have the sign '>' need to modify the GetTempFromResult function";
            }

            Console.WriteLine("found the temperature in hexa: " + val);  //the temperature in hexa

            val = Int32.Parse(val, NumberStyles.HexNumber).ToString(); //converting to decimal
            Console.WriteLine("found the temperature in decimal: " + val);  //the temperature in decimal

            return val;
        }
        */


    }
}