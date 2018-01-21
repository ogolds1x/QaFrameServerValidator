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
using System.Globalization;

//using ResetDS5;
namespace QAFrameServerValidator.Tests
{
    class IMUTemperatureMeasurementWithoutStreamingTest : TestBase
    {
        //bool isStreamingPassed;

        //FrameReaderUtil fr;





        public IMUTemperatureMeasurementWithoutStreamingTest(Factory _factory)
        {
            this.factory = _factory;
            factory.AddTest(this);
        }
        public IMUTemperatureMeasurementWithoutStreamingTest()
        {

        }

        public override void ExecuteTest()
        {
          //  /*
          //   * Main test flows:
          //      1. IMU Temperature Read w/o streaming
          //      Get temperature of IMU
          //      • via Sending IMU read command
          //      • via public API in Windows. - Shadi and Ofir test
          //      expected results:
          //      •	Get command/ API should success.
          //      •	Check that temp in acceptable range 0-70.
          //   * 
          //  */


            
          ////in order to check that the temperature is correct we can use HWD commands: GTEMP and MM_TEMP to use them we need the following code: 
          ////we will use Yaniv dll for now (until we will learn how to send hw commands using the realsense.api)
          //Guid DSGUID = new Guid("08090549-CE78-41DC-A0FB-1BD66694BB0C "); //the guid i got from Yaniv
          //var winUSBDevice = new HWMonitorDevice("8086", "0AD2", DSGUID, 1); //setting of the command.. like Sam's code
          //var parser = new CommandsXmlParser("Commands.xml"); //the xml file where you find all commands availble
          //CommandResult result = parser.SendCommand(winUSBDevice, "GTEMP"); //insert the HW command, the same as typing in the Terminal
          //string val = result.FormatedString; //the result of the command, returns the the result if the command was succesfull
          //val = GetTempFromResult(val); //leaving only the number, changed from hexadecimal to decimal number- should be between 0 to 70
          //Console.WriteLine("temp = " + val);
          //  int x = Convert.ToInt32(val);

          //  if (x > 0 && x < 70)
          //  {
          //      Console.WriteLine("The IMUTemperatureWithoutStreamingTest was successfull! Horray!");
          //  }
          //  else
          //  {
          //      Console.WriteLine("The IMUTemperatureWithoutStreamingTest failed! Boo!");
          //  }
        }

   
        
       //this code is used for getting the temperature (GTEMP MM_TEMP):
       private string GetTempFromResult(string val)
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

    }
}
