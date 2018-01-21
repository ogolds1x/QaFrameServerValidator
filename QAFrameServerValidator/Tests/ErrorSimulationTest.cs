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
//using System.Globalization;

//using ResetDS5;
namespace QAFrameServerValidator.Tests
{
    class ErrorSimulationTest : TestBase
    {

        private const uint numberOfFramesToCollect = 80;

        bool isStreamingPassed;

        FrameReaderUtil fr;


        private static ManualResetEvent mediaCaptureReady = new ManualResetEvent(false);
        private ManualResetEvent StartWhileMeasurementTempStreaming;



        public ErrorSimulationTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        public ErrorSimulationTest()
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
             *  Register error handler events on Linux - not in this test
                o In another thread simulate FW errors by HW monitor commands. (List Of commands - according to FW spec)

             * 
                    - After each simulated error, Error event should be received. 
                    - Error type should be compatible with the error was simulated. 
            */

            /*
           //establishing connection to the camera- i will need this in order to use the hardware command
           fr = new FrameReaderUtil();
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

           //small check to see that the camera is not null
           if (fr.GetDepthMediaCapture() == null)
               throw new Exception("The camera is not connected.. check camera Frames Sample (windows universal)");
           Console.WriteLine("AVIKOVA connection success!!");
           */












            //start streaming using my function, simulating error and listening to the event

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






            Thread.Sleep(10000);




            //Console.WriteLine(FrameReaderUtil.isSuccess);
            //Console.WriteLine($"ISTESTPASSED? = {IsTestPassed()}");






            return;


            //I tried to create a structure and send it using the extended command to use hw command (mm_temp) still doens't work

            //KSPROPERTY_SR300_CONTROL_HW_COMMAND mm_temp_command = new KSPROPERTY_SR300_CONTROL_HW_COMMAND();
            //mm_temp_command.opCode = 0x2A;
            //mm_temp_command.bufferLength = 1;
            //mm_temp_command.param1 = mm_temp_command.param2 = mm_temp_command.param3 = 0;
            //mm_temp_command.param4 = 0;
            //mm_temp_command.buffer = new byte();


            //int size_of_mm_temp_command = Marshal.SizeOf<KSPROPERTY_SR300_CONTROL_HW_COMMAND>();
            //byte[] data_of_mm_temp_command = PdoToBuffer<KSPROPERTY_SR300_CONTROL_HW_COMMAND>(mm_temp_command);

            ////uint size = 4;

            //var result = controller.GetDevicePropertyByExtendedId(data_of_mm_temp_command, (uint)size_of_mm_temp_command);

            //Console.WriteLine(result.Status);



            

            //KSPROPERTY_SR300_CONTROL_HW_COMMAND DEPTH_EU_ERROR_SET_COMMAND = new KSPROPERTY_SR300_CONTROL_HW_COMMAND();
            //DEPTH_EU_ERROR_SET_COMMAND.opCode = 0x4D;
            //DEPTH_EU_ERROR_SET_COMMAND.bufferLength = 1;
            //DEPTH_EU_ERROR_SET_COMMAND.param1 = DEPTH_EU_ERROR_SET_COMMAND.param2 = DEPTH_EU_ERROR_SET_COMMAND.param3 = 0;
            //DEPTH_EU_ERROR_SET_COMMAND.param4 = 0;
            //DEPTH_EU_ERROR_SET_COMMAND.buffer = new byte();


            //int depth_command_value = 2;
            //Byte[] command_value = BitConverter.GetBytes(depth_command_value);
            //byte[] data_of_size_of_DEPTH_EU_ERROR_SET_COMMAND = PdoToBuffer<KSPROPERTY_SR300_CONTROL_HW_COMMAND>(DEPTH_EU_ERROR_SET_COMMAND);

            ////uint size = 4;


            //controller.SetDevicePropertyByExtendedId(data_of_size_of_DEPTH_EU_ERROR_SET_COMMAND, command_value);

            //Console.WriteLine("simulate error using the depth_eu_error_set command , number of error is: "+ depth_command_value);


        }

  


        /*
         * when we will use bessudo's code we can uncomment this
         * 
        public static byte[] PdoToBuffer<T>(T obj)
        {
            int len = Marshal.SizeOf<T>();
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr<T>(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        */
    }
}
